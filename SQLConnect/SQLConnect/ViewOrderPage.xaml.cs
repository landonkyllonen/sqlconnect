using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SQLConnect
{
	public partial class ViewOrderPage : ContentPage
	{
		public ViewOrderPage()
		{
			InitializeComponent();

			OrderListItem order = Statics.Default.getOrderClicked();

			orderItemList.ItemsSource = order.orderItems;

			idLabel.Text = "#" + order.orderID;
			totalLbl.Text = order.orderTotal;
		}
	}
}
