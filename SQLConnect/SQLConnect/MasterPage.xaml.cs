using System;

using Xamarin.Forms;

namespace SQLConnect
{
	public partial class MasterPage : MasterDetailPage
	{
		public MasterPage()
		{
			InitializeComponent();

			Statics.Default.setMaster(this);

			masterPage.ListView.ItemSelected += OnItemSelected;
		}

		void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			//indexSelected = (masterPage.ListView.ItemsSource as List<MasterPageItem>).IndexOf(e.SelectedItem as MasterPageItem);

			var item = e.SelectedItem as MasterPageItem;
			if (item != null)
			{
				Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
				masterPage.ListView.SelectedItem = null;
				IsPresented = false;
			}
		}
	}
}
