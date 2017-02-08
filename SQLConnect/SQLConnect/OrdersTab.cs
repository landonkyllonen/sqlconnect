using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SQLConnect
{
	public class OrdersTab : ContentPage
	{
		ObservableCollection<OrderListItem> orderItems;

		public OrdersTab()
		{
			//Initialize list
			ListView orderList = new ListView();
			orderList.RowHeight = 60;

			//Create row layouts
			var orderDataTemplate = new DataTemplate(() =>
			{
				RelativeLayout holder = new RelativeLayout { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };

				var id = new Label { FontSize = 15, TextColor = Color.Teal, VerticalTextAlignment = TextAlignment.Start, HorizontalTextAlignment = TextAlignment.Start };
				var date = new Label { FontSize = 15, TextColor = Color.Teal, VerticalTextAlignment = TextAlignment.End, HorizontalTextAlignment = TextAlignment.Start };
				var total = new Label { FontSize = 15, TextColor = Color.Teal, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.End };
				var payment = new Label { FontSize = 15, TextColor = Color.Teal, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.End };
				var delivery = new Label { FontSize = 15, TextColor = Color.Teal, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.End };


				id.SetBinding(Label.TextProperty, "orderID");
				date.SetBinding(Label.TextProperty, "orderDate");
				total.SetBinding(Label.TextProperty, "orderTotal");
				payment.SetBinding(Label.TextProperty, "orderPaymentStatus");
				delivery.SetBinding(Label.TextProperty, "orderCompletionStatus");


				holder.Children.Add(id, Constraint.Constant(15), Constraint.Constant(10),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Width / 4 - 10;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Height/2;
									}));
				holder.Children.Add(date, Constraint.Constant(15), 
				                    Constraint.RelativeToParent((parent) =>
									{
										return parent.Height/2;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Width / 4 - 10;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Height/2-10;
									}));
				holder.Children.Add(total, Constraint.RelativeToParent((parent) =>
									{
										return parent.Width / 4;
									}),
									Constraint.Constant(0),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Width / 4;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Height;
									}));
				holder.Children.Add(delivery, Constraint.RelativeToParent((parent) =>
									{
										return parent.Width*3/4;
									}),
									Constraint.Constant(0),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Width * 1 / 4 -15;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Height;
									}));
				holder.Children.Add(payment, Constraint.RelativeToParent((parent) =>
									{
										return parent.Width * 1/2;
									}),
									Constraint.Constant(0),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Width * 1 / 4-15;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Height;
									}));

				return new ViewCell { View = holder };
			});

			orderList.ItemTemplate = orderDataTemplate;

			orderItems = new ObservableCollection<OrderListItem>();
			orderItems.Add(new OrderListItem { orderID = 62378346, orderDate = "2017-01-27", orderTotal = "$54.60", orderPaymentStatus = "Unpaid", orderCompletionStatus="Ready" });
			orderItems.Add(new OrderListItem { orderID = 73458659, orderDate = "2017-01-24", orderTotal = "$23.60", orderPaymentStatus = "Paid", orderCompletionStatus = "Ready" });
			orderItems.Add(new OrderListItem { orderID = 12543473, orderDate = "2017-01-21", orderTotal = "$43.45", orderPaymentStatus = "Paid", orderCompletionStatus = "Received" });
			orderItems.Add(new OrderListItem { orderID = 62346437, orderDate = "2017-01-19", orderTotal = "$70.15", orderPaymentStatus = "Paid", orderCompletionStatus = "Received" });

			orderList.ItemsSource = orderItems;

			//Define container
			RelativeLayout relativeLayout = new RelativeLayout();
			relativeLayout.HorizontalOptions = LayoutOptions.Fill;
			relativeLayout.VerticalOptions = LayoutOptions.Fill;

			//Add to container
			relativeLayout.Children.Add(orderList, Constraint.RelativeToParent((parent) => { return 0; }),
										Constraint.RelativeToParent((parent) => { return 0; }),
										Constraint.RelativeToParent((parent) => { return parent.Width; }),
										Constraint.RelativeToParent((parent) => { return parent.Height; }));

			Content = relativeLayout;
		}

	}
}