using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SQLConnect
{
	public partial class ViewLogPage : ContentPage
	{
		public ViewLogPage()
		{
			InitializeComponent();

			LogListItem log = Statics.Default.getLogClicked();

			Title = log.logTitle;
			logDate.Text = log.logDate;
			logTextBox.Text = log.logText;
			impSwitch.IsToggled = log.logImportant;
			pubSwitch.IsToggled = log.logPublic;

			if (log.logMeds.Length > 0) { med1Lbl.Text = log.logMeds[0];}
			if (log.logMeds.Length > 1) { med2Lbl.Text = log.logMeds[1]; }
			if (log.logMeds.Length > 2) { med3Lbl.Text = log.logMeds[2]; }
		}
	}
}
