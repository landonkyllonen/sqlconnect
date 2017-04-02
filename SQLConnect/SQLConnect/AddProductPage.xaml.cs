using System;
using System.Net;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace SQLConnect
{
	public partial class AddProductPage : ContentPage
	{
		public AddProductPage()
		{
			InitializeComponent();

			string[] categories = new string[] { "Flowers", "Concentrates", "Edibles", "Glass", "Apparel" };
			foreach (string s in categories)
			{
				newCat.Items.Add(s);
			}
			string[] bulkTypes = new string[] { "None", "Linear", "Diminishing" };
			foreach (string s in bulkTypes)
			{
				newBulkType.Items.Add(s);
			}
		}

		async void saveNew(object s, EventArgs e)
		{
			var answer = await DisplayAlert("Add this Item", "Review your submission for accuracy. Are you sure you want to make this product available for sale with these values?", "Yes", "No");

			if (answer)
			{
				//Connect to url.
				var client = new System.Net.Http.HttpClient();

				//Show that we are waiting for a response and wait for it.

				var response = await client.GetAsync("http://cbd-online.net/landon/addOrEditProduct.php?" +
														 "operation=" + WebUtility.UrlEncode("0") +//0 add, 1 edit.
				                                         "&dispId=" + WebUtility.UrlEncode(Statics.Default.getCreds()[16]) +
														 "&name=" + WebUtility.UrlEncode(newName.Text) +
														 "&cat=" + WebUtility.UrlEncode(newCat.Items[newCat.SelectedIndex]) +
														 "&desc=" + WebUtility.UrlEncode(newDesc.Text) +
														 "&pic=" + WebUtility.UrlEncode("fix this") +
														 "&unitPrice=" + WebUtility.UrlEncode(newUnit.Text) +
														 "&incFlag=" + WebUtility.UrlEncode(newIncFlag.IsToggled.ToString()) +
														 "&incUnitPrice=" + WebUtility.UrlEncode(newIncUnit.Text) +
														 "&dealFlag=" + WebUtility.UrlEncode(newDealFlag.IsToggled.ToString()) +
														 "&discount=" + WebUtility.UrlEncode(newDiscount.Text) +
														 "&bulkType=" + WebUtility.UrlEncode(newBulkType.Items[newBulkType.SelectedIndex]) +
														 "&bulkDiscount=" + WebUtility.UrlEncode(newBulk.Text));

				var output = await response.Content.ReadAsStringAsync();

				string[] components = output.Split(new string[] { "\n" }, StringSplitOptions.None);

				if (components[0].Equals("true")){
					await DisplayAlert("Success", "Item values modified. You can see your edited inventory list by logging in again.", "Okay");
					//Change locally.
					ObservableCollection<ProductListItem> pulled = Statics.Default.getProducts();
					ProductListItem product = new ProductListItem();

					product.prodName = newName.Text;
					product.prodCategory = newCat.Items[newCat.SelectedIndex];
					product.prodDescription = newDesc.Text;
					//Change img
					product.prodUnitPrice = double.Parse(newUnit.Text);
					product.prodIncentiveFlag = newIncFlag.IsToggled;
					product.prodUnitPriceIncentive = double.Parse(newIncUnit.Text);
					product.prodDealFlag = newDealFlag.IsToggled;
					product.prodDiscount = double.Parse(newDiscount.Text);
					product.prodBulkType = newBulkType.SelectedIndex;
					product.prodBulkDiscount = double.Parse(newBulk.Text);

					pulled.Add(product);
					Statics.Default.setProducts(pulled);

					await Navigation.PopModalAsync();
				}
				else
				{
					await DisplayAlert("Error", "Sorry, there was a problem uploading your changes.", "Okay");
				}
			}

		}

		void cancelNew(object s, EventArgs e)
		{
			Navigation.PopModalAsync();
		}
	}
}
