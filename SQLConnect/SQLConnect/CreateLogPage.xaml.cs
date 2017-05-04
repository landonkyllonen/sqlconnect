using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using Xamarin.Forms;

namespace SQLConnect
{
	public partial class CreateLogPage : ContentPage
	{
		ObservableCollection<MedListItem> meds;
		ObservableCollection<SimpleListItem> conds;
		List<string> condsOriginal;

		string[] medNames;

		string[] feedbackPairs;

		public CreateLogPage()
		{
			InitializeComponent();

			logTextBox.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeSentence);

			feedbackPairs = Statics.Default.getCreds()[14].Split(new string[] { "##" }, StringSplitOptions.None);

			DateTime date = DateTime.Today;
			logDate.Text = date.ToString("d");

			meds = Statics.Default.getMeds();
			conds = Statics.Default.getConds();
			condsOriginal = new List<string>();

			medNames = new string[meds.Count];

			int counter = 0;
			foreach (MedListItem med in meds)
			{
				medNames[counter] = med.medName;
				counter++;
			}

			//Populate extra pickers.
			extraCond.Items.Add("None");
			condsOriginal.Add("None");
			foreach (SimpleListItem cond in conds)
			{
				extraCond.Items.Add(cond.labelName);
				condsOriginal.Add(cond.labelName);
			}

			extraMed.Items.Add("None");
			foreach (string med in medNames)
			{
				extraMed.Items.Add(med);
			}
			extraMed.SelectedIndexChanged += filterCondChoices;

			//Set meds if available.
			if (meds.Count > 0){med1Lbl.Text = medNames[0];}
			if (meds.Count > 1) { med2Lbl.Text = medNames[1]; }
			if (meds.Count > 2) { med3Lbl.Text = medNames[2]; }

			string[] quickEntries = {
				"I'm feeling excellent today.",
				"I'm feeling well today.",
				"I'm feeling a little off today.",
				"I'm feeling tired today.",
				"I'm feeling sick today.",
				"I'm in pain today."
			};
			foreach (string s in quickEntries)
			{
				quickPick.Items.Add(s);
			}
		}

		public void activatePicker(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();
			quickPick.Focus();
		}

		public void setQuickEntry(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();
			string selection = quickPick.Items[quickPick.SelectedIndex];
			logTextBox.Text = selection;
		}

		void filterCondChoices(object sender, EventArgs e)
		{
			//Add removed choices back.
			extraCond.Items.Clear();
			foreach (string c in condsOriginal)
			{
				extraCond.Items.Add(c);
			}
			//Set cond choice back to nothing.
			extraCond.SelectedIndex = 0;

			string medname = extraMed.Items[extraMed.SelectedIndex];

			List<string> associated = new List<string>();
			//Loop through the previous feedback pairs, find conds associated with this medname and remove from choices.
			foreach (string pair in feedbackPairs)
			{
				string[] medcondpair = pair.Split(new string[] { "--" }, StringSplitOptions.None);
				//will be of length 2, med--cond.
				if (medname.Equals(medcondpair[0]))
				{
					associated.Add(medcondpair[1]);
				}
			}

			//Now loop through cond picker and remove associated cond choices.
			foreach (string s in extraCond.Items)
			{
				if (associated.Contains(s)){extraCond.Items.Remove(s);}
			}
		}

		public void toggleFeedback(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();
			if (extraComponent.IsVisible){startExtraBtn.Text = "Start Feedback";}
			else {startExtraBtn.Text = "Cancel";}

			extraComponent.IsVisible = !extraComponent.IsVisible;
			finishBtn.IsVisible = !finishBtn.IsVisible;
		}

