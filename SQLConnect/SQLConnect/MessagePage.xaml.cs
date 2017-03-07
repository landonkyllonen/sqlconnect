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
			from.Text = msgClicked.msgFrom;
			date.Text = msgClicked.msgDate;
			content.Text = msgClicked.msgContent;
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
	}
}
