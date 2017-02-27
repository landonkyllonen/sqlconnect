using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SQLConnect
{
	public class LogsTab:ContentPage
	{
		public LogsTab()
		{
			RelativeLayout holder = new RelativeLayout();

			//Load logs and initialize list.
			ObservableCollection<LogListItem> logs = Statics.Default.getLogs();

			logs.Add(new LogListItem { logTitle = "TestLog", logDate = "2017-01-01", logMeds = new string[] { "med1", "med2", "med3" }, logText = "sample text", logPublic = false, logImportant = true });
			logs.Add(new LogListItem { logTitle = "TestLog", logDate = "2017-01-01", logMeds = new string[] { "med1", "med2", "med3" }, logText = "sample text", logPublic = true, logImportant = false });
			logs.Add(new LogListItem { logTitle = "TestLog", logDate = "2017-01-01", logMeds = new string[] { "med1", "med2", "med3" }, logText = "sample text", logPublic = true, logImportant = true });


			ListView logList = new ListView
			{
				RowHeight = 80
			};
			logList.ItemsSource = logs;

			//Create row layouts
			var logDataTemplate = new DataTemplate(() =>
			{
				RelativeLayout templateHolder = new RelativeLayout { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };

				var title = new Label { FontSize = 15, TextColor = Color.Teal, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start };
				var date = new Label { FontSize = 15, TextColor = Color.Teal, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start };
				var visibility = new Image { Aspect = Aspect.Fill, Source="group.png" };
				var important = new Image { Aspect = Aspect.Fill, Source="important.png" };

				title.SetBinding(Label.TextProperty, "logTitle");
				date.SetBinding(Label.TextProperty, "logDate");
				visibility.SetBinding(IsVisibleProperty, "logPublic");
				important.SetBinding(IsVisibleProperty, "logImportant");

				templateHolder.Children.Add(title, Constraint.Constant(15), Constraint.Constant(10),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Width /2 - 15;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Height / 2 - 10;
									}));
				templateHolder.Children.Add(date, Constraint.Constant(15),
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

				templateHolder.Children.Add(visibility, Constraint.RelativeToParent((parent) => { return parent.Width *3/4-15; }),
											Constraint.RelativeToParent((parent) => { return parent.Height / 2 - 15; }),
											Constraint.Constant(30), Constraint.Constant(30));

				templateHolder.Children.Add(important, Constraint.RelativeToParent((parent) => { return parent.Width*3/4+30; }),
											Constraint.RelativeToParent((parent) => { return parent.Height / 2 - 15; }),
											Constraint.Constant(30), Constraint.Constant(30));

				return new ViewCell { View = templateHolder };
			});

			logList.ItemTemplate = logDataTemplate;
			logList.ItemTapped+= logSelected;

			holder.Children.Add(logList, Constraint.Constant(0), Constraint.Constant(0),
								Constraint.RelativeToParent((parent) =>
								{
									return parent.Width;
								}),
								Constraint.RelativeToParent((parent) =>
								{
									return parent.Height * .85;
								}));

			//Now create bottom bar
			RelativeLayout bottom = new RelativeLayout { BackgroundColor = Color.FromHex("#009a9a") };

			Button createLog = new Button
			{
				Text = "+ Create Log",
				TextColor = Color.White,
				BackgroundColor = Color.FromHex("#009a9a")
			};
			createLog.Clicked += toCreateLog;

			string[] sortTypes = new string[] { "Date", "Public", "Importance" };
			Picker sorting = new Picker
			{
				TextColor = Color.White,
				BackgroundColor = Color.FromHex("#009a9a")
			};

			foreach (string s in sortTypes)
			{
				sorting.Items.Add(s);
			}

			Label sortLbl = new Label { TextColor = Color.White, Text = "Sort By:", HorizontalTextAlignment=TextAlignment.Center, VerticalTextAlignment=TextAlignment.Center };

			bottom.Children.Add(createLog, Constraint.Constant(0), Constraint.Constant(0),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Width *2/3;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Height;
									}));

			bottom.Children.Add(sortLbl, Constraint.RelativeToParent((parent) => { return parent.Width * 2 / 3; }), Constraint.Constant(0),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Width * 1 / 3;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Height * 1 / 3;
									}));

			bottom.Children.Add(sorting, Constraint.RelativeToParent((parent) => { return parent.Width*2/3+8; }), Constraint.RelativeToParent((parent) => { return parent.Height * 1 / 3; }),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Width * 1 / 3-16;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Height*2/3;
									}));

			holder.Children.Add(bottom, Constraint.Constant(0), Constraint.RelativeToParent((parent) => { return parent.Height * 0.85; }),
								Constraint.RelativeToParent((parent) => { return parent.Width; }),
			                    Constraint.RelativeToParent((parent) => { return parent.Height * 0.15; }));

			Content = holder;
		}

		public void toCreateLog(object s, EventArgs e)
		{
			Navigation.PushModalAsync(new CreateLogPage());
		}

		public void logSelected(object sender, ItemTappedEventArgs e)
		{

		}
	}
}
