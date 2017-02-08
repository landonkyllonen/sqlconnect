using System;
using System.Collections.Generic;

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
			from.Text = msgClicked.msgFrom;
			date.Text = msgClicked.msgDate;
			content.Text = msgClicked.msgContent;
		}

		async void reply(object sender, System.EventArgs e)
		{
			await Navigation.PushModalAsync(new Page());
		}

		async void delete(object sender, System.EventArgs e)
		{
			//Connect to url.
			var client = new System.Net.Http.HttpClient();

			//Show that we are waiting for a response and wait for it.

			var response = await client.GetAsync("http://cbd-online.net/landon/deleteMessage.php?" +
			                                     "id=" + Statics.Default.UrlEncodeParameter(msgClicked.msgId.ToString()));

			var output = await response.Content.ReadAsStringAsync();		
		}
	}
}
