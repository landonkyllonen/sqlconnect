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

			string authHalf = "GFEDCBA";

			string complete = authHalf + authRetrieved;

			byte[] messageCrypt = Crypto.EncryptAes(message, complete, saltDefault);

			byte[] titleCrypt = Crypto.EncryptAes(titletext, complete, saltDefault);

			//Show that we are waiting for a response and wait for it.

			await client.GetAsync("http://cbd-online.net/landon/messageTemplate.php?" +
			                                     "user=" + WebUtility.UrlEncode(to.Text) +
			                                     "&sender=" + WebUtility.UrlEncode(Statics.Default.getUser()) +
			                                     "&msg=" + WebUtility.UrlEncode(Convert.ToBase64String(messageCrypt)) +
			                                     "&date=" + WebUtility.UrlEncode(date.ToString("d")) +
			                                     "&title=" + WebUtility.UrlEncode(Convert.ToBase64String(titleCrypt)));


			//INSERT FEEDBACK TOAST HERE

			await Navigation.PopModalAsync();
		}
	}
}
