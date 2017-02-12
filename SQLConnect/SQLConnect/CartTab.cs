using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SQLConnect
{
	public class CartTab : ContentPage
	{
		ObservableCollection<CartListItem> cartItems;

		public CartTab()
		{
			//Initialize list
			ListView cartList = new ListView();
			cartList.RowHeight = 60;

			//Create row layouts
			var productDataTemplate = new DataTemplate(() =>
			{
				RelativeLayout holder = new RelativeLayout { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };

				var name = new Label { FontSize=15, TextColor = Color.Teal, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start };
				var amount = new Label { FontSize = 15, TextColor = Color.Teal, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.End };
				var unitType = new Label { FontSize = 15, TextColor = Color.Teal, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start };
				var total = new Label { FontSize = 15, TextColor = Color.Teal, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.End };

				name.SetBinding(Label.TextProperty, "prodName");
				amount.SetBinding(Label.TextProperty, "prodAmount");
				unitType.SetBinding(Label.TextProperty, "prodUnitType");
				total.SetBinding(Label.TextProperty, "prodTotal");

				holder.Children.Add(name, Constraint.Constant(15), Constraint.Constant(0),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Width/3-10;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Height;
									}));
				holder.Children.Add(amount, Constraint.RelativeToParent((parent) =>
									{
										return parent.Width / 3;
									}),
				                    Constraint.Constant(0),
				                    Constraint.Constant(50),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Height;
									}));
				holder.Children.Add(unitType, Constraint.RelativeToView(amount, (parent, sibling) =>
									{
										return sibling.X + sibling.Width+5;
									}),
				                    Constraint.Constant(0),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Width / 3 - 50;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Height;
									}));
				holder.Children.Add(total, Constraint.RelativeToParent((parent) =>
									{
										return parent.Width*2/3;
									}),
				                    Constraint.Constant(0),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Width /3 - 15;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Height;
									}));

				return new ViewCell { View = holder };
			});

			cartList.ItemTemplate = productDataTemplate;

			cartItems = new ObservableCollection<CartListItem>();
			cartItems.Add(new CartListItem { prodName = "Pineapple Express", prodAmount = 3, prodUnitType = "oz.", prodTotal = "$60.85" });
			cartItems.Add(new CartListItem { prodName = "Peanut Butter Cup", prodAmount = 2, prodUnitType = "", prodTotal = "$3.50" });
			cartItems.Add(new CartListItem { prodName = "Leaf T-Shirt", prodAmount = 1, prodUnitType = "", prodTotal = "$14.50" });
			cartItems.Add(new CartListItem { prodName = "Glass Pipe", prodAmount = 1, prodUnitType = "", prodTotal = "$10.75" });

			cartList.ItemsSource = cartItems;

			Button checkoutButton = new Button
			{
				Text = "Checkout",
				BackgroundColor = Color.Teal,
				FontSize=18,
				TextColor = Color.White,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};

			//Define container
			RelativeLayout relativeLayout = new RelativeLayout();
			relativeLayout.HorizontalOptions = LayoutOptions.Fill;
			relativeLayout.VerticalOptions = LayoutOptions.Fill;

			//Add to container
			relativeLayout.Children.Add(cartList, Constraint.RelativeToParent((parent) => { return 0; }),
										Constraint.RelativeToParent((parent) => { return 0; }),
										Constraint.RelativeToParent((parent) => { return parent.Width; }),
										Constraint.RelativeToParent((parent) => { return parent.Height * .85; }));

			relativeLayout.Children.Add(checkoutButton, Constraint.RelativeToParent((parent) =>
										{
											return parent.Width / 2 - parent.Width * .4 / 2;
										}), Constraint.RelativeToParent((parent) =>
										{
											return parent.Height * .85;
										}), Constraint.RelativeToParent((parent) =>
										{
											return parent.Width * .6;
										}), Constraint.RelativeToParent((parent) =>
										{
											return parent.Height * .2;
										}));

			Content = relativeLayout;
		}

	}
}

