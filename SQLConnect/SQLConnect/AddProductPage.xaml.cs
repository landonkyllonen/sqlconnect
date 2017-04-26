using System;
using System.Net.Http;
using System.IO;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Plugin.Media.Abstractions;
using System.Diagnostics;
using Plugin.Media;
using System.Threading.Tasks;
using System.Text;
using System.Linq;

namespace SQLConnect
{
	public partial class AddProductPage : ContentPage
	{
		//Things added for plugin
		public List<string> Uris;
		private ObservableCollection<MediaFile> _images;
		public ObservableCollection<MediaFile> Images
		{
			get { return _images ?? (_images = new ObservableCollection<MediaFile>()); }
			set
			{
				if (_images != value)
				{
					_images = value;
					OnPropertyChanged();
				}
			}
		}

		//Mine
		byte[] imageBytes;

		public AddProductPage()
		{
			InitializeComponent();

			string[] categories = new string[] { "Flowers", "Concentrates", "Edibles", "Glass", "Apparel" };
			foreach (string s in categories)
			{
				newCat.Items.Add(s);
			}
			newCat.SelectedIndexChanged += catChanged;
			string[] bulkTypes = new string[] { "None", "Linear", "Diminishing" };
			foreach (string s in bulkTypes)
			{
				newBulkType.Items.Add(s);
			}
			newBulkType.SelectedIndexChanged += bulkChanged;
		}

