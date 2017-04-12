using System;
using System.Net;
using System.Net.Http;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xamarin.Forms;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.IO;

namespace SQLConnect
{
	public partial class ProductPage : ContentPage
	{
		ProductListItem product;

		string[] medicalAmounts;
		double[] medicalPrices;
		int discountType;
		int index;
		byte[] imageBytes;


		public ProductPage(bool dealLink)
		{
			InitializeComponent();

			if (dealLink)
			{
				product = Statics.Default.getDeal();
			}else
			{
				product = Statics.Default.getProdClicked();
			}

			Title = product.prodName;
			image.Source = product.prodImgUrl;

			medicalAmounts = new string[] { "Gram", "Eighth\n(~3.5g)", "Quarter\n(~7g)", "Half Oz\n(~14g)", "Ounce\n(~28g)" };
			//Discount?
			discountType = product.prodBulkType;
			//Populate price list for flowers.
			//If discount available, it is applied iteratively to amounts greater than a gram. This value could be set by the dispensary, maybe for each item?
			//1 gram is base price
			switch (discountType)
			{
				case 0:
					//No discount.
					medicalPrices = new double[5];
					medicalPrices[0] = product.prodUnitPrice;
					medicalPrices[1] = 3.54688 * product.prodUnitPrice;
					medicalPrices[2] = 3.54688 * 2 * product.prodUnitPrice;
					medicalPrices[3] = 3.54688 * 2 * 2 * product.prodUnitPrice;
					medicalPrices[4] = 3.54688 * 2 * 2 * 2 * product.prodUnitPrice;
					break;
				case 1:
					//Linear discount progression, 1 oz being the maximum discount(specified amount)
					medicalPrices = new double[5];
					medicalPrices[0] = product.prodUnitPrice;//None
					medicalPrices[1] = 3.54688 * product.prodUnitPrice * (1-product.prodBulkDiscount*1/4);// fourth of discount
					medicalPrices[2] = 3.54688 * 2 * product.prodUnitPrice * (1 - product.prodBulkDiscount * 1 / 2);// half of discount
					medicalPrices[3] = 3.54688 * 2 * 2 * product.prodUnitPrice * (1 - product.prodBulkDiscount * 3 / 4);// three/fourths of discount
					medicalPrices[4] = 3.54688 * 2 * 2 * 2 * product.prodUnitPrice * (1 - product.prodBulkDiscount);// max discount
					break;
				case 2:
					//each step up gives half the discount of the previous step.
					medicalPrices = new double[5];
					medicalPrices[0] = product.prodUnitPrice;
					medicalPrices[1] = 3.54688 * product.prodUnitPrice * (1-(product.prodBulkDiscount/2));//Half discount
					medicalPrices[2] = 3.54688 * 2 * product.prodUnitPrice * (1 - (product.prodBulkDiscount* 3/4));//Fourth more
					medicalPrices[3] = 3.54688 * 2 * 2 * product.prodUnitPrice * (1 - (product.prodBulkDiscount*7/8));//Eighth more
					medicalPrices[4] = 3.54688 * 2 * 2 * 2 * product.prodUnitPrice * (1 - (product.prodBulkDiscount*15/16));//Sixteenth more
					break;
				default:
					//No discount.
					medicalPrices = new double[5];
					medicalPrices[0] = product.prodUnitPrice;
					medicalPrices[1] = 3.54688 * product.prodUnitPrice;
					medicalPrices[2] = 3.54688 * 2 * product.prodUnitPrice;
					medicalPrices[3] = 3.54688 * 2 * 2 * product.prodUnitPrice;
					medicalPrices[4] = 3.54688 * 2 * 2 * 2 * product.prodUnitPrice;
					break;
			}

			index = 0;
			priceExact.Text = medicalPrices[0].ToString("C");
			priceExactRate.Text = "(" + medicalPrices[0].ToString("C") + "/g)";

			//Get information on what type of choices to display.

			if (Statics.Default.getCreds()[12].Equals("1")&&Statics.Default.IsEditing())
			{
				editView.IsVisible = true;
				browseView.IsVisible = false;

				//Init edit values.
				editPic.Source = product.prodImgUrl;
				editDesc.Text = product.prodDescription;
				editBulk.Text = (product.prodBulkDiscount * 100).ToString();
				editUnit.Text = product.prodUnitPrice.ToString("F");
				editIncUnit.Text = product.prodUnitPriceIncentive.ToString();
				editIncFlag.IsToggled = product.prodIncentiveFlag;
				//Select proper bulk type
				string[] bulkTypes = new string[] { "None", "Linear", "Diminishing" };
				foreach (string s in bulkTypes)
				{
					editBulkType.Items.Add(s);
				}
				editBulkType.SelectedIndex = discountType;
				editDealFlag.IsToggled = product.prodDealFlag;
				editDiscount.Text = (product.prodDiscount * 100).ToString();
			}
			else
			{
				if (product.prodCategory.Equals("Flowers"))
				{
					componentExact.IsVisible = true;
				}
				else
				{
					componentRegular.IsVisible = true;
				}
			}

			price.Text = product.prodUnitPrice.ToString("C");

			Debug.WriteLine("editView: " + editView.IsVisible + " browse: " + browseView.IsVisible);
		}



