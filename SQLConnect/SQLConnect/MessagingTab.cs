using System.Collections.ObjectModel;
using Xamarin.Forms;
using System;

namespace SQLConnect
{
	public class MessagingTab : ContentPage
	{
		ObservableCollection<MessageListItem> messages;
		ObservableCollection<MessageListItem> messagesFiltered;

		ListView messageList;
		SearchBar userFilter;

		bool searching;

		public Command LoadMessagesCommand
		{
			get
			{
				return new Command(ExecuteLoadMessagesCommand, () =>
			   {
				   return !IsBusy;
			   });
			}
		}

		public MessagingTab()
		{
			searching = false;

			//Initialize list
			messageList = new ListView();
			messageList.IsPullToRefreshEnabled = true;
			messageList.RefreshCommand = LoadMessagesCommand;

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

				/*title.SetBinding(Label.TextColorProperty, new Binding("msgViewed", BindingMode.Default,
																	  new BinaryToColorConverter(), null));
				from.SetBinding(Label.TextColorProperty, new Binding("msgViewed", BindingMode.Default,
																	  new BinaryToColorConverter(), null));
				date.SetBinding(Label.TextColorProperty, new Binding("msgViewed", BindingMode.Default,
																	  new BinaryToColorConverter(), null));*/
				
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
			messagesFiltered = new ObservableCollection<MessageListItem>();


			messageList.ItemsSource = messages;
			messageList.ItemTapped += onMessageSelect;

			userFilter = new SearchBar
			{
				TextColor = Color.Teal,
				CancelButtonColor = Color.Teal,
				Placeholder = "Filter by username",
				PlaceholderColor=Color.Gray
			};
			userFilter.TextChanged += Handle_TextChanged;
			userFilter.SearchButtonPressed += (sender, e) => userFilter.Unfocus();

			Button contacts = new Button
			{
				Text = "Contacts",
				BackgroundColor=Color.Teal,
				TextColor=Color.White,
				HorizontalOptions=LayoutOptions.Center,
				VerticalOptions=LayoutOptions.Center
			};
			contacts.Clicked += toContacts;

			Button functionToggle = new Button
			{
				Text = "Compose Message",
				BackgroundColor = Color.Teal,
				TextColor = Color.White,
				FontSize=16,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};
			functionToggle.Clicked += toCompose;

			Button blacklist = new Button
			{
				Text = "Blacklist",
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};
			blacklist.Clicked += toBlacklist;

			//Define container
			RelativeLayout relativeLayout = new RelativeLayout();
			relativeLayout.HorizontalOptions = LayoutOptions.Fill;
			relativeLayout.VerticalOptions = LayoutOptions.Fill;

			//Add to container
			relativeLayout.Children.Add(messageList, Constraint.RelativeToParent((parent) => { return 0; }),
										Constraint.RelativeToParent((parent) => { return parent.Height * .1+5; }),
			                            Constraint.RelativeToParent((parent) => { return parent.Width; }),
			                            Constraint.RelativeToParent((parent) => { return parent.Height*.75-5; }));

			relativeLayout.Children.Add(userFilter, Constraint.RelativeToParent((parent) => { return parent.Width / 2 - 125; }),
										Constraint.RelativeToParent((parent) => { return 5; }),
										Constraint.Constant(220), Constraint.RelativeToParent((parent) => { return parent.Height * .1; }));

			relativeLayout.Children.Add(contacts, Constraint.RelativeToParent((parent) =>
										{
											return parent.Width*0+10;
										}), Constraint.RelativeToParent((parent) =>
										{
											return parent.Height * .85;
										}), Constraint.RelativeToParent((parent) =>
										{
											return parent.Width * .3;
										}), Constraint.RelativeToParent((parent) =>
										{
											return parent.Height * .15;
										}));

			relativeLayout.Children.Add(functionToggle, Constraint.RelativeToParent((parent) =>
										{
											return parent.Width/3+10;
										}), Constraint.RelativeToParent((parent) =>
										{
											return parent.Height * .85-5;
										}), Constraint.RelativeToParent((parent) =>
										{
											return parent.Width*.3;
										}), Constraint.RelativeToParent((parent) =>
										{
											return parent.Height * .15+10;
										}));

			relativeLayout.Children.Add(blacklist, Constraint.RelativeToParent((parent) =>
										{
											return parent.Width*2/3+10;
										}), Constraint.RelativeToParent((parent) =>
										{
											return parent.Height * .85;
										}), Constraint.RelativeToParent((parent) =>
										{
											return parent.Width * .3;
										}), Constraint.RelativeToParent((parent) =>
										{
											return parent.Height * .15;
										}));

			Content = relativeLayout;
		}

