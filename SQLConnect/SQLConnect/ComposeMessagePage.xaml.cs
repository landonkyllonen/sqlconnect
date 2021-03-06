﻿using System;
using Xamarin.Forms;
using System.Net.Http;

namespace SQLConnect
{
	public partial class ComposeMessagePage : ContentPage
	{
		public ComposeMessagePage(string replytitle, string replyto)
		{
			InitializeComponent();

			if (!string.IsNullOrEmpty(replytitle) && !string.IsNullOrEmpty(replyto))
			{
				title.Text = replytitle;
				to.Text = replyto;
				to.IsEnabled = false;
				quickContact.IsVisible = false;
			}

			title.Completed += (sender, e) => to.Focus();
			to.Completed += (sender, e) => content.Focus();

			//Initialize contacts list for quick contact select.
			foreach (SimpleListItem item in Statics.Default.getContacts())
			{
				contactPick.Items.Add(item.labelName);
			}
			contactPick.SelectedIndexChanged += (sender, e) => to.Text = contactPick.Items[contactPick.SelectedIndex];

			title.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeSentence);
			content.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeSentence);
		}

		public async void sendMessage(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();

			DateTime date = DateTime.Today;

			string message = content.Text;

			string titletext = title.Text;

			//Connect to url.
			var client = new HttpClient();

			var contentsent = new MultipartFormDataContent();
			contentsent.Add(new StringContent(to.Text), "user");

			var response = await client.PostAsync("http://cbd-online.net/landon/getAuth.php", contentsent);

			var authRetrieved = await response.Content.ReadAsStringAsync();

			//Encrypt
			byte[] saltDefault = {0x20,0x20,0x20,0x20,0x20, 0x20, 0x20, 0x20};

			string authHalf = Statics.Default.getAuthHalf();

			string complete = authHalf + authRetrieved;

			string messageCrypt = Convert.ToBase64String(Crypto.EncryptAes(message, complete, saltDefault));

			string titleCrypt = Convert.ToBase64String(Crypto.EncryptAes(titletext, complete, saltDefault));

			var contentsent2 = new MultipartFormDataContent();
			contentsent2.Add(new StringContent(to.Text), "user");
			contentsent2.Add(new StringContent(Statics.Default.getUser()), "sender");
			contentsent2.Add(new StringContent(messageCrypt), "msg");
			contentsent2.Add(new StringContent(date.ToString("d")), "date");
			contentsent2.Add(new StringContent(titleCrypt), "title");

			var response2 = await client.PostAsync("http://cbd-online.net/landon/messageTemplate.php", contentsent2);

			var output = await response2.Content.ReadAsStringAsync();

			switch (output)
			{
				case "True":
					await DisplayAlert("Success", "Message Sent!", "OK");
					await Navigation.PopModalAsync();
					break;
				case "False":
					await DisplayAlert("Error", "User not found.", "OK");
					break;
				default:
					await DisplayAlert("Error", "Sorry, for some reason the message was not sent.", "OK");
					break;
			}
		}

		public void pickContact(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();
			contactPick.Focus();
			return;
		}
	}
}