		void increment(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();
			if (int.Parse(value.Text) > 9) { return; }
			else {
				value.Text = (int.Parse(value.Text) + 1).ToString();
				price.Text = (int.Parse(value.Text) * product.prodUnitPrice).ToString("C");
			}
		}



		void decrement(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();
			if (int.Parse(value.Text) < 2) { return; }
			else {
				value.Text = (int.Parse(value.Text) - 1).ToString();
				price.Text = (int.Parse(value.Text) * product.prodUnitPrice).ToString("C");
			}
		}



		void previous(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();
			if (index == 0)
			{
				return;
			}
			else {
				index--;
				if (index == 0) { valueLeft.Text = ""; }
				else { valueLeft.Text = medicalAmounts[index -1]; }
				valueMid.Text = medicalAmounts[index];
				valueRight.Text = medicalAmounts[index + 1];
				double grams = 0;
				switch (index)
				{
					case 0:
						grams = 1;
						break;
					case 1:
						grams = 3.54688;
						break;
					case 2:
						grams = 3.54688 * 2;
						break;
					case 3:
						grams = 3.54688 * 4;
						break;
					case 4:
						grams = 3.54688 * 8;
						break;
					default:
						break;
				}
				priceExact.Text = medicalPrices[index].ToString("C");
				priceExactRate.Text = "(" + (medicalPrices[index] / grams).ToString("C") + "/g)";
			}
		}



		void next(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();
			if (index == 4)
			{
				return;
			}
			else {
				index++;
				if (index == 4) { valueRight.Text = ""; }
				else {valueRight.Text = medicalAmounts[index + 1];}
				valueMid.Text = medicalAmounts[index];
				valueLeft.Text = medicalAmounts[index - 1];
				double grams = 0;
				switch (index)
				{
					case 0:
						grams = 1;
						break;
					case 1:
						grams = 3.54688;
						break;
					case 2:
						grams = 3.54688 * 2;
						break;
					case 3:
						grams = 3.54688 * 4;
						break;
					case 4:
						grams = 3.54688 * 8;
						break;
					default:
						break;
				}
				priceExact.Text = medicalPrices[index].ToString("C");
				priceExactRate.Text = "(" + (medicalPrices[index] / grams).ToString("C") + "/g)";
			}
		}




