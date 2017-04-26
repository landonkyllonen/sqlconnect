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
using System.Linq;

namespace SQLConnect
{
	public partial class ProductPage : ContentPage
	{
		ProductListItem product;

		ImageSource ogSrc;

		string[] medicalAmounts;
		string[] bulkTypes;
		double[] medicalPrices;
		double[] regularPrices;

		double discountMult;

		double totalDiscount;

		//Regular
		int bulkInterval;
		int bulkLimit;

		//For flower amount position.
		int index;
		//For regular amount position.
		int regIndex;

		byte[] imageBytes;


		public ProductPage(bool dealLink)
		{
			InitializeComponent();

			//Get Item clicked, is it from home page or list of items?
			if (dealLink)
			{
				product = Statics.Default.getDeal();
			}
			else
			{
				product = Statics.Default.getProdClicked();
			}

			//INITIALIZATIONS
			Title = product.prodName;
			index = 0;
			regIndex = 0;
			bulkLimit = product.prodBulkLimit;
			bulkInterval = product.prodBulkInterval;
			editBulkType.SelectedIndexChanged += bulkChanged;

			//Try to set image.
			image.Source = product.prodImgSrc;
			ogSrc = product.prodImgSrc;
			//If image didn't finish loading before selection, load it.
			if (product.prodImgSrc == null)
			{
				//This gets and sets image for all relevant views.
				gatherItemImage();
			}

			//Flower amount options.
			medicalAmounts = new string[] { "Gram", "Eighth\n(~3.5g)", "Quarter\n(~7g)", "Half Oz\n(~14g)", "Ounce\n(~28g)" };
			//Regular item amounts are numeric and capped at 20.

			//Bulk discount options.
			bulkTypes = new string[] { "None", "Linear", "Diminishing" };

			//Discount mult does nothing to pricing unless dealflag is set.
			if (product.prodDealFlag)
			{
				discountMult = 1 - product.prodDiscount;
			}
			else
			{
				discountMult = 1;
			}



			//Calculation of pricing for each quantity.
			switch (product.prodBulkType)
			{
				case 0:
					//No discount.
					medicalPrices = new double[5];
					medicalPrices[0] = product.prodUnitPrice*discountMult;
					medicalPrices[1] = 3.54688 * product.prodUnitPrice* discountMult;
					medicalPrices[2] = 3.54688 * 2 * product.prodUnitPrice* discountMult;
					medicalPrices[3] = 3.54688 * 2 * 2 * product.prodUnitPrice* discountMult;
					medicalPrices[4] = 3.54688 * 2 * 2 * 2 * product.prodUnitPrice* discountMult;
					regularPrices = new double[20];
					for (int i = 0; i < 20; i++)
					{
						regularPrices[i] = product.prodUnitPrice * (i+1) * discountMult;
					}
					break;
				case 1:
					//Linear discount progression, 1 oz being the maximum discount(specified amount)
					medicalPrices = new double[5];
					medicalPrices[0] = product.prodUnitPrice* discountMult;//None
					medicalPrices[1] = 3.54688 * product.prodUnitPrice * (1 - product.prodBulkDiscount * 1 / 4)* discountMult;// fourth of discount
					medicalPrices[2] = 3.54688 * 2 * product.prodUnitPrice * (1 - product.prodBulkDiscount * 1 / 2)* discountMult;// half of discount
					medicalPrices[3] = 3.54688 * 2 * 2 * product.prodUnitPrice * (1 - product.prodBulkDiscount * 3 / 4)* discountMult;// three/fourths of discount
					medicalPrices[4] = 3.54688 * 2 * 2 * 2 * product.prodUnitPrice * (1 - product.prodBulkDiscount)* discountMult;// max discount
					regularPrices = new double[20];
					for (int i = 0; i < 20; i++)
					{
						double currentBulkDiscount;
						double discountIntervals = bulkLimit / bulkInterval;
						//% per interval is given by dividing the maximum discount by how many intervals are specified.
						//E.g. If the limit is set at 20 items with 20% max discount, and the intervals are every 5 items,
						//Your discount would be 20%/(20/5) = 20%/4 = 5% per interval.
						double discountPerInterval = product.prodBulkDiscount / discountIntervals;

						//Intervals reached is #items/items needed for each step of the discount. We round this down.
						//E.g. if interval is 4 and disPerInt is 5%, buying 7 items gives same discount as buying 4.
						double intervalsReached = Math.Floor((double)(i+1) / bulkInterval);

						//Cap intervalsreached at limit set.
						//If reached > or = to intervals allowed by limit...
						//Equalize.
						if (intervalsReached >= discountIntervals)
						{
							intervalsReached = discountIntervals;
						}

						//So current discount is given by intervals reached * discountPerInterval.
						currentBulkDiscount = intervalsReached * discountPerInterval;

						//This is 1-currentBulkDiscount because discounts from server are given as 0.2 for 20%.
						//So to take 20% off we multiply by 0.8.
						//Note: this would not be the same as dividing by 0.2.
						regularPrices[i] = product.prodUnitPrice * (i+1) * discountMult * (1-currentBulkDiscount);
						//Discounts here are multiplicative, not additive.
						//This is to the benefit of the dispensary, as adding discounts together would be a larger discount.
					}
					break;
				case 2:
					//each step up gives half the discount of the previous step.
					medicalPrices = new double[5];
					medicalPrices[0] = product.prodUnitPrice* discountMult;
					medicalPrices[1] = 3.54688 * product.prodUnitPrice * (1 - (product.prodBulkDiscount / 2))* discountMult;//Half discount
					medicalPrices[2] = 3.54688 * 2 * product.prodUnitPrice * (1 - (product.prodBulkDiscount * 3 / 4))* discountMult;//Fourth more
					medicalPrices[3] = 3.54688 * 2 * 2 * product.prodUnitPrice * (1 - (product.prodBulkDiscount * 7 / 8))* discountMult;//Eighth more
					medicalPrices[4] = 3.54688 * 2 * 2 * 2 * product.prodUnitPrice * (1 - (product.prodBulkDiscount))* discountMult;//Sixteenth more
					regularPrices = new double[20];
					for (int i = 0; i < 20; i++)
					{
						double cumulativeDiscount;
						double discountIntervals = bulkLimit / bulkInterval;
						//% per interval decreases by half each interval.
						double intervalsReached = Math.Floor((double)(i+1) / bulkInterval);

						//Cap intervalsreached at limit set.
						//If reached > or = to intervals allowed by limit...
						//Equalize.
						if (intervalsReached >= discountIntervals)
						{
							intervalsReached = discountIntervals;
						}

						//We see from the medicalPrices examples that there is a pattern.
						//If variable n is 2 to the power of intervals reached,
						//Discount = 1-maxdiscount * (n-1/n).
						//For example, take the 4th interval: We have divided the discount in half 4 times.
						//In terms of 16ths, this gives us 8/16 + 4/16 + 2/16 + 1/16 = 15/16.
						int n = (int) Math.Pow(2,intervalsReached);

						//If achieved limit, give full discount.
						if ((int)intervalsReached == (int)discountIntervals)
						{
							cumulativeDiscount = product.prodBulkDiscount;
						}//Else, get closer by half.
						else
						{
							cumulativeDiscount = product.prodBulkDiscount * (n - 1) / n;
						}

						regularPrices[i] = product.prodUnitPrice * (i+1) * discountMult * (1 - cumulativeDiscount);
						Debug.WriteLine("Price calc'd: " + regularPrices[i]);
					}
					break;
				default:
					//No discount.
					medicalPrices = new double[5];
					medicalPrices[0] = product.prodUnitPrice* discountMult;
					medicalPrices[1] = 3.54688 * product.prodUnitPrice* discountMult;
					medicalPrices[2] = 3.54688 * 2 * product.prodUnitPrice* discountMult;
					medicalPrices[3] = 3.54688 * 2 * 2 * product.prodUnitPrice* discountMult;
					medicalPrices[4] = 3.54688 * 2 * 2 * 2 * product.prodUnitPrice* discountMult;
					regularPrices = new double[20];
					for (int i = 0; i < 20; i++)
					{
						regularPrices[i] = product.prodUnitPrice * (i+1) * discountMult;
					}
					break;
			}

			//Initialize price displays with calculated values.
			priceExact.Text = medicalPrices[0].ToString("C");
			priceExactRate.Text = "(" + medicalPrices[0].ToString("C") + "/g)";
			if (medicalPrices[0] < product.prodUnitPrice)
			{
				totalDiscount = (1-medicalPrices[0] / product.prodUnitPrice) * 100;
				priceExactOff.Text = Math.Round(totalDiscount, MidpointRounding.AwayFromZero).ToString() + "%\noff";
				priceExactOff.IsVisible = true;
			}
			price.Text = regularPrices[0].ToString("C");
			priceRegRate.Text = "(" + regularPrices[0].ToString("C") + "/item)";
			if (regularPrices[0] < product.prodUnitPrice)
			{
				totalDiscount = (1-regularPrices[0] / product.prodUnitPrice) * 100;
				priceRegOff.Text = Math.Round(totalDiscount, MidpointRounding.AwayFromZero).ToString() + "%\noff";
				priceRegOff.IsVisible = true;
			}


			//IF MANAGER EDITING, SHOW EDITING INTERFACE,
			if (Statics.Default.getCreds()[12].Equals("1") && Statics.Default.IsEditing())
			{
				editView.IsVisible = true;
				browseView.IsVisible = false;

				//Init edit values.
				editPic.Source = product.prodImgSrc;
				editDesc.Text = product.prodDescription;
				editBulk.Text = (product.prodBulkDiscount * 100).ToString();
				editUnit.Text = product.prodUnitPrice.ToString("F");
				editIncUnit.Text = product.prodUnitPriceIncentive.ToString();
				editIncFlag.IsToggled = product.prodIncentiveFlag;

				//Select proper bulk type
				foreach (string s in bulkTypes)
				{
					editBulkType.Items.Add(s);
				}
				editBulkType.SelectedIndex = product.prodBulkType;
				editDealFlag.IsToggled = product.prodDealFlag;
				editDiscount.Text = (product.prodDiscount * 100).ToString();

				//Include certain values for editing if not flowers.
				if (!product.prodCategory.Equals("Flowers"))
				{
					regBulk1.IsVisible = true;
					regBulk2.IsVisible = true;
					editBulkLimit.Text = product.prodBulkLimit.ToString();
					editBulkInterval.Text = product.prodBulkInterval.ToString();
					//Enable them if bulktype isn't none.
					if (editBulkType.SelectedIndex > 0)
					{
						regBulk1.IsEnabled = true;
						regBulk2.IsEnabled = true;
					}
				}
			}
			else
			{//IF ACCESSING NORMALLY, SHOW PRODUCT PAGE FOR FLOWERS OR REG.
				if (product.prodCategory.Equals("Flowers"))
				{
					componentExact.IsVisible = true;
				}
				else
				{
					componentRegular.IsVisible = true;
				}
			}



			Debug.WriteLine("editView: " + editView.IsVisible + " browse: " + browseView.IsVisible);
		}



