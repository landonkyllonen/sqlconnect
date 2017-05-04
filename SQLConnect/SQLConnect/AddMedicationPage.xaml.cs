using System;
using System.Collections.ObjectModel;
using System.Net.Http;
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
				var client = new HttpClient();

				var content = new MultipartFormDataContent();
				content.Add(new StringContent(Statics.Default.getUser()), "user");
				content.Add(new StringContent(medNameEntry.Text), "name");
				content.Add(new StringContent(medMethodEntry.Text), "method");
				content.Add(new StringContent(medDoseEntry.Text), "dose");
				content.Add(new StringContent(medFreqEntry.Text), "freq");

				var response = await client.PostAsync("http://cbd-online.net/landon/addMedication.php", content);

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