		public async void pickPic(object s, EventArgs e)
		{
			//Get byte[] of selected pic.
			imageBytes = await PickPictureAsync(null);
			if (imageBytes != null)
			{
				Debug.WriteLine("ImageBytes is no longer empty.");

				/*//UNCOMMENT THIS FOR QUICK IMAGE UPDATE THROUGH UPLOADPICTURE.PHP CHECK SQL COMMAND FOR SAFETY
				//Connect to url.
				var client = new HttpClient();

				var contentSent = new MultipartFormDataContent();

				if (imageBytes != null)
				{
					contentSent.Add(new ByteArrayContent(imageBytes), "image");
				}

				//Show that we are waiting for a response and wait for it.
				var response = await client.PostAsync("http://cbd-online.net/landon/uploadPicture.php", contentSent);

				var output = await response.Content.ReadAsStringAsync();

				Debug.WriteLine(output);
				*/

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

			}
			catch (Exception ex)
			{
				Debug.WriteLine("\nIn PictureViewModel.PickPictureAsync() - Exception:\n{0}\n", ex);//TODO: Do something more useful
				return null;
			}
			//Receipts.Add(ImageSource.FromFile(filePath));

			//Uris.Add(filePath);

			//return imageName;
		}




		async void addToCart(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();
			CartListItem cartItem;
			if (product.prodCategory.Equals("Flowers"))
			{
				double amount;
				string unit;
				string rate;
				switch (index)
				{
					case 0:
						amount = (double)1;
						unit = "g";
						rate = "(" + (medicalPrices[0]).ToString("C") + "/g)";
						break;
					case 1:
						amount = (double)1 / 8;
						unit = "oz";
						rate = "("+(medicalPrices[1]/3.54688).ToString("C")+"/g)";
						break;
					case 2:
						amount = (double)1 / 4;
						unit = "oz";
						rate = "(" + (medicalPrices[2] / (3.54688*2)).ToString("C") + "/g)";
						break;
					case 3:
						amount = (double)1 / 2;
						unit = "oz";
						rate = "(" + (medicalPrices[3] / (3.54688 * 4)).ToString("C") + "/g)";
						break;
					default:
						amount = (double)1;
						unit = "oz";
						rate = "(" + (medicalPrices[4] / (3.54688 * 8)).ToString("C") + "/g)";
						break;
				}
				cartItem = new CartListItem { prodName = product.prodName, prodIsRegular=false, prodIsFlower=true, prodAmount = amount, prodRate = rate, prodUnitType = unit, prodTotal = medicalPrices[index].ToString("C")};
			}
			else {
				cartItem = new CartListItem { prodName = product.prodName, prodIsRegular=true, prodIsFlower = false, prodAmount = double.Parse(value.Text), prodUnitType = "", prodTotal = (product.prodUnitPrice * double.Parse(value.Text)).ToString("C") };
			}
			ObservableCollection<CartListItem> pulled = Statics.Default.getCartItems();

			pulled.Add(cartItem);

			Statics.Default.setCartItems(pulled);

			var answer = await DisplayAlert("Added to Cart", "Would you like to keep browsing?", "Keep Browsing", "View Cart");

			if (answer)
			{
				await Navigation.PopModalAsync();
			}
			else {
				NavigationPage nav = new NavigationPage(new CartTab { Title = "Your Cart" });
				NavigationPage.SetHasBackButton(nav, true);
				await Navigation.PushModalAsync(nav);
			}
		}




