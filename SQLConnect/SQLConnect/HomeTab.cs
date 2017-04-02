using System;

using Xamarin.Forms;

namespace SQLConnect
{
	public class HomeTab : ContentPage
	{
		string[] credentials;
		ProductListItem deal;

		public HomeTab()
		{
			RelativeLayout rel = new RelativeLayout
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

				//Create and add views to inner layout
				Label welcome = new Label
				{
				Text = "Hello, User!",
					FontSize = 25,
					TextColor = Color.Teal,
					HorizontalOptions = LayoutOptions.CenterAndExpand
				};

				Label dealsTag = new Label
				{
					Text = "  Current Deal",
					FontSize = 25,
					TextColor = Color.White,
					BackgroundColor = Color.FromHex("#006767")
				};

				BoxView overlay = new BoxView
				{
					Color = Color.FromHex("#00cdcd"),
					Opacity = 0.3
				};

				Label dealPrice = new Label
				{
				Text = "price/g",
					FontSize = 15,
					TextColor = Color.Teal,
					HorizontalOptions=LayoutOptions.CenterAndExpand
				};
					
				StackLayout dealInfoLayout = new StackLayout
				{
					Orientation = StackOrientation.Vertical,
					Spacing=0
				};

					Label dealName = new Label
					{
						Text = "Product Name",
						TextColor = Color.Teal,
						FontSize=15,
						HorizontalOptions=LayoutOptions.CenterAndExpand,
						VerticalOptions=LayoutOptions.CenterAndExpand
					};

					Label dealCategory = new Label
					{
						TextColor = Color.Teal,
						Text = "Category",
						HorizontalOptions = LayoutOptions.CenterAndExpand,
						VerticalOptions = LayoutOptions.CenterAndExpand
					};
					dealInfoLayout.Children.Add(dealName);
					dealInfoLayout.Children.Add(dealCategory);

				Button toDeal = new Button
				{
					Text = "Go",
					BackgroundColor=Color.FromHex("#009a9a"),
					FontSize = 18,
					TextColor=Color.White,
					HorizontalOptions = LayoutOptions.CenterAndExpand
				};
				toDeal.Clicked += goToDeal;

				BoxView dealBG = new BoxView
				{
					Color = Color.FromHex("#00cdcd"),
					Opacity = 0
				};

				BoxView underBar = new BoxView
				{
					Color = Color.FromHex("#006767")
				};


				Button toMyID = new Button
				{
					FontSize = 20,
					BackgroundColor = Color.FromHex("#009a9a"),
					Text="My Card",
					TextColor=Color.White
				};

				Button toProducts = new Button
				{
					FontSize = 20,
					BackgroundColor = Color.FromHex("#009a9a"),
					Text="Browse Products",
					TextColor = Color.White
				};
				toProducts.Clicked += toProductsTab;

				Button toHelp = new Button
				{
					FontSize = 20,
					BackgroundColor = Color.FromHex("#009a9a"),
					Text="Help",
					TextColor = Color.White
				};
				toHelp.Clicked += toHelpPage;

				rel.Children.Add(welcome, Constraint.RelativeToParent((parent) =>
							{
								return parent.X;
							}),
							Constraint.RelativeToParent((parent) =>
							{
								return parent.Y+20;
							}),
							Constraint.RelativeToParent((parent) =>
							{
								return parent.Width;
							}),
								 Constraint.Constant(30));

				rel.Children.Add(dealsTag, Constraint.RelativeToParent((parent) =>
							{
								return parent.X;
							}),
			                     Constraint.RelativeToView(welcome, (parent, sibling) =>
							{
								return sibling.Y+50;
							}),
							Constraint.RelativeToParent((parent) =>
							{
								return parent.Width;
							}),
		                         Constraint.Constant(30));

			rel.Children.Add(overlay, Constraint.RelativeToParent((parent) =>
							{
								return parent.X;
							}),
							 Constraint.RelativeToView(dealsTag, (parent, sibling) =>
							{
								return sibling.Y + 90;
							}),
							Constraint.RelativeToParent((parent) =>
							{
								return parent.Width;
							}),
							Constraint.RelativeToParent((parent) =>
							{
								return parent.Height;
							}));

			rel.Children.Add(dealBG, Constraint.RelativeToParent((parent) =>
							{
								return parent.X;
							}),
			                 Constraint.RelativeToView(dealsTag, (parent, sibling) =>
							{
								return sibling.Y+30;
							}),
							Constraint.RelativeToParent((parent) =>
							{
								return parent.Width;
							}),
								 Constraint.Constant(50));

			rel.Children.Add(dealInfoLayout, Constraint.RelativeToParent((parent) =>
							{
								return parent.X+parent.Width*.2;
							}),
			                     Constraint.RelativeToView(dealsTag, (parent, sibling) =>
							{
								return sibling.Y + 30;
							}),
							Constraint.RelativeToParent((parent) =>
							{
								return parent.Width * .6;
							}),
								 Constraint.Constant(50));



			rel.Children.Add(dealPrice, Constraint.RelativeToParent((parent) =>
							{
								return parent.X;
							}),
			                 Constraint.RelativeToView(dealInfoLayout, (parent, sibling) =>
							{
								return sibling.Y + 15;
							}),
							Constraint.RelativeToParent((parent) =>
							{
								return parent.Width * .2;
							}),
								 Constraint.Constant(20));

				rel.Children.Add(toDeal, Constraint.RelativeToParent((parent) =>
							{
								return parent.X + parent.Width * .8;
							}),
			                     Constraint.RelativeToView(dealInfoLayout, (parent, sibling) =>
							{
								return sibling.Y;
							}),
							Constraint.RelativeToParent((parent) =>
							{
								return parent.Width * .2;
							}),
								 Constraint.Constant(50));


			rel.Children.Add(underBar, Constraint.RelativeToParent((parent) =>
							{
								return parent.X;
							}),
								 Constraint.RelativeToView(dealInfoLayout, (parent, sibling) =>
							{
								return sibling.Y+50;
							}),
							Constraint.RelativeToParent((parent) =>
							{
								return parent.Width;
							}),
								 Constraint.Constant(10));

			rel.Children.Add(toMyID, Constraint.RelativeToParent((parent) =>
							{
								return parent.X + parent.Width * .2;
							}),
								 Constraint.RelativeToView(dealInfoLayout, (parent, sibling) =>
							{
								return sibling.Y+100;
							}),
							Constraint.RelativeToParent((parent) =>
							{
								return parent.Width * .6;
							}),
								 Constraint.Constant(60));

			rel.Children.Add(toProducts, Constraint.RelativeToParent((parent) =>
							{
								return parent.X + parent.Width * .2;
							}),
			                 Constraint.RelativeToView(toMyID, (parent, sibling) =>
							{
								return sibling.Y + 70;
							}),
							Constraint.RelativeToParent((parent) =>
							{
								return parent.Width * .6;
							}),
								 Constraint.Constant(60));

			rel.Children.Add(toHelp, Constraint.RelativeToParent((parent) =>
							{
								return parent.X + parent.Width * .2;
							}),
							 Constraint.RelativeToView(toProducts, (parent, sibling) =>
							{
								return sibling.Y + 70;
							}),
							Constraint.RelativeToParent((parent) =>
							{
								return parent.Width * .6;
							}),
			                 Constraint.Constant(60));

			Content = rel;

			//Initializations
			credentials = Statics.Default.getCreds();

			//If a dispensary owner(authorized), show link to owner controls.
			if (credentials[12].Equals("1"))
			{
				toMyID.Text = "Manage Dispensary";
				toMyID.Clicked += toDispensaryManagement;
			}else
			{
				toMyID.Clicked += toBarcodePage;
			}

			//If online and there is a deal, show it.
			if (!Statics.Default.isOffline() && Statics.Default.getDeal() != null)
			{
				deal = Statics.Default.getDeal();
				dealName.Text = deal.prodName;
				dealPrice.Text = "$" + deal.prodUnitPrice + "/g";
				dealCategory.Text = deal.prodCategory;
			}
			else
			{
				toDeal.IsEnabled = false;
			}

			welcome.SetValue(Label.TextProperty, "Hello, " + credentials[0] + "!");
		}

		public void goToDeal(object s, EventArgs e)
		{
			Statics.Default.getMaster().Detail = new NavigationPage(new ProductPage());
		}

		public void toProductsTab(object s, EventArgs e)
		{
			Statics.Default.getMaster().Detail = new NavigationPage(new ProductsPage());
		}

		public void toBarcodePage(object s, EventArgs e)
		{
			NavigationPage nav = new NavigationPage(new BarcodePage());
			NavigationPage.SetHasBackButton(nav, true);
			Navigation.PushModalAsync(nav);
		}

		public void toHelpPage(object s, EventArgs e)
		{
			NavigationPage nav = new NavigationPage(new BarcodePage());
			NavigationPage.SetHasBackButton(nav, true);
			Navigation.PushModalAsync(nav);
		}

		public void toDispensaryManagement(object s, EventArgs e)
		{
			NavigationPage nav = new NavigationPage(new DispensaryManagementPage());
			NavigationPage.SetHasBackButton(nav, true);
			Navigation.PushModalAsync(nav);
		}
	}
}