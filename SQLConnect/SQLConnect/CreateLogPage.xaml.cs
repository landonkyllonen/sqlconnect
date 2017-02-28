using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SQLConnect
{
	public partial class CreateLogPage : ContentPage
	{
		public CreateLogPage()
		{
			InitializeComponent();

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

		public void submitLog(object s, EventArgs e)
		{
			//Check to see that values are given.
			if (String.IsNullOrEmpty(title.Text) || String.IsNullOrEmpty(logTextBox.Text))
			{
				//Give error.
			}
			else {
				//Submit log to db.
			}
		}

		public void submitLogFeedback(object s, EventArgs e)
		{
			//Check to see that values are given.
			if (String.IsNullOrEmpty(title.Text) || String.IsNullOrEmpty(logTextBox.Text)||extraMed.SelectedIndex==0||extraCond.SelectedIndex==0)
			{
				//Give error.
			}
			else {
				//Submit log to db.
			}
		}

		public void toggleUserClarification(object s, EventArgs e)
		{
			negText.IsVisible = !negText.IsVisible;
		}
	}
}
