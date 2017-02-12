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
			s.ToString();
			e.ToString();
			await Navigation.PushModalAsync(new SQLConnectPage());
		}
	}
}
