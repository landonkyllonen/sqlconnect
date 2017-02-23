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
			//Check that fields are not empty and that medication does not already exist.
			if (String.IsNullOrEmpty(medNameEntry.Text)|| String.IsNullOrEmpty(medDoseEntry.Text)|| String.IsNullOrEmpty(medFreqEntry.Text)|| String.IsNullOrEmpty(medMethodEntry.Text) || Statics.Default.getCreds()[10].Contains(medNameEntry.Text))
			{
				console.Text = "Fields cannot be blank or you are already taking this medication.";
				return;
			}

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

		public void toDose(object s, EventArgs e)
		{
			medDoseEntry.Focus();
		}

		public void toFreq(object s, EventArgs e)
		{
			medFreqEntry.Focus();
		}

		public void toMethod(object s, EventArgs e)
		{
			medMethodEntry.Focus();
		}
	}
}