		void increment(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();
			if (regIndex == 19) { return; }
			else
			{
				regIndex++;
				//Recalc discount
				totalDiscount = (1-regularPrices[regIndex] / (product.prodUnitPrice*(regIndex+1)))*100;
				Debug.WriteLine("Price for this amount:" + regularPrices[regIndex]);
				Debug.WriteLine("Amount:" + regIndex+1);
				Debug.WriteLine("Without discounts:" + product.prodUnitPrice*(regIndex+1));
				Debug.WriteLine("Ratio:" + regularPrices[regIndex] / (product.prodUnitPrice * (regIndex + 1)));
				Debug.WriteLine("Discount %:" + totalDiscount);
				Debug.WriteLine("Rounded:" + Math.Round(totalDiscount, MidpointRounding.AwayFromZero));
				if (totalDiscount > 0)
				{
					priceRegOff.Text = Math.Round(totalDiscount, MidpointRounding.AwayFromZero).ToString() + "%\noff";
					priceRegOff.IsVisible = true;
				}
				else
				{
					priceRegOff.IsVisible = false;
				}
				value.Text = (regIndex + 1).ToString();
				price.Text = (regularPrices[regIndex]).ToString("C");
				priceRegRate.Text = "(" + (regularPrices[regIndex] / (regIndex + 1)).ToString("C") + "/item)";
			}
		}



