using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SQLConnect
{
	public partial class AddMedicationPage : ContentPage
	{
		public AddMedicationPage()
		{
			InitializeComponent();
		}

		public void addMedication(object s, EventArgs e)
		{
			//Add the medication to current medications and upload to db.
			ObservableCollection<MedListItem> medsPulled = Statics.Default.getMeds();
			string[] creds = Statics.Default.getCreds();

			MedListItem newMed = new MedListItem { medName=medNameEntry.Text, medDose=medDoseEntry.Text, medFrequency=medFreqEntry.Text, medMethod=medMethodEntry.Text };
			string medsAppend = ";;" + newMed.medName + "--" + newMed.medDose + "--" + newMed.medFrequency + "--" + newMed.medMethod;

			medsPulled.Add(newMed);
			creds[10] = creds[10] + medsAppend;

			Statics.Default.setMeds(medsPulled);
			Statics.Default.setCreds(creds);

			Navigation.PopModalAsync();
		}
	}
}
