using System;
using System.Net;
using System.Net.Http;
using System.IO;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Plugin.Media.Abstractions;
using System.Diagnostics;
using Plugin.Media;
using System.Threading.Tasks;

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
			string[] bulkTypes = new string[] { "None", "Linear", "Diminishing" };
			foreach (string s in bulkTypes)
			{
				newBulkType.Items.Add(s);
			}
		}

		async void saveNew(object s, EventArgs e)
		{
			var answer = await DisplayAlert("Add this Item", "Review your submission for accuracy. Are you sure you want to make this product available for sale with these values?", "Yes", "No");

			bool valid = true;
			string error = "";

			/*Checklist*/

			//Fails if item already exists in this dispensary.
			foreach (ProductListItem p in Statics.Default.getProducts())
			{
				if (p.prodName.Equals(newName.Text)) { valid = false; error = "This item name already exists at your dispensary."; break; }
			}
			//Fails if any info is empty.
			if (String.IsNullOrEmpty(newName.Text) ||
			   String.IsNullOrEmpty(newCat.Items[newCat.SelectedIndex]) ||
			   String.IsNullOrEmpty(newDesc.Text) ||
			   String.IsNullOrEmpty(newUnit.Text) ||
			   //String.IsNullOrEmpty(newIncUnit.Text) || NOT REQUIRED YET
			   String.IsNullOrEmpty(newBulkType.Items[newBulkType.SelectedIndex]))
			{
				valid = false; 
				error = "You must fill in all fields.";
			}

			//Fails if image isn't provided.
			if (imageBytes == null) { valid = false; error = "No image data was selected."; }

			//Fails if either discount is applied but set to 0%.
			if ((int.Parse(newDiscount.Text) < 5 && newDealFlag.IsToggled) || (int.Parse(newBulk.Text) < 5 && newBulkType.SelectedIndex > 0))
			{
				valid = false;
				error = "Discount cannot be less than 5%, increase discount % or disable the discount(Set 'Bulk discount' to none or disable 'Discount applied'";
			}

			if (answer && valid)
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
				contentSent.Add(new StringContent(newBulkType.Items[newBulkType.SelectedIndex]), "bulkType");
				contentSent.Add(new StringContent(bulkDiscount.ToString()), "bulkDiscount");

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
					//Change img
					product.prodUnitPrice = double.Parse(newUnit.Text);
					product.prodIncentiveFlag = newIncFlag.IsToggled;
					product.prodUnitPriceIncentive = double.Parse(newIncUnit.Text);
					product.prodDealFlag = newDealFlag.IsToggled;
					product.prodDiscount = double.Parse(newDiscount.Text);
					product.prodBulkType = newBulkType.SelectedIndex;
					product.prodBulkDiscount = double.Parse(newBulk.Text);

					pulled.Add(product);
					Statics.Default.setProducts(pulled);

					await Navigation.PopModalAsync();
				}
				else
				{
					await DisplayAlert("Error", "Sorry, there was a problem uploading your changes.", "Okay");
				}
			}
			else
			{
				await DisplayAlert("Error", error, "Okay");
			}

		}

		public async void pickPic(object s, EventArgs e)
		{
			//Get byte[] of selected pic.
			imageBytes = await PickPictureAsync(null);
			if (imageBytes != null)
			{
				Debug.WriteLine("ImageBytes is no longer empty.");



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
					return memoryStream.ToArray();
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

		void cancelNew(object s, EventArgs e)
		{
			Navigation.PopModalAsync();
		}
	}
}
