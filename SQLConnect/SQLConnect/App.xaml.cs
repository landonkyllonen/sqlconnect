using Xamarin.Forms;
using System;

namespace SQLConnect
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = new SQLConnectPage();
		}

		protected override void OnStart()
		{
			// Handle when your app start.
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
