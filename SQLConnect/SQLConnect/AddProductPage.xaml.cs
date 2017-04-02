using System;
using System.Net;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SQLConnect
{
	public partial class AddProductPage : ContentPage
	{
		public AddProductPage()
		{
			InitializeComponent();
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

			}

		}

		void cancelNew(object s, EventArgs e)
		{
			
		}
	}
}
