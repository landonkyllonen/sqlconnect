using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SQLConnect
{
	public partial class CreateLogPage : ContentPage
	{
		ObservableCollection<MedListItem> meds;
		ObservableCollection<CondListItem> conds;

		string[] medNames;

		public CreateLogPage()
		{
			InitializeComponent();

			DateTime date = DateTime.Today;
			logDate.Text = date.ToString("d");

			meds = Statics.Default.getMeds();
			conds = Statics.Default.getConds();

			medNames = new string[meds.Count];

			int counter = 0;
			foreach (MedListItem med in meds)
			{
				medNames[counter] = med.medName;
				counter++;
			}

			//Populate extra pickers.
			foreach (CondListItem cond in conds)
			{
				extraCond.Items.Add(cond.condName);
			}
			foreach (string med in medNames)
			{
				extraMed.Items.Add(med);
			}

			//Set meds if available.
			if (meds.Count > 0){med1Lbl.Text = medNames[0];}
			if (meds.Count > 1) { med2Lbl.Text = medNames[1]; }
			if (meds.Count > 2) { med3Lbl.Text = medNames[2]; }

			string[] quickEntries = new string[]{
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
			quickPick.Focus();
		}

		public void setQuickEntry(object s, EventArgs e)
		{
			string selection = quickPick.Items[quickPick.SelectedIndex];
			logTextBox.Text = selection;
		}

		public void toggleFeedback(object s, EventArgs e)
		{
			if (extraComponent.IsVisible){startExtraBtn.Text = "Start Feedback";}
			else {startExtraBtn.Text = "Cancel";}

			extraComponent.IsVisible = !extraComponent.IsVisible;
			finishBtn.IsVisible = !finishBtn.IsVisible;
		}

		public async void submitLog(object s, EventArgs e)
		{
			//Check to see that values are given.
			if (String.IsNullOrEmpty(title.Text) || String.IsNullOrEmpty(logTextBox.Text))
			{
				//Give error.
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
				var client = new System.Net.Http.HttpClient();

				//Show that we are waiting for a response and wait for it.

				var response = await client.GetAsync("http://cbd-online.net/landon/uploadOneLog.php?" +
													 "user=" + System.Net.WebUtility.UrlEncode(Statics.Default.getUser()) +
													 "&log=" + System.Net.WebUtility.UrlEncode(logstring));

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
			//Check to see that values are given.
			if (String.IsNullOrEmpty(title.Text) || String.IsNullOrEmpty(logTextBox.Text)||extraMed.SelectedIndex==0||extraCond.SelectedIndex==0)
			{
				//Give error.
			}
			else {
				//Submit log to db.

				//Create Log string
				string pub = "0";
				string imp = "0";
				if (pubSwitch.IsToggled){pub = "1";}
				if (impSwitch.IsToggled){imp = "1";}

				string medstring = "";

				foreach (string medname in medNames)
				{
					medstring += medname + "--";
				}
				//Cut off last delims.
				medstring = medstring.Substring(0, medstring.Length - 2);


				string logstring = title.Text + ";;" + logDate.Text + ";;" + logTextBox.Text + ";;" + pub + ";;" + imp + ";;" + medstring;
				//Connect to url.
				var client = new System.Net.Http.HttpClient();

				//Show that we are waiting for a response and wait for it.

				var response = await client.GetAsync("http://cbd-online.net/landon/uploadLogWithReview.php?" +
				                                     "user=" + System.Net.WebUtility.UrlEncode(Statics.Default.getUser()) +
				                                     "&log=" + System.Net.WebUtility.UrlEncode(logstring));

				var output = await response.Content.ReadAsStringAsync();

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
			negText.IsVisible = !negText.IsVisible;
		}
	}
}