		void Handle_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!Statics.Default.isOffline())
			{
				searching = true;

				messageList.BeginRefresh();

				if (string.IsNullOrWhiteSpace(e.NewTextValue))
				{
					messageList.ItemsSource = messages;
				}else {
					messagesFiltered.Clear();
					foreach (MessageListItem item in messages)
					{
						if (item.msgFrom.ToLower().Contains(e.NewTextValue.ToLower()))
						{
							messagesFiltered.Add(item);
						}
					}
					messageList.ItemsSource = messagesFiltered;
				}

				messageList.EndRefresh();

				searching = false;
			}
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

		public async void toContacts(object s, EventArgs e)
		{
			NavigationPage nav = new NavigationPage(new ContactsPage());
			NavigationPage.SetHasBackButton(nav, true);
			await Navigation.PushModalAsync(nav);
		}

		public async void toCompose(object s, EventArgs e)
		{
			NavigationPage nav = new NavigationPage(new ComposeMessagePage(null, null));
			NavigationPage.SetHasBackButton(nav, true);
			await Navigation.PushModalAsync(nav);
		}

		public async void toBlacklist(object s, EventArgs e)
		{
			NavigationPage nav = new NavigationPage(new BlacklistPage());
			NavigationPage.SetHasBackButton(nav, true);
			await Navigation.PushModalAsync(nav);
		}

		private async void ExecuteLoadMessagesCommand()
		{
			if (IsBusy||searching)
				return;

			IsBusy = true;
			LoadMessagesCommand.ChangeCanExecute();

			ObservableCollection<MessageListItem> messagesRefreshed = new ObservableCollection<MessageListItem>();

			string auth = Statics.Default.getCreds()[11];

			byte[] saltDefault = { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };

			//Connect to url.
			var client = new System.Net.Http.HttpClient();

			//Show that we are waiting for a response and wait for it.

			var response = await client.GetAsync("http://cbd-online.net/landon/acquireMessages.php?" +
												 "user=" + System.Net.WebUtility.UrlEncode(Statics.Default.getUser()));

			var output = await response.Content.ReadAsStringAsync();

			//Process the output.
			string[] messageObjects = output.Split(new string[] { ";;" }, StringSplitOptions.None);

			//If the split yields only 1 object with value "", return no messages.
			//Not sure why this value is retrieved in the first place.
			if (messageObjects[0].Equals(""))
			{
				messageObjects = new string[0];
			}

			//Separate into components and turn into objects.
			string authHalf = Statics.Default.getAuthHalf();

			string complete = authHalf + auth;

			//bound as $title--$msg--$date--$viewed--$from--$id;;
			foreach (string obj in messageObjects)
			{
				string[] messageComponents = obj.Split(new string[] { "--" }, StringSplitOptions.None);

				bool viewed = false;
				if (messageComponents[3].Equals("1")) { viewed = true; }
				messagesRefreshed.Add(new MessageListItem
				{
					msgId = int.Parse(messageComponents[5]),
					msgContent = Crypto.DecryptAes(Convert.FromBase64String(messageComponents[1]), complete, saltDefault),
					msgDate = messageComponents[2],
					msgFrom = messageComponents[4],
					msgTitle = Crypto.DecryptAes(Convert.FromBase64String(messageComponents[0]), complete, saltDefault),
					msgViewed = viewed
				});
			}

			//messages now contains all the messages for this user that are not deleted, 
			Statics.Default.setMessages(messagesRefreshed);
			messages = messagesRefreshed;
			messageList.ItemsSource = messages;

			IsBusy = false;
			LoadMessagesCommand.ChangeCanExecute();
			messageList.EndRefresh();
		}
	}
}

