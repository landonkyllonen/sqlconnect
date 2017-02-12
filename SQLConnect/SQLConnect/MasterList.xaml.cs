using System.Collections.Generic;

using Xamarin.Forms;

namespace SQLConnect
{
	public partial class MasterList : ContentPage
	{
		public ListView ListView { get { return listView; } }

		public MasterList()
		{
			InitializeComponent();

			var masterPageItems = new List<MasterPageItem>();
			masterPageItems.Add(new MasterPageItem
			{
				Title = "Social",
				IconSource = "user.png",
				TargetType = typeof(SocialPage)
			});
			masterPageItems.Add(new MasterPageItem
			{
				Title = "Products",
				IconSource = "cart.png",
				TargetType = typeof(ProductsPage)
			});
			masterPageItems.Add(new MasterPageItem
			{
				Title = "Analytics",
				IconSource = "polyline.png",
				TargetType = typeof(AnalyticsPage)
			});
			masterPageItems.Add(new MasterPageItem
			{
				Title = "Change Dispensary",
				IconSource = "store.png",
				TargetType = typeof(DispensaryPage)
			});
			masterPageItems.Add(new MasterPageItem
			{
				Title = "Settings",
				IconSource = "settings.png",
				TargetType = typeof(SettingsPage)
			});

			listView.ItemsSource = masterPageItems;
		}
	}
}