		void decrement(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();
			if (regIndex ==0) { return; }
			else
			{
				regIndex--;
				//Recalc discount
				totalDiscount = (1 - regularPrices[regIndex] / (product.prodUnitPrice * (regIndex + 1))) * 100;
				if (totalDiscount > 0)
				{
					priceRegOff.Text = Math.Round(totalDiscount, MidpointRounding.AwayFromZero).ToString() + "%\noff";
					priceRegOff.IsVisible = true;
				}
				else
				{
					priceRegOff.IsVisible = false;
				}
				value.Text = (regIndex +1).ToString();
				price.Text = (regularPrices[regIndex]).ToString("C");
				priceRegRate.Text = "(" + (regularPrices[regIndex]/(regIndex+1)).ToString("C") + "/item)";
			}
		}



		void previous(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();

			double grams;

			if (index == 0)
			{
				return;
			}
			else
			{
				index--;

				//Recalc discount

				switch (index)
				{
					case 0:
						//1g
						grams = 1;
						totalDiscount = (1 - medicalPrices[index] / (product.prodUnitPrice)) * 100;
						break;
					case 1:
						grams = 3.54688;
						//3.54688g
						totalDiscount = (1 - medicalPrices[index] / (product.prodUnitPrice* grams)) * 100;
						break;
					case 2:
						grams = 3.54688 * 2;
						//3.54688g
						totalDiscount = (1 - medicalPrices[index] / (product.prodUnitPrice * grams)) * 100;
						break;
					case 3:
						grams = 3.54688 * 4;
						//3.54688g
						totalDiscount = (1 - medicalPrices[index] / (product.prodUnitPrice * grams)) * 100;
						break;
					case 4:
						//3.54688g
						grams = 3.54688 * 8;
						totalDiscount = (1 - medicalPrices[index] / (product.prodUnitPrice * grams)) * 100;
						break;
					default:
						grams = 0;
						break;
						
				}
				if (totalDiscount > 0)
				{
					priceExactOff.Text = Math.Round(totalDiscount, MidpointRounding.AwayFromZero).ToString() + "%\noff";
					priceExactOff.IsVisible = true;
				}
				else
				{
					priceExactOff.IsVisible = false;
				}

				if (index == 0) { valueLeft.Text = ""; }
				else { valueLeft.Text = medicalAmounts[index - 1]; }
				valueMid.Text = medicalAmounts[index];
				valueRight.Text = medicalAmounts[index + 1];

				priceExact.Text = medicalPrices[index].ToString("C");
				priceExactRate.Text = "(" + (medicalPrices[index] / grams).ToString("C") + "/g)";
			}
		}