		public async void submitLog(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();
			//Check to see that values are given.
			if (string.IsNullOrEmpty(title.Text) || string.IsNullOrEmpty(logTextBox.Text))
			{
				//Give error.
				await DisplayAlert("Error", "No fields can be left blank. (Title and log entry)", "OK");
			}
			else {
				//Submit log to db.

				//Create Log string
				string pub = "0";
				string imp = "0";
				if (pubSwitch.IsToggled) { pub = "1"; }
				if (impSwitch.IsToggled) { imp = "1"; }

				string medstring = "";

				foreach (string medname in medNames)
				{
					medstring += medname + "--";
				}
				//Cut off last delims.
				medstring = medstring.Substring(0, medstring.Length - 2);


				string logstring = title.Text + ";;" + logDate.Text + ";;" + logTextBox.Text + ";;" + pub + ";;" + imp + ";;" + medstring;
				//Connect to url.
				var client = new HttpClient();

				var content = new MultipartFormDataContent();
				content.Add(new StringContent(Statics.Default.getUser()), "user");
				content.Add(new StringContent(logstring), "log");

				await client.PostAsync("http://cbd-online.net/landon/uploadOneLog.php", content);

				//var output = await response.Content.ReadAsStringAsync();

				//Add to local and update statics.
				ObservableCollection<LogListItem> pulled = Statics.Default.getLogs();
				pulled.Add(new LogListItem { logTitle = title.Text, logDate = logDate.Text, logText = logTextBox.Text, logImportant = boolEquivalent(imp), logPublic = boolEquivalent(pub), logMeds = medNames });
				Statics.Default.setLogs(pulled);

				await Navigation.PopModalAsync();
			}
		}

		public async void submitLogFeedback(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();
			//Check to see that values are given.
			if (string.IsNullOrEmpty(title.Text) || string.IsNullOrEmpty(logTextBox.Text)||extraMed.SelectedIndex==0||extraCond.SelectedIndex==0)
			{
				//Give error.
				await DisplayAlert("Error", "No fields can be left blank. Press cancel if you do no not want to provide feedback.", "OK");
			}
			else {
				//Submit log to db.

				//Create Log string
				string pub = "0";
				string imp = "0";
				if (pubSwitch.IsToggled){pub = "1";}
				if (impSwitch.IsToggled){imp = "1";}

				string pos = "0";
				string neg = "0";
				if (posSwitch.IsToggled) { pos = "1"; }
				if (negSwitch.IsToggled) { neg = "1"; }

				string medstring = "";

				foreach (string medname in medNames)
				{
					medstring += medname + "--";
				}
				//Cut off last delims.
				medstring = medstring.Substring(0, medstring.Length - 2);


				string logstring = title.Text + ";;" + logDate.Text + ";;" + logTextBox.Text + ";;" + pub + ";;" + imp + ";;" + medstring;
				//Connect to url.
				var client = new HttpClient();

				var content = new MultipartFormDataContent();
				content.Add(new StringContent(Statics.Default.getUser()), "user");
				content.Add(new StringContent(logstring), "log");
				content.Add(new StringContent(Statics.Default.getCreds()[14]), "oldFeedback");
				content.Add(new StringContent(extraMed.Items[extraMed.SelectedIndex]), "med");
				content.Add(new StringContent(extraCond.Items[extraCond.SelectedIndex]), "cond");
				content.Add(new StringContent(pos), "pos");
				content.Add(new StringContent(neg), "neg");
				content.Add(new StringContent(negText.Text), "note");

				var response = await client.PostAsync("http://cbd-online.net/landon/uploadLogWithReview.php", content);

				await response.Content.ReadAsStringAsync();

				//Add to local and update statics.
				ObservableCollection<LogListItem> pulled = Statics.Default.getLogs();
				pulled.Add(new LogListItem { logTitle = title.Text, logDate = logDate.Text, logText = logTextBox.Text, logImportant = boolEquivalent(imp), logPublic = boolEquivalent(pub), logMeds = medNames });
				Statics.Default.setLogs(pulled);

				await Navigation.PopModalAsync();
			}
		}

		public static bool boolEquivalent(string zeroone)
		{
			if (zeroone.Equals("1")){
				return true;
			}
			    else if (zeroone.Equals("0")){
				return false;
			}
			else {
				System.Diagnostics.Debug.WriteLine("in boolEquivalent that was not a 0 or a 1!");
				return false;
			}
		}

		public void toggleUserClarification(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();
			negText.IsVisible = !negText.IsVisible;
		}
	}
}
