using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SQLConnect
{
	public partial class ContactsPage : ContentPage
	{
		public ContactsPage()
		{
			InitializeComponent();

			ObservableCollection<SimpleListItem> contacts = Statics.Default.getContacts();

			contactList.ItemsSource = contacts;
			contactList.ItemTapped += onItemSelect;
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
													 "&type=" + System.Net.WebUtility.UrlEncode("Contacts") +
				                                     "&itemname=" + System.Net.WebUtility.UrlEncode(((SimpleListItem)e.Item).labelName) +
				                           			 "&action=" + System.Net.WebUtility.UrlEncode("Remove"));

				var output = await response.Content.ReadAsStringAsync();

				//and locally.
				ObservableCollection<SimpleListItem> pulled = Statics.Default.getContacts();
				foreach (SimpleListItem contact in pulled)
				{
					if (contact.labelName.Equals(((SimpleListItem)e.Item).labelName))
					{
						pulled.Remove(contact);
						break;
					}
				}

				//Reflect changes to static variable.
				Statics.Default.setContacts(pulled);

			}
			return;
		}

		void addContact(object sender, System.EventArgs e)
		{
			//add contact listed in entry.
			return;
		}
	}
}