		void next(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();

			double grams;

			if (index == 4)
			{
				return;
			}
			else
			{
				index++;
				//Recalc discount
				switch (index)
				{
					case 0:
						//1g
						grams = 1;
						totalDiscount = (1 - medicalPrices[index] / (product.prodUnitPrice)) * 100;
						break;
					case 1:
						//3.54688g
						grams = 3.54688;
						totalDiscount = (1 - medicalPrices[index] / (product.prodUnitPrice * grams)) * 100;
						break;
					case 2:
						grams = 3.54688 * 2;
						totalDiscount = (1 - medicalPrices[index] / (product.prodUnitPrice * grams)) * 100;
						break;
					case 3:
						grams = 3.54688 *4;
						totalDiscount = (1 - medicalPrices[index] / (product.prodUnitPrice * grams)) * 100;
						break;
					case 4:
						grams = 3.54688 * 8;
						totalDiscount = (1 - medicalPrices[index] / (product.prodUnitPrice * grams)) * 100;
						break;
					default:
						grams = 0;
						break;

				}
				if (totalDiscount > 0)
				{
					priceExactOff.Text = Math.Round(totalDiscount, MidpointRounding.AwayFromZero).ToString() + "%\noff";
					priceExactOff.IsVisible = true;
				}
				else
				{
					priceExactOff.IsVisible = false;
				}

				if (index == 4) { valueRight.Text = ""; }
				else { valueRight.Text = medicalAmounts[index + 1]; }
				valueMid.Text = medicalAmounts[index];
				valueLeft.Text = medicalAmounts[index - 1];

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

				ImageSource src = ImageSource.FromStream(() => new MemoryStream(imageBytes));
				editPic.Source = src;

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

					byte[] uncompressed = memoryStream.ToArray();
					byte[] compressed = DependencyService.Get<IFileProcessing>().compress(uncompressed, "png");
					if (compressed != null)
					{
						return compressed;
					}
					else
					{
						Debug.WriteLine("Compression did not work properly.");
						return uncompressed;
					}
					//return uncompressed;
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
			//Check for suspension, if transactions suspended, disallow new additions and disallow checkout.
			if (Statics.Default.getCreds()[17] != "0")
			{
				await DisplayAlert("Transactions suspended", "Transactions for all users have been suspended temporarily by the dispensary owner. Try again later. (You will have to log out and back in)", "Okay");
				return;
			}

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
						rate = "(" + (medicalPrices[1] / 3.54688).ToString("C") + "/g)";
						break;
					case 2:
						amount = (double)1 / 4;
						unit = "oz";
						rate = "(" + (medicalPrices[2] / (3.54688 * 2)).ToString("C") + "/g)";
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
				//Medicalprices takes the amount, discount and bulkdiscount into account already.
				cartItem = new CartListItem { prodName = product.prodName, prodIsRegular = false, prodIsFlower = true, prodAmount = amount, prodRate = rate, prodUnitType = unit, prodTotal = (medicalPrices[index]).ToString("C") };
			}
			else
			{
				string rate = "(" + (regularPrices[regIndex] / (regIndex + 1)).ToString("C") + "/item)";
				//Regular prices does now too.
				cartItem = new CartListItem { prodName = product.prodName, prodIsRegular = true, prodIsFlower = false, prodAmount = regIndex+1, prodRate = rate, prodUnitType = "", prodTotal = (regularPrices[regIndex]).ToString("C") };
			}
			//Update static cart.
			ObservableCollection<CartListItem> pulled = Statics.Default.getCartItems();
			pulled.Add(cartItem);
			Statics.Default.setCartItems(pulled);

			var answer = await DisplayAlert("Added to Cart", "Would you like to keep browsing?", "Keep Browsing", "View Cart");

			if (answer)
			{
				await Navigation.PopModalAsync();
			}
			else
			{
				NavigationPage nav = new NavigationPage(new CartTab { Title = "Your Cart" });
				NavigationPage.SetHasBackButton(nav, true);
				await Navigation.PushModalAsync(nav);
			}
		}




