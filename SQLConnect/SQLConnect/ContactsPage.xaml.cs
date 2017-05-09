using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using Xamarin.Forms;

namespace SQLConnect
{
	public partial class ContactsPage : ContentPage
	{
		ObservableCollection<SimpleListItem> contacts;
		public ContactsPage()
		{
			InitializeComponent();

			contacts = Statics.Default.getContacts();

			contactList.ItemsSource = contacts;
			contactList.ItemTapped += onItemSelect;

			if (contacts.Count < 1)
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
				content.Add(new StringContent("Contacts"), "type");
				content.Add(new StringContent(((SimpleListItem)e.Item).labelName), "itemname");
				content.Add(new StringContent("Remove"), "action");

				var response = await client.PostAsync("http://cbd-online.net/landon/changeUserList.php", content);

				await response.Content.ReadAsStringAsync();

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

				if (pulled.Count < 1)
				{
					empty.IsVisible = true;
				}
			}
			return;
		}

		async void addContact(object sender, EventArgs e)
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
			content.Add(new StringContent("Contacts"), "type");
			content.Add(new StringContent(name), "itemname");
			content.Add(new StringContent("Add"), "action");

			await client.PostAsync("http://cbd-online.net/landon/changeUserList.php", content);

			//var output = response.Content.ReadAsStringAsync();

			//locally
			contacts.Add(new SimpleListItem { labelName = name });
			Statics.Default.setContacts(contacts);

			nameEntry.Text = "";

			empty.IsVisible = false;
		}
	}
}
