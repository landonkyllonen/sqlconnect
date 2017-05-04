using Xamarin.Forms;
using System.Net.Http;
using System.Collections.ObjectModel;
using System;

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
				var answer = await DisplayAlert("Are you sure?", "If this is ON, your name can appear if a user performs a search for other users with similar conditions/medications. These users may try to message you.", "Okay", "Nevermind");


				if (answer)
				{
					//Upload changes.
					//Connect to url.
					var client = new HttpClient();

					var content = new MultipartFormDataContent();
					content.Add(new StringContent(Statics.Default.getUser()), "user");
					content.Add(new StringContent("0"), "pref");
					content.Add(new StringContent(appear.ToString()), "state");

					var response = await client.PostAsync("http://cbd-online.net/landon/changePref.php", content);

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
				var client = new HttpClient();

				var content = new MultipartFormDataContent();
				content.Add(new StringContent(Statics.Default.getUser()), "user");
				content.Add(new StringContent("0"), "pref");
				content.Add(new StringContent(appear.ToString()), "state");

				var response = await client.PostAsync("http://cbd-online.net/landon/changePref.php", content);

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
				var answer = await DisplayAlert("Are you sure?", "Turning this on will disallow non-contacts from sending messages to you.", "Okay", "Nevermind");

				if (answer)
				{
					//Upload changes.
					//Connect to url.
					var client = new HttpClient();

					var content = new MultipartFormDataContent();
					content.Add(new StringContent(Statics.Default.getUser()), "user");
					content.Add(new StringContent("1"), "pref");
					content.Add(new StringContent(block.ToString()), "state");

					var response = await client.PostAsync("http://cbd-online.net/landon/changePref.php", content);

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
			var client = new HttpClient();

				var content = new MultipartFormDataContent();
				content.Add(new StringContent(Statics.Default.getUser()), "user");
				content.Add(new StringContent("1"), "pref");
				content.Add(new StringContent(block.ToString()), "state");

				var response = await client.PostAsync("http://cbd-online.net/landon/changePref.php", content);

				await response.Content.ReadAsStringAsync();
			}
		}

		async void unlink(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();
			string id = DependencyService.Get<IDeviceId>().getDeviceId();
			var answer = await DisplayAlert("Are you sure?", "Unlinking this device will unregister it, allowing 1 new device to login using your credentials.", "Okay", "Nevermind");

			if (answer)
			{
				//Upload changes.
				//Connect to url.
				var client = new HttpClient();

				var content = new MultipartFormDataContent();
				content.Add(new StringContent(id), "id");
				content.Add(new StringContent(Statics.Default.getUser()), "username");

				//Show that we are waiting for a response and wait for it.

				var response = await client.PostAsync("http://cbd-online.net/landon/unlink.php", content);

				await response.Content.ReadAsStringAsync();
			}
		}
	}
}
