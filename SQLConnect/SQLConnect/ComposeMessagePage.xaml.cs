using System;
using System.Diagnostics;
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
			string auth = Statics.Default.getCreds()[11];

			DateTime date = DateTime.Today;

			string message = content.Text;

			string titletext = title.Text;
			//Encrypt
			byte[] saltDefault = {0x20,0x20,0x20,0x20,0x20, 0x20, 0x20, 0x20};

			byte[] messageCrypt = Crypto.EncryptAes(message, auth, saltDefault);

			byte[] titleCrypt = Crypto.EncryptAes(titletext, auth, saltDefault);

			//Connect to url.
			var client = new System.Net.Http.HttpClient();

			//Show that we are waiting for a response and wait for it.

			var response = await client.GetAsync("http://cbd-online.net/landon/messageTemplate.php?" +
			                                     "user=" + WebUtility.UrlEncode(to.Text) +
			                                     "&sender=" + WebUtility.UrlEncode(Statics.Default.getUser()) +
			                                     "&msg=" + WebUtility.UrlEncode(Convert.ToBase64String(messageCrypt)) +
			                                     "&date=" + WebUtility.UrlEncode(date.ToString()) +
			                                     "&title=" + WebUtility.UrlEncode(Convert.ToBase64String(titleCrypt)));


			//INSERT FEEDBACK TOAST HERE

			await Navigation.PopModalAsync();
		}
	}
}