		async void saveEdits(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();

			var answer = await DisplayAlert("Save Changes", "Review your changes for accuracy. Are you sure you want to make these changes?", "Yes", "Review");

			string error = "";

			/*Checklist*/

			//Fails if any info is empty.
			if (String.IsNullOrEmpty(editDesc.Text) || editDesc.Text.Equals("Enter your description.") ||
			     String.IsNullOrEmpty(editUnit.Text) ||
			     String.IsNullOrEmpty(editDiscount.Text) ||
			    (String.IsNullOrEmpty(editBulk.Text) && editBulkType.SelectedIndex>0) ||
			    (String.IsNullOrEmpty(editDiscount.Text) && editDealFlag.IsToggled) ||
			    (String.IsNullOrEmpty(editIncUnit.Text) && editIncFlag.IsToggled) ||
			    editBulkType.SelectedIndex < 0 ||
			    (String.IsNullOrEmpty(editBulkLimit.Text) && !product.prodCategory.Equals("Flowers")) ||
			    (String.IsNullOrEmpty(editBulkInterval.Text) && !product.prodCategory.Equals("Flowers")))
			{
				error = "No fields can be empty.";
				//Display error
				await DisplayAlert("Error", error, "Okay");
				return;
			}

			//Fails if either discount is applied but set to less than 5%.
			if ((int.Parse(editDiscount.Text) < 5 && editDealFlag.IsToggled) || 
			    (int.Parse(editBulk.Text) < 5 && editBulkType.SelectedIndex > 0))
			{
				error = "Discount cannot be less than 5%, increase discount % or disable the discount(Set 'Bulk discount' to none or disable 'Discount applied'";
				//Display error
				await DisplayAlert("Error", error, "Okay");
				return;
			}

			//Possibly create safeguards for someone trying to set an item to be free.

			if (answer)
			{
				//Upload changes, notify manager that they can browse products normally to see exactly how these changes look to the users.

				//Convert inputs
				string deal, inc;
				if (editDealFlag.IsToggled) { deal = "1"; } else { deal = "0"; }
				if (editIncFlag.IsToggled) { inc = "1"; } else { inc = "0"; }
				double discount = double.Parse(editDiscount.Text) / 100;
				double bulkDiscount = double.Parse(editBulk.Text) / 100;

				//Connect to url.
				var client = new HttpClient();

				var contentSent = new MultipartFormDataContent();
				contentSent.Add(new StringContent(Statics.Default.getCreds()[16]), "dispId");
				contentSent.Add(new StringContent("1"), "operation");
				contentSent.Add(new StringContent(product.prodName), "name");
				contentSent.Add(new StringContent(editDesc.Text), "desc");
				if (imageBytes != null)
				{
					contentSent.Add(new ByteArrayContent(imageBytes), "picBytes");
				}
				contentSent.Add(new StringContent(editUnit.Text), "unitPrice");
				contentSent.Add(new StringContent(inc), "incFlag");
				contentSent.Add(new StringContent(editIncUnit.Text), "incUnitPrice");
				contentSent.Add(new StringContent(deal), "dealFlag");
				contentSent.Add(new StringContent(discount.ToString()), "discount");
				contentSent.Add(new StringContent(editBulkType.Items[editBulkType.SelectedIndex]), "bulkType");
				contentSent.Add(new StringContent(bulkDiscount.ToString()), "bulkDiscount");
				if (!product.prodCategory.Equals("Flowers") && editBulkType.SelectedIndex > 0)
				{
					contentSent.Add(new StringContent(editBulkLimit.Text), "bulkLimit");
					contentSent.Add(new StringContent(editBulkInterval.Text), "bulkInterval");
				}

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
					ObservableCollection<ProductListItem> pulledCat = Statics.Default.getCatClickedContents();
					int place = pulled.IndexOf(product);
					pulled.Remove(product);
					int placeCat = pulledCat.IndexOf(product);
					pulledCat.Remove(product);
					product.prodDescription = editDesc.Text;
					product.prodImgSrc = editPic.Source;
					product.prodUnitPrice = double.Parse(editUnit.Text);
					product.prodIncentiveFlag = editIncFlag.IsToggled;
					product.prodUnitPriceIncentive = double.Parse(editIncUnit.Text);
					product.prodDealFlag = editDealFlag.IsToggled;
					product.prodDiscount = discount;
					product.prodBulkType = editBulkType.SelectedIndex;
					product.prodBulkDiscount = bulkDiscount;
					if (!product.prodCategory.Equals("Flowers") && editBulkType.SelectedIndex > 0)
					{
						product.prodBulkLimit = int.Parse(editBulkLimit.Text);
						product.prodBulkInterval = int.Parse(editBulkInterval.Text);
					}

					pulled.Insert(place, product);
					pulledCat.Insert(placeCat, product);
					Statics.Default.setProducts(pulled);
					Statics.Default.setCatClickedContents(pulledCat);

					await Navigation.PopModalAsync();
				}
				else
				{
					await DisplayAlert("Error", "Sorry, there was a problem uploading your changes.", "Okay");

				}
			}
		}

