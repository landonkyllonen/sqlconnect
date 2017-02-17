using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SQLConnect
{
	public partial class ConditionsPage : ContentPage
	{
		public ConditionsPage()
		{
			InitializeComponent();

			ObservableCollection<CondListItem> conds = Statics.Default.getConds();

			conds = new ObservableCollection<CondListItem>();

			conds.Add(new CondListItem {condName="Condition Example"});

			condList.ItemsSource = conds;
			condList.ItemTapped += onItemSelect;
		}

		void onItemSelect(object sender, ItemTappedEventArgs e)
		{
			//Display Edit/Delete options.
			return;
		}
	}
}