		async void saveNew(object s, EventArgs e)
		{
			string error = "";

			/*Checklist*/

			//Fails if item already exists in this dispensary.
			foreach (ProductListItem p in Statics.Default.getProducts())
			{
				if (p.prodName.Equals(newName.Text)) { error = "This item name already exists at your dispensary."; break; }
			}

			//Fails if any info is empty.
			if (String.IsNullOrEmpty(newName.Text) ||
			    String.IsNullOrEmpty(newDesc.Text) || newDesc.Text.Equals("Enter your description.") ||
				 String.IsNullOrEmpty(newUnit.Text) ||
				(String.IsNullOrEmpty(newBulk.Text) && newBulkType.SelectedIndex > 0) ||
				(String.IsNullOrEmpty(newDiscount.Text) && newDealFlag.IsToggled) ||
				(String.IsNullOrEmpty(newIncUnit.Text) && newIncFlag.IsToggled) ||
			    newBulkType.SelectedIndex<0 || newCat.SelectedIndex < 0 ||
			    (String.IsNullOrEmpty(newBulkLimit.Text) && newCat.SelectedIndex>0 && newBulkType.SelectedIndex > 0) ||
			    (String.IsNullOrEmpty(newBulkInterval.Text) && newCat.SelectedIndex > 0 && newBulkType.SelectedIndex>0))
			{
				error = "No fields can be empty.";
				//Display error
				await DisplayAlert("Error", error, "Okay");
				return;
			}

			//Fails if image isn't provided.
			if (imageBytes == null) {
				error = "No image data was selected.";
				await DisplayAlert("Error", error, "Okay");
				return;
			}

			//Fails if either discount is applied but set to 0%.
			if ((int.Parse(newDiscount.Text) < 5 && newDealFlag.IsToggled) || 
			    (int.Parse(newBulk.Text) < 5 && newBulkType.SelectedIndex > 0))
			{
				error = "Discount cannot be less than 5%, increase discount % or disable the discount(Set 'Bulk discount' to none or disable 'Discount applied'";
				await DisplayAlert("Error", error, "Okay");
				return;
			}

			var answer = await DisplayAlert("Add this Item", "Review your submission for accuracy. Are you sure you want to make this product available for sale with these values?", "Yes", "Review");
			if (answer)
			{
				//Connect to url.
				var client = new HttpClient();

				//Format inputs.
				string deal, inc;
				if (newDealFlag.IsToggled) { deal = "1"; } else { deal = "0"; }
				if (newIncFlag.IsToggled) { inc = "1"; } else { inc = "0"; }
				double discount = double.Parse(newDiscount.Text) / 100;
				double bulkDiscount = double.Parse(newBulk.Text) / 100;

				var contentSent = new MultipartFormDataContent();
				contentSent.Add(new StringContent("0"), "operation");
				contentSent.Add(new StringContent(Statics.Default.getCreds()[16]), "dispId");
				contentSent.Add(new StringContent(newName.Text), "name");
				contentSent.Add(new StringContent(newCat.Items[newCat.SelectedIndex]), "cat");
				contentSent.Add(new StringContent(newDesc.Text), "desc");
				contentSent.Add(new ByteArrayContent(imageBytes), "picBytes");
				contentSent.Add(new StringContent(newUnit.Text), "unitPrice");
				contentSent.Add(new StringContent(inc), "incFlag");
				contentSent.Add(new StringContent(newIncUnit.Text), "incUnitPrice");
				contentSent.Add(new StringContent(deal), "dealFlag");
				contentSent.Add(new StringContent(discount.ToString()), "discount");
				contentSent.Add(new StringContent(newBulkType.Items[newBulkType.SelectedIndex].ToString()), "bulkType");
				contentSent.Add(new StringContent(bulkDiscount.ToString()), "bulkDiscount");
				if (newCat.SelectedIndex > 0 && newBulkType.SelectedIndex>0)
				{
					contentSent.Add(new StringContent(newBulkLimit.Text), "bulkLimit");
					contentSent.Add(new StringContent(newBulkInterval.Text), "bulkInterval");
				}

				//Show that we are waiting for a response and wait for it.
				var response = await client.PostAsync("http://cbd-online.net/landon/addOrEditProduct.php", contentSent);

				var output = await response.Content.ReadAsStringAsync();

				string[] components = output.Split(new string[] { "\n" }, StringSplitOptions.None);

				if (components[0].Equals("true"))
				{
					await DisplayAlert("Success", "Item values modified. You can see your edited inventory list by navigating to products section.", "Okay");
					//Change locally.
					ObservableCollection<ProductListItem> pulled = Statics.Default.getProducts();
					ProductListItem product = new ProductListItem();

					product.prodName = newName.Text;
					product.prodCategory = newCat.Items[newCat.SelectedIndex];
					product.prodDescription = newDesc.Text;
					product.prodImgSrc = newPic.Source;
					product.prodUnitPrice = double.Parse(newUnit.Text);
					product.prodIncentiveFlag = newIncFlag.IsToggled;
					product.prodUnitPriceIncentive = double.Parse(newIncUnit.Text);
					product.prodDealFlag = newDealFlag.IsToggled;
					product.prodDiscount = double.Parse(newDiscount.Text);
					product.prodBulkType = newBulkType.SelectedIndex;
					product.prodBulkDiscount = double.Parse(newBulk.Text);
					if (newCat.SelectedIndex > 0 && newBulkType.SelectedIndex > 0)
					{
						product.prodBulkLimit = int.Parse(newBulkLimit.Text);
						product.prodBulkInterval = int.Parse(newBulkInterval.Text);
					}

					pulled.Add(product);
					Statics.Default.setProducts(pulled);

					await Navigation.PopModalAsync();
				}
				else
				{
					await DisplayAlert("Error", "Sorry, there was a problem uploading your changes.", "Okay");
				}
			}
		}

		public async void pickPic(object s, EventArgs e)
		{
			//Get byte[] of selected pic.
			imageBytes = await PickPictureAsync(null);
			if (imageBytes != null)
			{
				Debug.WriteLine("ImageBytes is no longer empty.");
				ImageSource src = ImageSource.FromStream(() => new MemoryStream(imageBytes));
				newPic.Source = src;

				//Direct FTP attempt that was alternative to saving images on SQL
				/*if (Device.OS == TargetPlatform.Android || Device.OS == TargetPlatform.iOS)
					DependencyService.Get<IFtpWebRequest>().upload("ftp://82.221.139.182", localpath, "landon@cbd-online.net", "Sol@rwind99", "/landon");
					//await DisplayAlert("Upload", DependencyService.Get<IFtpWebRequest>().upload("ftp://82.221.139.182", localpath, "landon@cbd-online.net", "Sol@rwind99", "/landon"), "Ok");
				*/
			}
			else
			{
				Debug.WriteLine("No image data was collected.");
			}
		}

