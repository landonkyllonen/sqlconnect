using System;
using Xamarin.Forms;
using System.Net;

namespace SQLConnect
{
	public partial class ComposeMessagePage : ContentPage
	{
		public ComposeMessagePage()
		{
			InitializeComponent();
			title.Completed += (sender, e) => to.Focus();
			to.Completed += (sender, e) => content.Focus();
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
			var client = new System.Net.Http.HttpClient();

			//Show that we are waiting for a response and wait for it.

			var response = await client.GetAsync("http://cbd-online.net/landon/getAuth.php?" +
			                                     "user=" + WebUtility.UrlEncode(to.Text));

			var authRetrieved = await response.Content.ReadAsStringAsync();

			//Encrypt
			byte[] saltDefault = {0x20,0x20,0x20,0x20,0x20, 0x20, 0x20, 0x20};

			string authHalf = Statics.Default.getAuthHalf();

			string complete = authHalf + authRetrieved;

			string messageCrypt = Convert.ToBase64String(Crypto.EncryptAes(message, complete, saltDefault));

			string titleCrypt = Convert.ToBase64String(Crypto.EncryptAes(titletext, complete, saltDefault));

			//Show that we are waiting for a response and wait for it.

			await client.GetAsync("http://cbd-online.net/landon/messageTemplate.php?" +
			                                     "user=" + WebUtility.UrlEncode(to.Text) +
			                                     "&sender=" + WebUtility.UrlEncode(Statics.Default.getUser()) +
			                                     "&msg=" + WebUtility.UrlEncode(messageCrypt) +
			                                     "&date=" + WebUtility.UrlEncode(date.ToString("d")) +
			                                     "&title=" + WebUtility.UrlEncode(titleCrypt));


			//INSERT FEEDBACK TOAST HERE

			await Navigation.PopModalAsync();
		}
	}
}
