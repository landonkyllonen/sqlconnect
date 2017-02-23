using System.Collections.ObjectModel;
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
				dispItems.Add(new DispListItem { dispName = "Example Dispensary", dispAddress = "6436 Boulder Drive", dispCity = "Kelowna, BC", dispImgPath = "jarspic.jpg" });
				dispItems.Add(new DispListItem { dispName = "Example Dispensary 2", dispAddress = "5326 Rock Drive", dispCity = "Vancouver, BC", dispImgPath = "jarspic.jpg" });
				dispItems.Add(new DispListItem { dispName = "Example Dispensary 3", dispAddress = "5431 Stone Drive", dispCity = "Vernon, BC", dispImgPath = "jarspic.jpg" });
				dispItems.Add(new DispListItem { dispName = "Example Dispensary 4", dispAddress = "6868 Pebble Drive", dispCity = "Vernon, BC", dispImgPath = "jarspic.jpg" });
				dispItems.Add(new DispListItem { dispName = "Example Dispensary 5", dispAddress = "5336 Flint Drive", dispCity = "Vernon, BC", dispImgPath = "jarspic.jpg" });
			}
			else {
				//Import dispensaries
				credentials = Statics.Default.getCreds();
			}

			//if not first time login, make top message invisible and label as choose your dispensary
			if (!credentials[16].Equals(""))
			{
				firstTimeLbls.SetValue(IsVisibleProperty, false);
			}

			dispList.ItemsSource = dispItems;
			dispList.ItemTapped += onDispSelect;
		}

		public async void onDispSelect(object s, ItemTappedEventArgs e)
		{
			string newDispensary = ((DispListItem)e.Item).dispName;
			credentials[16] = newDispensary;
			Statics.Default.setCreds(credentials);

			//Connect to url.
			/*var client = new System.Net.Http.HttpClient();

			//Show that we are waiting for a response and wait for it.

			var response = await client.GetAsync("http://cbd-online.net/landon/changeDispensary.php?" +
			                                     "user=" + System.Net.WebUtility.UrlEncode(Statics.Default.getUser()) +
			                                     "&disp=" + System.Net.WebUtility.UrlEncode(newDispensary));*/

			await Navigation.PushModalAsync(new MasterPage());
		}

		void Handle_TextChanged(object sender, TextChangedEventArgs e)
		{
			dispList.BeginRefresh();

			if (string.IsNullOrWhiteSpace(e.NewTextValue))
				dispList.ItemsSource = dispItems;
			else {
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
}
