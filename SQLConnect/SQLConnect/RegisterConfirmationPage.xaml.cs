using System;

using Xamarin.Forms;

namespace SQLConnect
{
	public partial class RegisterConfirmationPage : ContentPage
	{
		public RegisterConfirmationPage()
		{
			InitializeComponent();
		}

		public async void toLogIn(object s, EventArgs e)
		{
			await Navigation.PushModalAsync(new SQLConnectPage());
		}
	}
}
