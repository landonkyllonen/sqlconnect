using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SQLConnect
{
	public partial class AddConditionPage : ContentPage
	{
		public AddConditionPage()
		{
			InitializeComponent();
		}

		public async void addCondition(object s, EventArgs e)
		{
			//Check if condition is empty or already existing.
			if (String.IsNullOrEmpty(condNameEntry.Text) || Statics.Default.getCreds()[9].Contains(condNameEntry.Text))
			{
				console.Text = "Condition cannot be blank or already added.";
				return;
			}
			else {
				//Upload to db.
				//Connect to url.
				var client = new System.Net.Http.HttpClient();

				//Show that we are waiting for a response and wait for it.

				var response = await client.GetAsync("http://cbd-online.net/landon/addCondition.php?" +
													 "user=" + System.Net.WebUtility.UrlEncode(Statics.Default.getUser()) +
													 "&name=" + System.Net.WebUtility.UrlEncode(condNameEntry.Text));

				var output = await response.Content.ReadAsStringAsync();
				//If successful, add the condition to current conditions.

				if (output.Equals("true"))
				{
					//Feedback.
				}

				ObservableCollection<CondListItem> condsListPulled = Statics.Default.getConds();
				string[] creds = Statics.Default.getCreds();

				CondListItem newCond = new CondListItem { condName = condNameEntry.Text };
				string appendCond = "--" + newCond;

				condsListPulled.Add(newCond);
				creds[9] = creds[9] + appendCond;

				Statics.Default.setConds(condsListPulled);
				Statics.Default.setCreds(creds);

				await Navigation.PopModalAsync();
			}
		}
	}
}
