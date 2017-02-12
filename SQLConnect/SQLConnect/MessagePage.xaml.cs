using System;
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

		async void reply(object sender, EventArgs e)
		{
			NavigationPage nav = new NavigationPage(new ComposeMessagePage());
			NavigationPage.SetHasBackButton(nav, true);
			await Navigation.PushModalAsync(nav);
		}

		async void delete(object sender, EventArgs e)
		{
			//Connect to url.
			var client = new System.Net.Http.HttpClient();

			//Show that we are waiting for a response and wait for it.

			var response = await client.GetAsync("http://cbd-online.net/landon/deleteMessage.php?" +
			                                     "id=" + Statics.Default.UrlEncodeParameter(msgClicked.msgId.ToString()));

			await response.Content.ReadAsStringAsync();

			await Navigation.PopModalAsync();
		}
	}
}
