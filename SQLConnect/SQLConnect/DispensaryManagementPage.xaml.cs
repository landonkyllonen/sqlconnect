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

			//If suspended = 1, prompt for code, else...

			//Give manager an unlock code to write down, suspend transactions until code is entered or support is called.
			var answer = await DisplayAlert("Suspend Transactions", "Are you sure? No customer can purchase from this dispensary until this is disabled!", "I'm sure", "No");

			//Generate random code.
			string unlockCode = "A6H327";

			if (answer)
			{
				await DisplayAlert("Transaction unlock code", "Write this code down to enable transactions again later: " + unlockCode, "Okay");
				suspendBtn.Text = "Enable Transactions";
			}
		}
	}
}
