using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SQLConnect
{
	public partial class AddConditionPage : ContentPage
	{
		public AddConditionPage()
		{
			InitializeComponent();
		}

		public void addCondition(object s, EventArgs e)
		{
			//Check if condition is empty or already existing.
			if (String.IsNullOrEmpty(condNameEntry.Text) || Statics.Default.getCreds()[9].Contains(condNameEntry.Text))
			{
				console.Text = "Condition cannot be blank or already added.";
				return;
			}

			//Add the condition to current conditions and upload to db.
			ObservableCollection<CondListItem> condsListPulled = Statics.Default.getConds();
			string[] creds = Statics.Default.getCreds();

			CondListItem newCond = new CondListItem { condName=condNameEntry.Text };
			string appendCond = "--" + newCond;

			condsListPulled.Add(newCond);
			creds[9] = creds[9] + appendCond;

			Statics.Default.setConds(condsListPulled);
			Statics.Default.setCreds(creds);

			Navigation.PopModalAsync();
		}
	}
}
