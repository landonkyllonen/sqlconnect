using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SQLConnect
{
	public partial class MedicationsPage : ContentPage
	{
		public MedicationsPage()
		{
			InitializeComponent();

			ObservableCollection<MedListItem> meds = Statics.Default.getMeds();

			//meds = new ObservableCollection<MedListItem>();
			medList.ItemsSource = meds;
			medList.ItemTapped += onItemSelect;
		}

		void onItemSelect(object sender, ItemTappedEventArgs e)
		{
			//Display Edit/Delete options.
			return;
		}

		void goToAddMedication(object sender, System.EventArgs e)
		{
			Navigation.PushModalAsync(new AddMedicationPage());
		}
	}
}
