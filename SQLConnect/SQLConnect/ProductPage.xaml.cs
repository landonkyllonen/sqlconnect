using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SQLConnect
{
	public partial class ProductPage : ContentPage
	{
		ProductListItem product;

		public ProductPage()
		{
			InitializeComponent();

			product = Statics.Default.getProdClicked();

			Title = product.prodName;
			image.Source = product.prodImgUrl;

			//Get information on what type of choices to display.
			//For now, display simple.
			componentRegular.IsVisible = true;

			price.Text = product.prodUnitPrice.ToString("C");
			stepper.ValueChanged += updateStepper;
		}

		void updateStepper(object s, EventArgs e)
		{
			value.Text = stepper.Value.ToString();
			price.Text = (stepper.Value * product.prodUnitPrice).ToString("C");
		}

		void addToCart(object s, EventArgs e)
		{
			CartListItem cartItem = new CartListItem { prodName = product.prodName, prodAmount = Double.Parse(value.Text), prodUnitType = "oz", prodTotal=(product.prodUnitPrice*Double.Parse(value.Text)).ToString("C")};
			ObservableCollection<CartListItem> pulled = Statics.Default.getCartItems();

			pulled.Add(cartItem);

			Statics.Default.setCartItems(pulled);

			Navigation.PopModalAsync();
		}
	}
}