		/// <summary>
		/// Allows the user to pick a photo on their device using the native photo handlers and returns a stream, which is converted to a byte[].
		/// </summary>
		/// <returns>string, the name of the image if everything went ok, 'false' if an exception was generated, and 'notfalse' if they simply canceled.</returns>
		//Leaving useless parts in case needed later for customization.
		public async Task<byte[]> PickPictureAsync(PickMediaOptions options = null)
		{
			await CrossMedia.Current.Initialize();

			MediaFile file = null;
			string filePath = string.Empty;
			string imageName = string.Empty;

			try
			{
				file = await CrossMedia.Current.PickPhotoAsync();

				if (file == null)
				{
					return null;
				}//Not returning false here so we do not show an error if they simply hit cancel from the device's image picker
				else
				{
					filePath = file.Path;/* Add your own logic here for where to save the file */ //_fileHelper.CopyFile(file.Path, imageName);
					imageName = "SomeImageName.jpg";

					var memoryStream = new MemoryStream();
					file.GetStream().CopyTo(memoryStream);
					file.Dispose();

					byte[] uncompressed = memoryStream.ToArray();
					var compressed = DependencyService.Get<IFileProcessing>().compress(uncompressed, GetImageFormat(uncompressed));
					if (compressed != null)
					{
						return compressed;
					}
					else
					{
						Debug.WriteLine("Compression did not work properly.");
						return null;
					}
				}
			
			}catch (Exception ex)
			{
				Debug.WriteLine("\nIn PictureViewModel.PickPictureAsync() - Exception:\n{0}\n", ex);//TODO: Do something more useful
				return null;
			}
			//Receipts.Add(ImageSource.FromFile(filePath));

			//Uris.Add(filePath);

			//return imageName;
		}

		public string GetImageFormat(byte[] bytes)
		{
			// see http://www.mikekunz.com/image_file_header.html
			var png = new byte[] { 137, 80, 78, 71 };    // PNG
			var jpeg = new byte[] { 255, 216, 255, 224 }; // jpeg
			var jpeg2 = new byte[] { 255, 216, 255, 225 }; // jpeg canon

			var buffer = new byte[4];
			buffer[0] = bytes[0];
			buffer[1] = bytes[1];
			buffer[2] = bytes[2];
			buffer[3] = bytes[3];

			if (png.SequenceEqual(buffer.Take(png.Length)))
				return "png";

			if (jpeg.SequenceEqual(buffer.Take(jpeg.Length)))
				return "jpg";

			if (jpeg2.SequenceEqual(buffer.Take(jpeg2.Length)))
				return "jpg";

			throw new NotSupportedException("The image type is not supported, supported types are of type PNG or JPEG.");
		}

		void catChanged(object s, EventArgs e)
		{
			//Show controls specific to regular items.
			if (newCat.SelectedIndex > 0)
			{
				newPriceLbl.Text = "Price/unit:";
				regBulk1.IsVisible = true;
				regBulk2.IsVisible = true;
			}
			//Show just flower controls.
			else
			{
				newPriceLbl.Text = "Price/g:";
				regBulk1.IsVisible = false;
				regBulk2.IsVisible = false;
			}
		}

		void bulkChanged(object s, EventArgs e)
		{
			if (newBulkType.SelectedIndex > 0)
			{
				newBulk.IsEnabled = true;
				newBulkLimit.IsEnabled = true;
				newBulkInterval.IsEnabled = true;
			}
			else
			{
				newBulk.IsEnabled = false;
				newBulkLimit.IsEnabled = false;
				newBulkInterval.IsEnabled = false;
			}
		}

		void showBulkHelp(object s, EventArgs e)
		{
			DisplayAlert("Bulk Types:", "Linear: Discount approaches maximum % specified in equal increments. E.G. 5%, 10%, 15%, %20%...(With max 30%)\n" +
			             "Diminishing: Discount approaches maximum % specified more slowly with each step. E.G. 15%, ~22%, ~26%, ~28%...(With max 30%)",
			            "Close");
		}

		void cancelNew(object s, EventArgs e)
		{
			Navigation.PopModalAsync();
		}
	}
}
