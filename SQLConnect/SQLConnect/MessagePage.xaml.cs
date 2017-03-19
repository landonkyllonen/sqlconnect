using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SQLConnect
{
	public partial class MessagePage : ContentPage
	{
		MessageListItem msgClicked;

		public MessagePage()
		{
			InitializeComponent();

			msgClicked = Statics.Default.getMsgClicked();
			Title = msgClicked.msgTitle;
			from.Text = "From: " + msgClicked.msgFrom;
			date.Text = msgClicked.msgDate;
			content.Text = msgClicked.msgContent;

			//Check if user already has this user on blacklist or contacts.
			ObservableCollection<SimpleListItem> contacts = Statics.Default.getContacts();
			ObservableCollection<SimpleListItem> blacklist = Statics.Default.getBlacklist();

			int situation = 0;
			foreach (SimpleListItem c in contacts)
			{
				if (c.labelName.Equals(msgClicked.msgFrom))
				{
					situation = situation + 1;
					break;
				}
			}
			foreach (SimpleListItem b in blacklist)
			{
				if (b.labelName.Equals(msgClicked.msgFrom))
				{
					situation = situation + 10;
					break;
				}
			}

			userOptions.Items.Add("");
			//Restict options to what is available.
			switch (situation)
			{
				case 1://User already added to contacts.
					userOptions.Items.Add("Blacklist this user");
					break;
				case 10://User already blacklisted.
					userOptions.Items.Add("Add to contacts");
					break;
				case 11://Disable feature.
					optionsBtn.IsVisible = false;
					break;
				default:
					userOptions.Items.Add("Blacklist this user");
					userOptions.Items.Add("Add to contacts");
					break;
			}

			userOptions.SelectedIndexChanged += optionSelected;
		}

		async void reply(object sender, EventArgs e)
		{
			NavigationPage nav = new NavigationPage(new ComposeMessagePage("Re:"+msgClicked.msgTitle, msgClicked.msgFrom));
			NavigationPage.SetHasBackButton(nav, true);
			await Navigation.PushModalAsync(nav);
		}

		async void delete(object sender, EventArgs e)
		{
			var answer = await DisplayAlert("Delete?", "Do you really want to delete this message?", "Yes", "No");

			if (answer)
			{
				//Connect to url.
				var client = new System.Net.Http.HttpClient();

				//Show that we are waiting for a response and wait for it.

				var response = await client.GetAsync("http://cbd-online.net/landon/deleteMessage.php?" +
													 "id=" + Statics.Default.UrlEncodeParameter(msgClicked.msgId.ToString()));

				await response.Content.ReadAsStringAsync();

				//Remove locally
				ObservableCollection<MessageListItem> pulled = Statics.Default.getMessages();
				pulled.Remove(msgClicked);
				Statics.Default.setMessages(pulled);

				await Navigation.PopModalAsync();
			}
		}

		void optionSelected(object s, EventArgs e)
		{
			switch (userOptions.Items[userOptions.SelectedIndex])
			{
				case "Blacklist this user":
					addToBlacklist(s, e);
					break;
				case "Add to contacts":
					addToContacts(s, e);
					break;
				default:
					//do nothing
					break;
			}
		}

		void openOptions(object s, EventArgs e)
		{
			userOptions.Focus();
		}

		async void addToContacts(object s, EventArgs e)
		{
			//Display confirmation dialog.
			var answer = await DisplayAlert("Add to contacts?", "Do you want to add this user to your contacts?", "Yes", "No");

			if (answer)
			{

				//add contact listed in entry.
				string name = msgClicked.msgFrom;

				//db
				//Connect to url.
				var client = new System.Net.Http.HttpClient();

				//Show that we are waiting for a response and wait for it.

				var response = await client.GetAsync("http://cbd-online.net/landon/changeUserList.php?" +
													 "user=" + System.Net.WebUtility.UrlEncode(Statics.Default.getUser()) +
													 "&type=" + System.Net.WebUtility.UrlEncode("Contacts") +
													 "&itemname=" + System.Net.WebUtility.UrlEncode(name) +
														"&action=" + System.Net.WebUtility.UrlEncode("Add"));

				//var output = response.Content.ReadAsStringAsync();

				//locally
				ObservableCollection<SimpleListItem> contacts = Statics.Default.getContacts();
				contacts.Add(new SimpleListItem { labelName = name });
				Statics.Default.setContacts(contacts);

				//Remove this option.
				userOptions.Items.Remove("Add to contacts");
				userOptions.SelectedIndex = 0;
				//If it was last option, hide button.
				if (userOptions.Items.Count < 2){optionsBtn.IsVisible = false;}

				await DisplayAlert("Success", "User added to contacts.", "OK");
			}
		}

		async void addToBlacklist(object s, EventArgs e)
		{
			//Display confirmation dialog.
			var answer = await DisplayAlert("Blacklist?", "Do you want to reject future messages from this user?", "Yes", "No");

			if (answer)
			{

				//add sender to contacts.
				string name = msgClicked.msgFrom;

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
				ObservableCollection<SimpleListItem> blacklist = Statics.Default.getBlacklist();
				blacklist.Add(new SimpleListItem { labelName = name });
				Statics.Default.setBlacklist(blacklist);

				//Remove this option.
				userOptions.Items.Remove("Blacklist this user");
				userOptions.SelectedIndex = 0;
				//If last option, hide button.
				if (userOptions.Items.Count < 2) { optionsBtn.IsVisible = false; }

				await DisplayAlert("Success", "User added to blacklist.", "OK");
			}
		}
	}
}