		async void gatherItemImage()
		{
			var client = new HttpClient();

			ObservableCollection<ProductListItem> pulled = Statics.Default.getProducts();
			int place;

			//Get place for update
			place = pulled.IndexOf(product);

			var contentSent = new MultipartFormDataContent();
			contentSent.Add(new StringContent(product.prodName), "name");
			contentSent.Add(new StringContent(Statics.Default.getCreds()[16]), "dispId");

			Debug.WriteLine("Getting pic for " + product.prodName + " with dispId " + Statics.Default.getCreds()[16]);

			var response = await client.PostAsync("http://cbd-online.net/landon/downloadPictures.php", contentSent);

			byte[] output = await response.Content.ReadAsByteArrayAsync();

			if (output.Length < 1)
			{
				Debug.WriteLine("No data for " + product.prodName + " gathered.");
			}
			else
			{
				Debug.WriteLine("Printing byte[] length for " + product.prodName + ":");
				Debug.WriteLine(output.Length);
			}

			//Update picture
			ImageSource src = ImageSource.FromStream(() => new MemoryStream(output));
			product.prodImgSrc = src;
			editPic.Source = src;
			image.Source = src;

			//If picture was not already loaded upon reaching page, and is now being loaded, commit it to memory.
			if (product.prodImgSrc != ogSrc)
			{
				pulled.RemoveAt(place);
				pulled.Insert(place, product);
			}
			//Update static to save pictures for future use.
			Statics.Default.setProducts(pulled);
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

			return "unknown";
		}

		//Couldn't bind picker to 'isenabled' of other controls like switches without some sort of converter.
		void bulkChanged(object s, EventArgs e)
		{
			if (editBulkType.SelectedIndex > 0)
			{
				editBulk.IsEnabled = true;
				editBulkLimit.IsEnabled = true;
				editBulkInterval.IsEnabled = true;
			}
			else
			{
				editBulk.IsEnabled = false;
				editBulkLimit.IsEnabled = false;
				editBulkInterval.IsEnabled = false;
			}
		}

		void cancelNew(object s, EventArgs e)
		{
			Navigation.PopModalAsync();
		}

		void cancelEdits(object s, EventArgs e)
		{
			//Abort changes.
			Navigation.PopModalAsync();
		}
	}
}
