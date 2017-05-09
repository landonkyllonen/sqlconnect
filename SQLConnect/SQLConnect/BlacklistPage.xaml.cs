using System;
using System.Collections.ObjectModel;
using System.Net.Http;
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

			if (blacklist.Count < 1)
			{
				empty.IsVisible = true;
			}
		}

		async void onItemSelect(object sender, ItemTappedEventArgs e)
		{
			//Display confirmation dialog.
			var answer = await DisplayAlert("Delete?", "Do you really want to remove this user?", "Yes", "No");

			if (answer)
			{
				//Connect to url.
				var client = new HttpClient();

				var content = new MultipartFormDataContent();
				content.Add(new StringContent(Statics.Default.getUser()), "user");
				content.Add(new StringContent("Blacklist"), "type");
				content.Add(new StringContent(((SimpleListItem)e.Item).labelName), "itemname");
				content.Add(new StringContent("Remove"), "action");

				var response = await client.PostAsync("http://cbd-online.net/landon/changeUserList.php", content);

				await response.Content.ReadAsStringAsync();

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

				if (pulled.Count < 1)
				{
					empty.IsVisible = true;
				}
			}
			return;
		}

		async void addToBlacklist(object sender, EventArgs e)
		{
			sender.ToString();
			e.ToString();
			//Display confirmation dialog.
			var answer = await DisplayAlert("Blacklist?", "Do you want to reject future messages from this user?", "Yes", "No");

			if (answer)
			{

				console.Text = "";
				//add contact listed in entry.
				string name = nameEntry.Text;
				if (string.IsNullOrEmpty(name))
				{
					console.Text = "You must enter a name.";
					return;
				}

				//db
				//Connect to url.
				var client = new HttpClient();

				var content = new MultipartFormDataContent();
				content.Add(new StringContent(Statics.Default.getUser()), "user");
				content.Add(new StringContent("Blacklist"), "type");
				content.Add(new StringContent(name), "itemname");
				content.Add(new StringContent("Add"), "action");

				await client.PostAsync("http://cbd-online.net/landon/changeUserList.php", content);

				//var output = response.Content.ReadAsStringAsync();

				//locally
				blacklist.Add(new SimpleListItem { labelName = name });
				Statics.Default.setBlacklist(blacklist);

				nameEntry.Text = "";

				empty.IsVisible = false;
			}
		}
	}
}
