using System.Collections.ObjectModel;
using System.Net.Http;
using Xamarin.Forms;

namespace SQLConnect
{
	public partial class MedicationsPage : ContentPage
	{
		public MedicationsPage()
		{
			InitializeComponent();

			ObservableCollection<MedListItem> meds = Statics.Default.getMeds();

			//meds = new ObservableCollection<MedListItem>();
			medList.ItemsSource = meds;
			medList.ItemTapped += onItemSelect;
		}

		async void onItemSelect(object sender, ItemTappedEventArgs e)
		{
			//Display confirmation dialog.
			var answer = await DisplayAlert("Delete?", "Do you really want to delete this item?", "Yes", "No");

			if (answer)
			{
				//Delete med from db.
				//Connect to url.
				var client = new HttpClient();

				var content = new MultipartFormDataContent();
				content.Add(new StringContent(Statics.Default.getUser()), "user");
				content.Add(new StringContent("Conditions"), "type");
				content.Add(new StringContent(((SimpleListItem)e.Item).labelName), "itemname");

				var response = await client.PostAsync("http://cbd-online.net/landon/removeDetail.php", content);

				await response.Content.ReadAsStringAsync();

				//and locally.
				ObservableCollection<MedListItem> pulled = Statics.Default.getMeds();
				foreach (MedListItem med in pulled)
				{
					if (med.medName.Equals(((MedListItem)e.Item).medName))
					{
						pulled.Remove(med);
						break;
					}
				}

				//Reflect changes to static variable.
				Statics.Default.setMeds(pulled);
				medList.ItemsSource = pulled;
			}
			return;
		}

		void goToAddMedication(object sender, System.EventArgs e)
		{
			Navigation.PushModalAsync(new AddMedicationPage());
		}
	}
}
