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
			medNameEntry.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeSentence);
			medFreqEntry.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeSentence);
			medMethodEntry.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeSentence);
		}

		public async void addMedication(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();
			MedListItem newMed = new MedListItem { medName = medNameEntry.Text, medDose = medDoseEntry.Text, medFrequency = medFreqEntry.Text, medMethod = medMethodEntry.Text };
			//Check that fields are not empty and that medication does not already exist.
			if (string.IsNullOrEmpty(medNameEntry.Text) || string.IsNullOrEmpty(medDoseEntry.Text) || string.IsNullOrEmpty(medFreqEntry.Text) || string.IsNullOrEmpty(medMethodEntry.Text))
			{
				console.Text = "Fields cannot be blank.";
				return;
			}
			else if (Statics.Default.getMeds().Contains(newMed))
			{
				console.Text = "You are already taking this medication.";
				return;
			}else {
				//Connect to url.
				var client = new System.Net.Http.HttpClient();

				//Show that we are waiting for a response and wait for it.

				var response = await client.GetAsync("http://cbd-online.net/landon/addMedication.php?" +
													 "user=" + System.Net.WebUtility.UrlEncode(Statics.Default.getUser()) +
				                                     "&name=" + System.Net.WebUtility.UrlEncode(medNameEntry.Text) +
				                                     "&method=" + System.Net.WebUtility.UrlEncode(medMethodEntry.Text) +
				                                     "&dose=" + System.Net.WebUtility.UrlEncode(medDoseEntry.Text) +
				                                     "&freq=" + System.Net.WebUtility.UrlEncode(medFreqEntry.Text));

				var output = await response.Content.ReadAsStringAsync();
				//If successful, add the condition to current conditions.

				if (output.Equals("true"))
				{
					//Feedback.
				}

				//Add the medication to current medications and upload to db.
				ObservableCollection<MedListItem> medsPulled = Statics.Default.getMeds();
				string[] creds = Statics.Default.getCreds();

				string medsAppend = ";;" + newMed.medName + "--" + newMed.medDose + "--" + newMed.medFrequency + "--" + newMed.medMethod;

				medsPulled.Add(newMed);
				creds[10] = creds[10] + medsAppend;

				Statics.Default.setMeds(medsPulled);
				Statics.Default.setCreds(creds);

				await Navigation.PopModalAsync();
			}
		}

		public void toDose(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();
			medDoseEntry.Focus();
		}

		public void toFreq(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();
			medFreqEntry.Focus();
		}

		public void toMethod(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();
			medMethodEntry.Focus();
		}
	}
}
