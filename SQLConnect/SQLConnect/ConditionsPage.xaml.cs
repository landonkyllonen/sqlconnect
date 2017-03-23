using System.Collections.ObjectModel;
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
				var client = new System.Net.Http.HttpClient();

				//Show that we are waiting for a response and wait for it.

				var response = await client.GetAsync("http://cbd-online.net/landon/removeDetail.php?" +
													 "user=" + System.Net.WebUtility.UrlEncode(Statics.Default.getUser()) +
														 "&type=" + System.Net.WebUtility.UrlEncode("Conditions") +
				                                     "&itemname=" + System.Net.WebUtility.UrlEncode(((SimpleListItem)e.Item).labelName));

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
