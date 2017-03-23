using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SQLConnect
{
	public partial class ProductPage : ContentPage
	{
		ProductListItem product;

		string[] medicalAmounts;
		double[] medicalPrices;
		int discountType;
		double bulkDiscountRate;
		int index;

		public ProductPage()
		{
			InitializeComponent();

			product = Statics.Default.getProdClicked();

			Title = product.prodName;
			image.Source = product.prodImgUrl;

			medicalAmounts = new string[] { "Gram", "Eighth\n(~3.5g)", "Quarter\n(~7g)", "Half Oz\n(~14g)", "Ounce\n(~28g)" };
			//Discount?
			discountType = product.prodBulkType;
			bulkDiscountRate = 1-product.prodBulkDiscount;
			//Populate price list for flowers.
			//If discount available, it is applied iteratively to amounts greater than a gram. This value could be set by the dispensary, maybe for each item?
			//1 gram is base price
			switch (discountType)
			{
				case 0:
					//No discount.
					medicalPrices = new double[5];
					medicalPrices[0] = product.prodUnitPrice;
					medicalPrices[1] = 3.54688 * product.prodUnitPrice;
					medicalPrices[2] = 3.54688 * 2 * product.prodUnitPrice;
					medicalPrices[3] = 3.54688 * 2 * 2 * product.prodUnitPrice;
					medicalPrices[4] = 3.54688 * 2 * 2 * 2 * product.prodUnitPrice;
					break;
				case 1:
					//eighth is 3.54688 grams with a 5% discount
					//quarter is 2 eighths price with another 5% discount
					//half oz is 2 quarters price with another 5% discount
					medicalPrices = new double[5];
					medicalPrices[0] = product.prodUnitPrice;
					medicalPrices[1] = 3.54688 * product.prodUnitPrice * bulkDiscountRate;
					medicalPrices[2] = 3.54688 * 2 * product.prodUnitPrice * (bulkDiscountRate-.05);
					medicalPrices[3] = 3.54688 * 2 * 2 * product.prodUnitPrice * (bulkDiscountRate-.1);
					medicalPrices[4] = 3.54688 * 2 * 2 * 2 * product.prodUnitPrice * (bulkDiscountRate-.15);
					break;
				case 2:
					//each step up gives half the discount of the previous step.
					medicalPrices = new double[5];
					medicalPrices[0] = product.prodUnitPrice;
					medicalPrices[1] = 3.54688 * product.prodUnitPrice * bulkDiscountRate;
					medicalPrices[2] = 3.54688 * 2 * product.prodUnitPrice * (bulkDiscountRate - .025);
					medicalPrices[3] = 3.54688 * 2 * 2 * product.prodUnitPrice * (bulkDiscountRate - .0375);
					medicalPrices[4] = 3.54688 * 2 * 2 * 2 * product.prodUnitPrice * (bulkDiscountRate - .04375);
					break;
				default:
					//No discount.
					medicalPrices = new double[5];
					medicalPrices[0] = product.prodUnitPrice;
					medicalPrices[1] = 3.54688 * product.prodUnitPrice;
					medicalPrices[2] = 3.54688 * 2 * product.prodUnitPrice;
					medicalPrices[3] = 3.54688 * 2 * 2 * product.prodUnitPrice;
					medicalPrices[4] = 3.54688 * 2 * 2 * 2 * product.prodUnitPrice;
					break;
			}

			index = 0;
			priceExact.Text = medicalPrices[0].ToString("C");

			//Get information on what type of choices to display.
			//For now, display simple.

			if (product.prodIncrementType.Equals("Flower"))
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
			s.ToString();
			e.ToString();
			if (int.Parse(value.Text) > 9) { return; }
			else {
				value.Text = (int.Parse(value.Text) + 1).ToString();
				price.Text = (int.Parse(value.Text) * product.prodUnitPrice).ToString("C");
			}
		}

		void decrement(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();
			if (int.Parse(value.Text) < 2) { return; }
			else {
				value.Text = (int.Parse(value.Text) - 1).ToString();
				price.Text = (int.Parse(value.Text) * product.prodUnitPrice).ToString("C");
			}
		}

		void previous(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();
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
			s.ToString();
			e.ToString();
			if (index == 4)
			{
				return;
			}
			else {
				index++;
				if (index == 4) { valueRight.Text = ""; }
				else {valueRight.Text = medicalAmounts[index + 1];}
				valueMid.Text = medicalAmounts[index];
				valueLeft.Text = medicalAmounts[index - 1];
				priceExact.Text = medicalPrices[index].ToString("C");
			}
		}

		async void addToCart(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();
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
				cartItem = new CartListItem { prodName = product.prodName, prodAmount = double.Parse(value.Text), prodUnitType = "", prodTotal = (product.prodUnitPrice * double.Parse(value.Text)).ToString("C") };
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
