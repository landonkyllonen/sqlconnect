using System;
using System.Net.Http;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SQLConnect
{
	public class CartTab : ContentPage
	{
		ObservableCollection<CartListItem> cartItems;
		ListView cartList;

		public CartTab()
		{
			//Initialize list
			cartList = new ListView();
			cartList.RowHeight = 70;

			//Create row layouts
			var productDataTemplate = new DataTemplate(() =>
			{
				RelativeLayout holder = new RelativeLayout { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };

				var name = new Label { FontSize=15, TextColor = Color.Teal, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start };
				var amount = new Label { FontSize = 15, TextColor = Color.Teal, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.End };
				var unitType = new Label { FontSize = 15, TextColor = Color.Teal, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start };
				var rate = new Label { FontSize = 15, TextColor = Color.Teal, VerticalTextAlignment = TextAlignment.Start, HorizontalTextAlignment = TextAlignment.End };
				var total = new Label { FontSize = 15, TextColor = Color.Teal, VerticalTextAlignment = TextAlignment.End, HorizontalTextAlignment = TextAlignment.End };

				name.SetBinding(Label.TextProperty, "prodName");
				amount.SetBinding(Label.TextProperty, "prodAmount");
				unitType.SetBinding(Label.TextProperty, "prodUnitType");
				total.SetBinding(Label.TextProperty, "prodTotal");
				rate.SetBinding(Label.TextProperty, "prodRate");


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
										return parent.Height/2;
									}));


				holder.Children.Add(rate, Constraint.RelativeToView(total, (parent, sibling) =>
									{
										return sibling.X;
									}),
				                    Constraint.RelativeToParent((parent) => { return parent.Height / 2;}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Width / 3 - 15;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Height/2;
									}));

				return new ViewCell { View = holder };
			});

			cartList.ItemTemplate = productDataTemplate;

			cartItems = Statics.Default.getCartItems();

			/*cartItems = new ObservableCollection<CartListItem>();
			cartItems.Add(new CartListItem { prodName = "Pineapple Express", prodAmount = 3, prodUnitType = "oz.", prodTotal = "$60.85" });
			cartItems.Add(new CartListItem { prodName = "Peanut Butter Cup", prodAmount = 2, prodUnitType = "", prodTotal = "$3.50" });
			cartItems.Add(new CartListItem { prodName = "Leaf T-Shirt", prodAmount = 1, prodUnitType = "", prodTotal = "$14.50" });
			cartItems.Add(new CartListItem { prodName = "Glass Pipe", prodAmount = 1, prodUnitType = "", prodTotal = "$10.75" });*/

			cartList.ItemsSource = cartItems;
			cartList.ItemTapped += removeFromCart;

			double totalprice = 0;
			foreach (CartListItem c in cartItems)
			{
				double number = double.Parse(c.prodTotal.Substring(1));
				totalprice += number;
			}

			Label totalLbl = new Label
			{
				TextColor = Color.Teal,
				Text = "Total: $" + totalprice,
				FontSize=20,
				HorizontalTextAlignment=TextAlignment.Center,
				VerticalTextAlignment=TextAlignment.Center
			};

			Button checkoutButton = new Button
			{
				Text = "Checkout",
				BackgroundColor = Color.Teal,
				FontSize=18,
				TextColor = Color.White,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};
			checkoutButton.Clicked += checkout;

			//Define container
			RelativeLayout relativeLayout = new RelativeLayout();
			relativeLayout.HorizontalOptions = LayoutOptions.Fill;
			relativeLayout.VerticalOptions = LayoutOptions.Fill;

			//Add to container
			relativeLayout.Children.Add(cartList, Constraint.RelativeToParent((parent) => { return 0; }),
										Constraint.RelativeToParent((parent) => { return 0; }),
										Constraint.RelativeToParent((parent) => { return parent.Width; }),
										Constraint.RelativeToParent((parent) => { return parent.Height * .77; }));

			relativeLayout.Children.Add(totalLbl, Constraint.Constant(0), Constraint.RelativeToParent((parent) => { return parent.Height * .77; }),
										Constraint.RelativeToParent((parent) => { return parent.Width; }), Constraint.RelativeToParent((parent) => { return parent.Height * .1; }));

			relativeLayout.Children.Add(checkoutButton, Constraint.RelativeToParent((parent) =>
										{
											return parent.Width / 2 - 80;
										}), Constraint.RelativeToParent((parent) =>
										{
											return parent.Height * .87;
										}), Constraint.Constant(160), Constraint.RelativeToParent((parent) =>
										{
											return parent.Height * .1;
										}));

			Content = relativeLayout;
		}

		async void checkout(object sender, EventArgs e)
		{
			//Check for suspension, if transactions suspended, disallow new additions and disallow checkout.
			if (Statics.Default.getCreds()[17] != "0")
			{
				await DisplayAlert("Transactions suspended", "Transactions for all users have been suspended temporarily by the dispensary owner. Try again later. (You will have to log out and back in)", "Okay");
				return;
			}
			else if (cartItems.Count < 1)
			{
				await DisplayAlert("No items", "You must add items to your cart before you can place an order.", "Okay");
				return;
			}
			else
			{
				DateTime date = DateTime.Today;
				double total = 0;
				string cartString = "";

				foreach (CartListItem c in cartItems)
				{
					total += double.Parse(c.prodTotal);
					cartString += c.prodName + "--" + c.prodAmount + "--" + c.prodUnitType + "--" + c.prodTotal + "--" + c.prodRate + ";;";
				}

				//Cut off extra delims.
				cartString = cartString.Substring(0, cartString.Length - 2);

				OrderListItem order = new OrderListItem
				{
					orderDate = date.ToString("d"),
					orderTotal = total.ToString(),
					orderPaymentStatus = "Unpaid",
					orderCompletionStatus = "Incomplete",
					orderItems = cartItems
				};

				//Connect to url.
				var client = new HttpClient();

				var content = new MultipartFormDataContent();
				content.Add(new StringContent(cartString), "cart");
				content.Add(new StringContent(order.orderDate), "date");
				content.Add(new StringContent(Statics.Default.getUser()), "user");
				content.Add(new StringContent(order.orderTotal), "total");

				var response = await client.PostAsync("http://cbd-online.net/landon/saveOrder.php", content);

				var output = await response.Content.ReadAsStringAsync();
				//Will output new auto id.
				string[] returned = output.Split(new string[] { ";;" }, StringSplitOptions.RemoveEmptyEntries);
				if (returned[0].Equals("true"))
				{
					order.orderID = int.Parse(returned[1]);
					//Save new order locally.
					ObservableCollection<OrderListItem> pulled = Statics.Default.getOrders();
					pulled.Add(order);
					Statics.Default.setOrders(pulled);
				}
				else
				{
					//Something went wrong in php.
					await DisplayAlert("Error", "Sorry, something went wrong while submitting your order.", "Okay");
				}
			}
		}

		async void removeFromCart(object s, ItemTappedEventArgs e)
		{
			bool answer = await DisplayAlert("Remove From Cart?", "Are you sure you want to remove this item?", "Yes", "No");

			if (answer)
			{
				CartListItem pressed = (CartListItem)e.Item;
				cartItems.Remove(pressed);
				cartList.ItemsSource = cartItems;
				//Update statics.
				Statics.Default.setCartItems(cartItems);
				//Save changes for reload.
				Statics.Default.serializeAndSave("cart");
			}
		}
	}
}

