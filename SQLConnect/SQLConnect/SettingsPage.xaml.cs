using Xamarin.Forms;
using System.Net;
using System.Collections.ObjectModel;

namespace SQLConnect
{
	public partial class SettingsPage : ContentPage
	{
		int appear, block;
		ObservableCollection<SimpleListItem> blacklist;

		public SettingsPage()
		{
			InitializeComponent();

			appear = Statics.Default.getAppearInSearch();
			block = Statics.Default.getBlockNonContacts();
			blacklist = Statics.Default.getBlacklist();

			if (appear == 1) { hideSwitch.IsToggled = true;}
			if (block == 1) { blockSwitch.IsToggled = true;}
		}

		async void hideWarning(object s, ToggledEventArgs e)
		{
			s.ToString();
			e.ToString();
			if (hideSwitch.IsToggled)
			{
				appear = 0;
				var answer = await DisplayAlert("Are you sure?", "Turning this on means that your username will appear to users looking for others with " +
									  "similar conditions/medications, and also that they may try to send you messages.", "Okay", "Nevermind");


				if (answer)
				{
					//Upload changes.
					//Connect to url.
					var client = new System.Net.Http.HttpClient();

					//Show that we are waiting for a response and wait for it.

					var response = await client.GetAsync("http://cbd-online.net/landon/changePref.php?" +
														 "user=" + WebUtility.UrlEncode(Statics.Default.getUser()) +
														 "&pref=" + 0 + "&state=" + appear);

					await response.Content.ReadAsStringAsync();
				}
				else {
					hideSwitch.IsToggled = false;
				}
			}
			else {
				appear = 1;
				//Upload changes.
				//Connect to url.
				var client = new System.Net.Http.HttpClient();

				//Show that we are waiting for a response and wait for it.

				var response = await client.GetAsync("http://cbd-online.net/landon/changePref.php?" +
													 "user=" + WebUtility.UrlEncode(Statics.Default.getUser()) +
													 "&pref=" + 0 + "&state=" + appear);

				await response.Content.ReadAsStringAsync();
			}
		}

		async void blockWarning(object s, ToggledEventArgs e)
		{
			s.ToString();
			e.ToString();
			if (blockSwitch.IsToggled)
			{
				block = 0;
				var answer = await DisplayAlert("Are you sure?", "Turning this on means that users will be told that they must be your contact to send you a message.", "Okay", "Nevermind");

				if (answer)
				{
					//Upload changes.
					//Connect to url.
					var client = new System.Net.Http.HttpClient();

					//Show that we are waiting for a response and wait for it.

					var response = await client.GetAsync("http://cbd-online.net/landon/changePref.php?" +
														 "user=" + WebUtility.UrlEncode(Statics.Default.getUser()) +
														 "&pref=" + 1 + "&state=" + block);

					await response.Content.ReadAsStringAsync();
				}
				else {
					blockSwitch.IsToggled = false;
				}
			}
			else {
				block = 1;
				//Upload changes.
				//Connect to url.
			var client = new System.Net.Http.HttpClient();

				//Show that we are waiting for a response and wait for it.

				var response = await client.GetAsync("http://cbd-online.net/landon/changePref.php?" +
													 "user=" + WebUtility.UrlEncode(Statics.Default.getUser()) +
													 "&pref=" + 1 + "&state=" + block);

				await response.Content.ReadAsStringAsync();
			}
		}
	}
}
