using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SQLConnect
{
	public partial class BlacklistPage : ContentPage
	{
		ObservableCollection<SimpleListItem> blacklist;
		public BlacklistPage()
		{
			InitializeComponent();

			blacklist = Statics.Default.getBlacklist();

			blackList.ItemsSource = blacklist;
			blackList.ItemTapped += onItemSelect;
		}

		async void onItemSelect(object sender, ItemTappedEventArgs e)
		{
			//Display confirmation dialog.
			var answer = await DisplayAlert("Delete?", "Do you really want to remove this user?", "Yes", "No");

			if (answer)
			{
				//Delete cond from db.
				//Connect to url.
				var client = new System.Net.Http.HttpClient();

				//Show that we are waiting for a response and wait for it.

				var response = await client.GetAsync("http://cbd-online.net/landon/changeUserList.php?" +
													 "user=" + System.Net.WebUtility.UrlEncode(Statics.Default.getUser()) +
													 "&type=" + System.Net.WebUtility.UrlEncode("Blacklist") +
													 "&itemname=" + System.Net.WebUtility.UrlEncode(((SimpleListItem)e.Item).labelName) +
													 "&action=" + System.Net.WebUtility.UrlEncode("Remove"));

				var output = await response.Content.ReadAsStringAsync();

				//and locally.
				ObservableCollection<SimpleListItem> pulled = Statics.Default.getBlacklist();
				foreach (SimpleListItem user in pulled)
				{
					if (user.labelName.Equals(((SimpleListItem)e.Item).labelName))
					{
						pulled.Remove(user);
						break;
					}
				}

				//Reflect changes to static variable.
				Statics.Default.setBlacklist(pulled);

			}
			return;
		}

		async void addToBlacklist(object sender, System.EventArgs e)
		{
			//Display confirmation dialog.
			var answer = await DisplayAlert("Blacklist?", "Do you want to reject future messages from this user?", "Yes", "No");

			if (answer)
			{

				console.Text = "";
				//add contact listed in entry.
				string name = nameEntry.Text;
				if (String.IsNullOrEmpty(name))
				{
					console.Text = "You must enter a name.";
					return;
				}

				//db
				//Connect to url.
				var client = new System.Net.Http.HttpClient();

				//Show that we are waiting for a response and wait for it.

				var response = await client.GetAsync("http://cbd-online.net/landon/changeUserList.php?" +
													 "user=" + System.Net.WebUtility.UrlEncode(Statics.Default.getUser()) +
													 "&type=" + System.Net.WebUtility.UrlEncode("Blacklist") +
													 "&itemname=" + System.Net.WebUtility.UrlEncode(name) +
														"&action=" + System.Net.WebUtility.UrlEncode("Add"));

				//var output = response.Content.ReadAsStringAsync();

				//locally
				blacklist.Add(new SimpleListItem { labelName = name });
				Statics.Default.setBlacklist(blacklist);

				nameEntry.Text = "";
			}
		}
	}
}
