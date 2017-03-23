using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

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
				var client = new System.Net.Http.HttpClient();

				//Show that we are waiting for a response and wait for it.

				await client.GetAsync("http://cbd-online.net/landon/changeDispensary.php?" +
													 "user=" + System.Net.WebUtility.UrlEncode(Statics.Default.getUser()) +
													 "&disp=" + System.Net.WebUtility.UrlEncode(newDispensary));

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
			var client = new System.Net.Http.HttpClient();

			//Show that we are waiting for a response and wait for it.

			var response = await client.GetAsync("http://cbd-online.net/landon/getItemInfo.php?" +
			                                     "dispId=" + System.Net.WebUtility.UrlEncode(dispId));

			var output = await response.Content.ReadAsStringAsync();

			//Process the output.
			string[] productObjects = output.Split(new string[] { ";;" }, StringSplitOptions.None);

			//Separate into product components and turn into product objects.
			prods = new ObservableCollection<ProductListItem>();
			foreach (string obj in productObjects)
			{
				string[] productComponents = obj.Split(new string[] { "--" }, StringSplitOptions.None);

				Debug.WriteLine(obj);
				//FOR DEBUGGING--
				string comps = "";
				foreach (string s in productComponents)
				{
					comps = comps + " " + s;
				}
				Debug.WriteLine(comps);
				//FOR DEBUGGING--

				bool deal = false;
				bool incentive = false;
				if (productComponents[8].Equals("1")) { deal = true; }
				if (productComponents[9].Equals("1")) { incentive = true; }

				//bound as name--category--description--imageurl--incrementtype--baseprice--incbaseprice--dealdiscount--dealflag-incflag--bulkdis--bulkdistype;;
				prods.Add(new ProductListItem
				{
					prodName = productComponents[0],
					prodCategory = productComponents[1],
					prodDescription = productComponents[2],
					prodImgUrl = productComponents[3],
					prodIncrementType = productComponents[4],
					prodUnitPrice = double.Parse(productComponents[5]),
					prodUnitPriceIncentive = double.Parse(productComponents[6]),
					prodDiscount = double.Parse(productComponents[7]),
					prodDealFlag = deal,
					prodIncentiveFlag = incentive,
					prodBulkDiscount = double.Parse(productComponents[10]),
					prodBulkType = int.Parse(productComponents[11])
				});
			}

			//products now contains all the products loaded from a certain dispensary, save to static for use in deal on front page,
			//as well as for lists.

			Statics.Default.setProducts(prods);
		}
	}
}
