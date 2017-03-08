using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SQLConnect
{
	public partial class ProductPage : ContentPage
	{
		ProductListItem product;

		string[] medicalAmounts;
		double[] medicalPrices;
		int index;

		public ProductPage()
		{
			InitializeComponent();

			product = Statics.Default.getProdClicked();

			Title = product.prodName;
			image.Source = product.prodImgUrl;

			medicalAmounts = new string[] { "Gram", "Eighth", "Quarter", "Half Oz" };
			//Test
			medicalPrices = new double[] { 10.75, 20.00, 38.55, 72.30 };
			index = 0;
			priceExact.Text = medicalPrices[0].ToString("C");

			//Get information on what type of choices to display.
			//For now, display simple.

			if (product.prodCategory.Equals("Flowers"))
			{
				componentExact.IsVisible = true;
			}
			else {
				componentRegular.IsVisible = true;
			}

			price.Text = product.prodUnitPrice.ToString("C");
		}

		void increment(object s, EventArgs e)
		{
			value.Text = (int.Parse(value.Text) + 1).ToString();
			price.Text = (int.Parse(value.Text) * product.prodUnitPrice).ToString("C");
		}

		void decrement(object s, EventArgs e)
		{
			value.Text = (int.Parse(value.Text) - 1).ToString();
			price.Text = (int.Parse(value.Text) * product.prodUnitPrice).ToString("C");
		}

		void previous(object s, EventArgs e)
		{
			if (index == 0)
			{
				return;
			}
			else {
				index--;
				if (index == 0) { valueLeft.Text = ""; }
				else { valueLeft.Text = medicalAmounts[index -1]; }
				valueMid.Text = medicalAmounts[index];
				valueRight.Text = medicalAmounts[index + 1];
				priceExact.Text = medicalPrices[index].ToString("C");
			}
		}

		void next(object s, EventArgs e)
		{
			if (index == 3)
			{
				return;
			}
			else {
				index++;
				if (index == 3) { valueRight.Text = ""; }
				else {valueRight.Text = medicalAmounts[index + 1];}
				valueMid.Text = medicalAmounts[index];
				valueLeft.Text = medicalAmounts[index - 1];
				priceExact.Text = medicalPrices[index].ToString("C");
			}
		}

		async void addToCart(object s, EventArgs e)
		{
			CartListItem cartItem;
			if (product.prodCategory.Equals("Flowers"))
			{
				double amount;
				switch (index)
				{
					case 0:
						amount = 1;
						break;
					case 1:
						amount = 1 / 8;
						break;
					case 2:
						amount = 1 / 4;
						break;
					case 3:
						amount = 1 / 2;
						break;
					default:
						amount = 1;
						break;
				}
				cartItem = new CartListItem { prodName = product.prodName, prodAmount = amount, prodUnitType = "oz.", prodTotal = medicalPrices[index].ToString("C")};
			}
			else {
				cartItem = new CartListItem { prodName = product.prodName, prodAmount = Double.Parse(value.Text), prodUnitType = "", prodTotal = (product.prodUnitPrice * Double.Parse(value.Text)).ToString("C") };
			}
			ObservableCollection<CartListItem> pulled = Statics.Default.getCartItems();

			pulled.Add(cartItem);

			Statics.Default.setCartItems(pulled);

			var answer = await DisplayAlert("Added to Cart", "Would you like to keep browsing?", "Keep Browsing", "View Cart");

			if (answer)
			{
				await Navigation.PopModalAsync();
			}
			else {
				NavigationPage nav = new NavigationPage(new CartTab { Title = "Your Cart" });
				NavigationPage.SetHasBackButton(nav, true);
				await Navigation.PushModalAsync(nav);
			}
		}
	}
}
