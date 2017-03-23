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
			condNameEntry.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeSentence);
		}

		public async void addCondition(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();
			//Check if condition is empty or already existing.
			if (string.IsNullOrEmpty(condNameEntry.Text))
			{
				console.Text = "Condition cannot be blank.";
				return;
			}
			else if (Statics.Default.getConds().Contains(new SimpleListItem { labelName=condNameEntry.Text}))
			{
				console.Text = "Condition has already been added.";
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

				ObservableCollection<SimpleListItem> condsListPulled = Statics.Default.getConds();
				string[] creds = Statics.Default.getCreds();

				SimpleListItem newCond = new SimpleListItem { labelName = condNameEntry.Text };
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
