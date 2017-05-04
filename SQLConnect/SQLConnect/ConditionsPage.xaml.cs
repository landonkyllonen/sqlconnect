using System.Collections.ObjectModel;
using System.Net.Http;
using Xamarin.Forms;

namespace SQLConnect
{
	public partial class ConditionsPage : ContentPage
	{
		public ConditionsPage()
		{
			InitializeComponent();

			ObservableCollection<SimpleListItem> conds = Statics.Default.getConds();

			//conds = new ObservableCollection<CondListItem>();
			condList.ItemsSource = conds;
			condList.ItemTapped += onItemSelect;
		}

		async void onItemSelect(object sender, ItemTappedEventArgs e)
		{
			//Display confirmation dialog.
			var answer = await DisplayAlert("Delete?", "Do you really want to delete this item?", "Yes", "No");

			if (answer)
			{
				//Delete cond from db.
				//Connect to url.
				var client = new HttpClient();

				var content = new MultipartFormDataContent();
				content.Add(new StringContent(Statics.Default.getUser()), "user");
				content.Add(new StringContent("Conditions"), "type");
				content.Add(new StringContent(((SimpleListItem)e.Item).labelName), "itemname");

				var response = await client.PostAsync("http://cbd-online.net/landon/removeDetail.php", content);

				await response.Content.ReadAsStringAsync();

				//and locally.
				ObservableCollection<SimpleListItem> pulled = Statics.Default.getConds();
				foreach (SimpleListItem cond in pulled)
				{
					if (cond.labelName.Equals(((SimpleListItem)e.Item).labelName))
					{
						pulled.Remove(cond);
						break;
					}
				}

				//Reflect changes to static variable.
				Statics.Default.setConds(pulled);

			}
			return;
		}

		void goToAddCondition(object sender, System.EventArgs e)
		{
			Navigation.PushModalAsync(new AddConditionPage());
		}
	}
}