		async void saveEdits(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();

			var answer = await DisplayAlert("Save Changes", "Review your changes for accuracy. Are you sure you want to make these changes?", "Yes", "No");

			bool valid = true;
			string error = "";

			/*Checklist*/

			//Fails if any info is empty.
			if(String.IsNullOrEmpty(editDesc.Text) ||
			   String.IsNullOrEmpty(editUnit.Text) ||
			   //String.IsNullOrEmpty(newIncUnit.Text) || NOT REQUIRED YET
			   String.IsNullOrEmpty(editBulkType.Items[editBulkType.SelectedIndex]))
			{
				valid = false;
				error = "No fields can be empty.";
			}

			//Fails if either discount is applied but set to less than 5%.
			if ((int.Parse(editDiscount.Text) < 5 && editDealFlag.IsToggled) || (int.Parse(editBulk.Text) < 5 && editBulkType.SelectedIndex > 0))
			{
				valid = false;
				error = "Discount cannot be less than 5%, increase discount % or disable the discount(Set 'Bulk discount' to none or disable 'Discount applied'";
			}

			//Possibly create safeguards for someone trying to set an item to be free.

			if (answer&&valid)
			{
				//Upload changes, notify manager that they can browse products normally to see exactly how these changes look to the users.

				//Check inputs
				string deal, inc;
				if (editDealFlag.IsToggled) { deal = "1"; } else { deal = "0"; }
				if (editIncFlag.IsToggled) { inc = "1"; } else { inc = "0"; }
				double discount = double.Parse(editDiscount.Text) / 100;
				double bulkDiscount = double.Parse(editBulk.Text) / 100;

				//Connect to url.
				var client = new HttpClient();

				var contentSent = new MultipartFormDataContent();
				contentSent.Add(new StringContent("1"), "operation");
				contentSent.Add(new StringContent(Statics.Default.getCreds()[16]), "dispId");
				contentSent.Add(new StringContent(editDesc.Text), "desc");
				if (imageBytes != null)
				{
					contentSent.Add(new ByteArrayContent(imageBytes), "picBytes");
				}
				contentSent.Add(new StringContent(editUnit.Text), "unitPrice");
				contentSent.Add(new StringContent(editIncFlag.IsToggled.ToString()), "incFlag");
				contentSent.Add(new StringContent(editIncUnit.Text), "incUnitPrice");
				contentSent.Add(new StringContent(editDealFlag.IsToggled.ToString()), "dealFlag");
				contentSent.Add(new StringContent(editDiscount.Text), "discount");
				contentSent.Add(new StringContent(editBulkType.Items[editBulkType.SelectedIndex]), "bulkType");
				contentSent.Add(new StringContent(editBulk.Text), "bulkDiscount");

				//Show that we are waiting for a response and wait for it.
				var response = await client.PostAsync("http://cbd-online.net/landon/addOrEditProduct.php", contentSent);

				Debug.WriteLine(Statics.Default.getCreds()[16]);
				Debug.WriteLine(editDesc.Text);
				Debug.WriteLine(product.prodImgUrl);
				Debug.WriteLine(editUnit.Text);
				Debug.WriteLine(inc);
				Debug.WriteLine(editIncUnit.Text);
				Debug.WriteLine(deal);
				Debug.WriteLine(discount.ToString());
				Debug.WriteLine(editBulkType.SelectedIndex.ToString());
				Debug.WriteLine(bulkDiscount.ToString());

				var output = await response.Content.ReadAsStringAsync();

				string[] components = output.Split(new string[] { "\n" }, StringSplitOptions.None);

				Debug.WriteLine(output);

				if (components[0].Equals("true"))
				{
					await DisplayAlert("Success", "Item values modified. You can see your new inventory list by logging in again.", "Okay");
					//change locally
					ObservableCollection<ProductListItem> pulled = Statics.Default.getProducts();
					pulled.Remove(product);
					product.prodDescription = editDesc.Text;
					//Change img
					product.prodUnitPrice = double.Parse(editUnit.Text);
					product.prodIncentiveFlag = editIncFlag.IsToggled;
					product.prodUnitPriceIncentive = double.Parse(editIncUnit.Text);
					product.prodDealFlag = editDealFlag.IsToggled;
					product.prodDiscount = double.Parse(editDiscount.Text);
					product.prodBulkType = editBulkType.SelectedIndex;
					product.prodBulkDiscount = bulkDiscount;

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
				//Display error
				await DisplayAlert("Error", error, "Okay");
			}
		}



		void cancelEdits(object s, EventArgs e)
		{
			//Abort changes.
			Navigation.PopModalAsync();
		}
	}
}
