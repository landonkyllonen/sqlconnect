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

			credentials = Statics.Default.getCreds();

			//if not first time login, make top message invisible and label as choose your dispensary
			if (!credentials[16].Equals(""))
			{
				firstTimeLbls.SetValue(IsVisibleProperty, false);
			}

			dispItems = new ObservableCollection<DispListItem>();
			dispItems.Add(new DispListItem {dispName="Example Dispensary", dispAddress="6436 Boulder Drive", dispCity="Kelowna, BC", dispImgPath="jarspic.jpg"});
			dispItems.Add(new DispListItem {dispName= "Example Dispensary 2", dispAddress = "5326 Rock Drive", dispCity = "Vancouver, BC", dispImgPath = "jarspic.jpg"});
			dispItems.Add(new DispListItem {dispName = "Example Dispensary 3", dispAddress = "5431 Stone Drive", dispCity = "Vernon, BC", dispImgPath = "jarspic.jpg" });
			dispItems.Add(new DispListItem {dispName = "Example Dispensary 4", dispAddress = "6868 Pebble Drive", dispCity = "Vernon, BC", dispImgPath = "jarspic.jpg" });
			dispItems.Add(new DispListItem {dispName = "Example Dispensary 5", dispAddress = "5336 Flint Drive", dispCity = "Vernon, BC", dispImgPath = "jarspic.jpg" });
			dispItemsFiltered = new ObservableCollection<DispListItem>();

			dispList.ItemsSource = dispItems;
			dispList.ItemTapped += onDispSelect;
		}

		public async void onDispSelect(object s, ItemTappedEventArgs args)
		{
			//Do something.
			credentials[16] = "ExampleDisp";
			Statics.Default.setCreds(credentials);
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
