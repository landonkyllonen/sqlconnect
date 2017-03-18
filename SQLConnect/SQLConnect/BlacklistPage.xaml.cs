using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SQLConnect
{
	public partial class BlacklistPage : ContentPage
	{
		public BlacklistPage()
		{
			InitializeComponent();

			ObservableCollection<SimpleListItem> blacklist = Statics.Default.getBlacklist();

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

		void addToBlacklist(object sender, System.EventArgs e)
		{
			//add user listed in entry.
			return;
		}
	}
}
