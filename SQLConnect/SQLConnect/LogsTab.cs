using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SQLConnect
{
	public class LogsTab
	{
		public LogsTab()
		{
			RelativeLayout holder = new RelativeLayout();

			//Load logs and initialize list.
			ObservableCollection<LogListItem> logs = Statics.Default.getLogs();

			ListView logList = new ListView { 
				RowHeight= 80
			};

			//Create row layouts
			var logDataTemplate = new DataTemplate(() =>
			{
				RelativeLayout templateHolder = new RelativeLayout { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };

				var title = new Label { FontSize = 15, TextColor = Color.Teal, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center };
				var date = new Label { FontSize = 15, TextColor = Color.Teal, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center };

				title.SetBinding(Label.TextProperty, "logTitle");
				date.SetBinding(Label.TextProperty, "logDate");

				templateHolder.Children.Add(title, Constraint.Constant(15), Constraint.Constant(10),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Width - 15;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Height / 2 - 10;
									}));
				templateHolder.Children.Add(date, Constraint.Constant(15),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Height/2;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Width / 2 - 15;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Height / 2 - 10;
									}));

				return new ViewCell { View = templateHolder };
			});

			logList.ItemTemplate = logDataTemplate;
		}
	}
}
