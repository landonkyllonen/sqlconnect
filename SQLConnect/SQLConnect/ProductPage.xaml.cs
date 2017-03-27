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
					//Linear discount progression, 1 oz being the maximum discount(specified amount)
					medicalPrices = new double[5];
					medicalPrices[0] = product.prodUnitPrice;//None
					medicalPrices[1] = 3.54688 * product.prodUnitPrice * (1-product.prodBulkDiscount*1/4);// fourth of discount
					medicalPrices[2] = 3.54688 * 2 * product.prodUnitPrice * (1 - product.prodBulkDiscount * 1 / 2);// half of discount
					medicalPrices[3] = 3.54688 * 2 * 2 * product.prodUnitPrice * (1 - product.prodBulkDiscount * 3 / 4);// three/fourths of discount
					medicalPrices[4] = 3.54688 * 2 * 2 * 2 * product.prodUnitPrice * (1 - product.prodBulkDiscount);// max discount
					break;
				case 2:
					//each step up gives half the discount of the previous step.
					medicalPrices = new double[5];
					medicalPrices[0] = product.prodUnitPrice;
					medicalPrices[1] = 3.54688 * product.prodUnitPrice * (1-(product.prodBulkDiscount/2));//Half discount
					medicalPrices[2] = 3.54688 * 2 * product.prodUnitPrice * (1 - (product.prodBulkDiscount* 3/4));//Fourth more
					medicalPrices[3] = 3.54688 * 2 * 2 * product.prodUnitPrice * (1 - (product.prodBulkDiscount*7/8));//Eighth more
					medicalPrices[4] = 3.54688 * 2 * 2 * 2 * product.prodUnitPrice * (1 - (product.prodBulkDiscount*15/16));//Sixteenth more
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
			priceExactRate.Text = "(" + medicalPrices[0].ToString("C") + "/g)";

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
				double grams = 0;
				switch (index)
				{
					case 0:
						grams = 1;
						break;
					case 1:
						grams = 3.54688;
						break;
					case 2:
						grams = 3.54688 * 2;
						break;
					case 3:
						grams = 3.54688 * 4;
						break;
					case 4:
						grams = 3.54688 * 8;
						break;
					default:
						break;
				}
				priceExact.Text = medicalPrices[index].ToString("C");
				priceExactRate.Text = "(" + (medicalPrices[index] / grams).ToString("C") + "/g)";
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
				double grams = 0;
				switch (index)
				{
					case 0:
						grams = 1;
						break;
					case 1:
						grams = 3.54688;
						break;
					case 2:
						grams = 3.54688 * 2;
						break;
					case 3:
						grams = 3.54688 * 4;
						break;
					case 4:
						grams = 3.54688 * 8;
						break;
					default:
						break;
				}
				priceExact.Text = medicalPrices[index].ToString("C");
				priceExactRate.Text = "(" + (medicalPrices[index] / grams).ToString("C") + "/g)";
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
				string unit;
				string rate;
				switch (index)
				{
					case 0:
						amount = (double)1;
						unit = "g";
						rate = "(" + (medicalPrices[0]).ToString("C") + "/g)";
						break;
					case 1:
						amount = (double)1 / 8;
						unit = "oz";
						rate = "("+(medicalPrices[1]/3.54688).ToString("C")+"/g)";
						break;
					case 2:
						amount = (double)1 / 4;
						unit = "oz";
						rate = "(" + (medicalPrices[2] / (3.54688*2)).ToString("C") + "/g)";
						break;
					case 3:
						amount = (double)1 / 2;
						unit = "oz";
						rate = "(" + (medicalPrices[3] / (3.54688 * 4)).ToString("C") + "/g)";
						break;
					default:
						amount = (double)1;
						unit = "oz";
						rate = "(" + (medicalPrices[4] / (3.54688 * 8)).ToString("C") + "/g)";
						break;
				}
				cartItem = new CartListItem { prodName = product.prodName, prodIsRegular=false, prodIsFlower=true, prodAmount = amount, prodRate = rate, prodUnitType = unit, prodTotal = medicalPrices[index].ToString("C")};
			}
			else {
				cartItem = new CartListItem { prodName = product.prodName, prodIsRegular=true, prodIsFlower = false, prodAmount = double.Parse(value.Text), prodUnitType = "", prodTotal = (product.prodUnitPrice * double.Parse(value.Text)).ToString("C") };
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
