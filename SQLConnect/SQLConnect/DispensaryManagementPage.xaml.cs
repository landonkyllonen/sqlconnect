using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SQLConnect
{
	public partial class DispensaryManagementPage : ContentPage
	{
		public DispensaryManagementPage()
		{
			InitializeComponent();

			//If suspended = 1, set btn text to Enable Transactions
			if (Statics.Default.getCreds()[17].Equals("1"))
			{
				suspendBtn.Text = "Enable Transactions";
			}
		}

		void toAddItem(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();

			NavigationPage nav = new NavigationPage(new AddProductPage());
			NavigationPage.SetHasBackButton(nav, true);
			Navigation.PushModalAsync(nav);
		}

		void toBrowseEdit(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();

			NavigationPage nav = new NavigationPage(new ProductsTab());
			NavigationPage.SetHasBackButton(nav, true);
			Navigation.PushModalAsync(nav);
		}

		async void suspendTransactions(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();

			if (Statics.Default.getCreds()[17].Equals("1")){
				//Figure out how to open an input box.
			}
			else
			{
				//Give manager an unlock code to write down, suspend transactions until code is entered or support is called.
				var answer = await DisplayAlert("Suspend Transactions", "Are you sure? No customer can purchase from this dispensary until this is disabled!", "I'm sure", "No");

				//Generate random code.
				Random random = new Random();
				byte[] buffer = new byte[6];
				random.NextBytes(buffer);
				string unlockCode = System.Text.Encoding.UTF8.GetString(buffer,0,6);

				if (answer)
				{
					suspendBtn.Text = "Enable Transactions";

					//Connect to url.
					var client = new HttpClient();
					var content = new MultipartFormDataContent();
					content.Add(new StringContent(Statics.Default.getCreds()[16]), "dispId");
					content.Add(new StringContent("0"), "operation");
					content.Add(new StringContent(unlockCode), "code");

					//Show that we are waiting for a response and wait for it.

					var response = await client.PostAsync("http://cbd-online.net/landon/toggleTransactions.php", content);

					var output = await response.Content.ReadAsStringAsync();

					await DisplayAlert("Transaction unlock code", "Write this code down to enable transactions again later: " + unlockCode, "Okay");
				}
			}
		}
	}
}
