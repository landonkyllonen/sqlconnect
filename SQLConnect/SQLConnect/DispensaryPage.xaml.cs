using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net.Http;

namespace SQLConnect
{
	public partial class DispensaryPage : ContentPage
	{
		string[] credentials;
		ObservableCollection<DispListItem> dispItems, dispItemsFiltered;

		public DispensaryPage()
		{
			InitializeComponent();

			dispItems = new ObservableCollection<DispListItem>();
			dispItemsFiltered = new ObservableCollection<DispListItem>();

			credentials = Statics.Default.getCreds();

			if (Statics.Default.isOffline())
			{
				dispItems.Add(new DispListItem { dispName = "Example Dispensary", dispId = "1", dispAddress = "6436 Boulder Drive", dispCity = "Kelowna, BC", dispImgPath = "jarspic.jpg" });
				dispItems.Add(new DispListItem { dispName = "Example Dispensary 2", dispId = "1", dispAddress = "5326 Rock Drive", dispCity = "Vancouver, BC", dispImgPath = "jarspic.jpg" });
				dispItems.Add(new DispListItem { dispName = "Example Dispensary 3", dispId = "1", dispAddress = "5431 Stone Drive", dispCity = "Vernon, BC", dispImgPath = "jarspic.jpg" });
				dispItems.Add(new DispListItem { dispName = "Example Dispensary 4", dispId = "1", dispAddress = "6868 Pebble Drive", dispCity = "Vernon, BC", dispImgPath = "jarspic.jpg" });
				dispItems.Add(new DispListItem { dispName = "Example Dispensary 5", dispId = "1", dispAddress = "5336 Flint Drive", dispCity = "Vernon, BC", dispImgPath = "jarspic.jpg" });
			}
			else {
				//Import dispensaries
				dispItems = Statics.Default.getDispensaries();
				foreach (DispListItem d in dispItems)
				{
					Debug.WriteLine(d.dispImgPath);
				}
			}

			//if not first time login, make top message invisible and label as choose your dispensary
			if (!credentials[16].Equals(""))
			{
				firstTimeLbls.SetValue(IsVisibleProperty, false);
			}

			dispList.ItemsSource = dispItems;
			dispList.ItemTapped += onDispSelect;

			dispSearch.SearchButtonPressed += (sender, e) => dispSearch.Unfocus();
		}

		public async void onDispSelect(object s, ItemTappedEventArgs e)
		{
			if (!Statics.Default.isOffline())
			{
				string newDispensary = ((DispListItem)e.Item).dispId;
				credentials[16] = newDispensary;
				Statics.Default.setCreds(credentials);

				//Connect to url.
				var client = new HttpClient();

				var content = new MultipartFormDataContent();
				content.Add(new StringContent(Statics.Default.getUser()), "user");
				content.Add(new StringContent(newDispensary), "disp");

				await client.PostAsync("http://cbd-online.net/landon/changeDispensary.php", content);

				//Reload products again.
				await asyncLoadProducts(newDispensary);

				//Save logoUrl to phone for use on next login.
				Application.Current.Properties["dispLogo"] = ((DispListItem)e.Item).dispLogoPath;

				await Navigation.PushModalAsync(new MasterPage());
			}
		}


		void Handle_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!Statics.Default.isOffline())
			{

				dispList.BeginRefresh();

				if (string.IsNullOrWhiteSpace(e.NewTextValue))
				{
					dispList.ItemsSource = dispItems;
				}else {
					dispItemsFiltered.Clear();
					foreach (DispListItem item in dispItems)
					{
						if (item.dispCity.ToLower().Contains(e.NewTextValue.ToLower()))
						{
							dispItemsFiltered.Add(item);
						}
					}
					dispList.ItemsSource = dispItemsFiltered;
				}

				dispList.EndRefresh();
			}
		}

		async Task asyncLoadProducts(string dispId)
		{
			Debug.WriteLine(dispId);

			ObservableCollection<ProductListItem> prods;

			//Connect to url.
			var client = new HttpClient();

			var content = new MultipartFormDataContent();
			content.Add(new StringContent(dispId), "dispId");

			var response = await client.PostAsync("http://cbd-online.net/landon/getItemInfo.php", content);

			var output = await response.Content.ReadAsStringAsync();

			//Process the output.
			string[] productObjects = output.Split(new string[] { ";;" }, StringSplitOptions.RemoveEmptyEntries);

			//Separate into product components and turn into product objects.
			prods = new ObservableCollection<ProductListItem>();
			foreach (string obj in productObjects)
			{
				string[] productComponents = obj.Split(new string[] { "--" }, StringSplitOptions.RemoveEmptyEntries);

				Debug.WriteLine(obj);
				//FOR DEBUGGING--
				string comps = "";
				foreach (string s in productComponents)
				{
					comps = comps + " " + s;
				}
				Debug.WriteLine(comps);
				//FOR DEBUGGING--

				//  0    1   2       3          4       5            6                7                8            9           10                11
				//Name, Id, Cat, Description, PicUrl, BasePrice, IncentiveFlag, IncentiveBasePrice, DealFlag, DealDiscount, BulkDiscountType, BulkDiscount
				bool deal = false;
				bool incentive = false;
				if (productComponents[8].Equals("1")) { deal = true; }
				if (productComponents[6].Equals("1")) { incentive = true; }

				var prod = new ProductListItem
				{
					prodName = productComponents[0],
					prodCategory = productComponents[2],
					prodDescription = productComponents[3],
					prodImgUrl = productComponents[4],
					prodUnitPrice = double.Parse(productComponents[5]),
					prodUnitPriceIncentive = double.Parse(productComponents[7]),
					prodDiscount = double.Parse(productComponents[9]),
					prodDealFlag = deal,
					prodIncentiveFlag = incentive,
					prodBulkDiscount = double.Parse(productComponents[11]),
					prodBulkType = int.Parse(productComponents[10])
				};

				if (prod.prodDealFlag) { Statics.Default.setDeal(prod); }

				//bound as name--category--description--imageurl--incrementtype--baseprice--incbaseprice--dealdiscount--dealflag-incflag--bulkdis--bulkdistype;;
				prods.Add(prod);
			}

			//products now contains all the products loaded from a certain dispensary, save to static for use in deal on front page,
			//as well as for lists.

			Statics.Default.setProducts(prods);
		}
	}
}
