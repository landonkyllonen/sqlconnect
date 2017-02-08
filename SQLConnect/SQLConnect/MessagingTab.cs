using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SQLConnect
{
	public class MessagingTab : ContentPage
	{
		ObservableCollection<MessageListItem> messages;

		public MessagingTab()
		{
			//Initialize list
			ListView messageList = new ListView();
			messageList.RowHeight = 60;

			//Create row layouts
			var messageDataTemplate = new DataTemplate(() =>
			{
				RelativeLayout holder = new RelativeLayout { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };

				var title = new Label {FontSize=15, TextColor = Color.Teal, VerticalTextAlignment=TextAlignment.Start, HorizontalTextAlignment=TextAlignment.Start};
				var from = new Label {FontSize = 15, TextColor = Color.Teal, VerticalTextAlignment = TextAlignment.End, HorizontalTextAlignment = TextAlignment.Start};
				var date = new Label {FontSize = 15, TextColor = Color.Teal, VerticalTextAlignment = TextAlignment.End, HorizontalTextAlignment = TextAlignment.End};

				title.SetBinding(Label.TextProperty, "msgTitle");
				from.SetBinding(Label.TextProperty, "msgFrom");
				date.SetBinding(Label.TextProperty, "msgDate");

				holder.Children.Add(title, Constraint.Constant(15), Constraint.Constant(10),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Width-15;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Height/2-10;
									}));
				holder.Children.Add(from, Constraint.Constant(15), 
				                    Constraint.RelativeToParent((parent) =>
									{
										return parent.Height/2;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Width/2-15;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Height/2-10;
									}));
				holder.Children.Add(date, Constraint.RelativeToParent((parent) =>
									{
										return parent.Width / 2;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Height / 2;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Width / 2 - 15;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Height / 2 - 10;
									}));

				return new ViewCell { View = holder };
			});

			messageList.ItemTemplate = messageDataTemplate;

			//Get messages from static loaded at login
			messages = Statics.Default.getMessages();

			messageList.ItemsSource = messages;
			messageList.ItemTapped += onMessageSelect;

			Button functionToggle = new Button
			{
				Text = "Compose Message",
				BackgroundColor=Color.Teal,
				TextColor=Color.White,
				HorizontalOptions=LayoutOptions.Center,
				VerticalOptions=LayoutOptions.Center
			};

			//Define container
			RelativeLayout relativeLayout = new RelativeLayout();
			relativeLayout.HorizontalOptions = LayoutOptions.Fill;
			relativeLayout.VerticalOptions = LayoutOptions.Fill;

			//Add to container
			relativeLayout.Children.Add(messageList, Constraint.RelativeToParent((parent) => { return 0; }),
										Constraint.RelativeToParent((parent) => { return 0; }),
			                            Constraint.RelativeToParent((parent) => { return parent.Width; }),
			                            Constraint.RelativeToParent((parent) => { return parent.Height*.85; }));

			relativeLayout.Children.Add(functionToggle, Constraint.RelativeToParent((parent) =>
										{
											return parent.Width/2-parent.Width*.4/2;
										}), Constraint.RelativeToParent((parent) =>
										{
											return parent.Height * .85;
										}), Constraint.RelativeToParent((parent) =>
										{
											return parent.Width*.4;
										}), Constraint.RelativeToParent((parent) =>
										{
											return parent.Height * .15;
										}));

			Content = relativeLayout;
		}

		public async void onMessageSelect(object s, ItemTappedEventArgs e)
		{
			//Pass to messagePage activity to display in full screen and allow reply/delete
			var message = e.Item as MessageListItem;
			Statics.Default.setMsgClicked(message);

			NavigationPage nav = new NavigationPage(new MessagePage());
			NavigationPage.SetHasBackButton(nav, true);

			await Navigation.PushModalAsync(nav);
		}
	}
}

