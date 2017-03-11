using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SQLConnect
{
	public class SearchTab : ContentPage
	{
		Picker one, two;
		int queryId;
		Entry three;
		Label console;
		string[] choices, piChoices, userChoices;
		public SearchTab()
		{
			RelativeLayout holder = new RelativeLayout();

			Label lbl1 = new Label
			{
				Text = "What are you looking for?",
				TextColor = Color.Teal,
				HorizontalTextAlignment = TextAlignment.Center
			};

			choices = new string[] {"Select", "Product Info", "Users" };

			piChoices = new string[] {"Select", "Most Popular", "Most Popular by Condition", "Specific Product" };

			userChoices = new string[] { "Select", "That have used...", "With condition..." };

			one = new Picker();
			foreach (string s in choices)
			{
				one.Items.Add(s);
			}
			one.SelectedIndexChanged += oneSelected;

			two = new Picker { IsVisible = false };
			two.SelectedIndexChanged += twoSelected;

			three = new Entry { IsVisible = false };
			three.Completed += threeCompleted;

			console = new Label
			{
				Text = "",
				TextColor = Color.Teal,
				HorizontalTextAlignment = TextAlignment.Center
			};

			ListView resultList = new ListView();
		}

		void oneSelected(object s, EventArgs e)
		{
			two.Items.Clear();

			switch (one.SelectedIndex)
			{
				case 1:
					foreach (string p in piChoices)
					{
						two.Items.Add(p);
					}
					two.IsVisible = true;
					queryId = 1;
					break;
				case 2:
					foreach (string u in userChoices)
					{
						two.Items.Add(u);
					}
					two.IsVisible = true;
					queryId = 2;
					break;
				default:
					//None selected.
					two.IsVisible = false;
					three.IsVisible = false;
					break;
			}
			three.IsVisible = false;
		}

		void twoSelected(object s, EventArgs e)
		{
			int indexOne = one.SelectedIndex;

			queryId = queryId + 10 * two.SelectedIndex;

			three.Text = "";


			//ProductInfo search
			switch (queryId)
			{
				case 11://Most Popular
					//Currently only display products offered by home dispensary.
					ObservableCollection<ProductListItem> prods = Statics.Default.getProducts();

					break;
				case 21://Most popular with condition...
					three.IsVisible = true;
					//Need to collect more info.
					break;
				case 31://Specific Product...
					three.IsVisible = true;
					//Need to collect more info.
					break;
				case 12://Users that have used med...
					three.IsVisible = true;
					//Need to collect more info.
					break;
				case 22://Users with the condition...
					three.IsVisible = true;
					//Need to collect more info.
					break;
				default:
					//selected nothing, do nothing.
					break;
			}
		}

		void threeCompleted(object s, EventArgs e)
		{
			ObservableCollection<ProductListItem> prods;
			if (String.IsNullOrEmpty(three.Text))
			{
				console.TextColor = Color.Red;
				console.Text = "You must enter something in the query!";
				return;
			}
			else {
				console.TextColor = Color.Teal;
			}
			switch (queryId)
			{
				case 21://Most popular with condition...
					console.Text = "Searching...";
					//Currently only display products offered by home dispensary.
					prods = Statics.Default.getProducts();

					break;
				case 31://Specific Product...
					console.Text = "Searching...";
					//Currently only display products offered by home dispensary.
					prods = Statics.Default.getProducts();

					break;
				case 12://Users that have used med...
					console.Text = "Searching...";
					string medName = three.Text;
					break;
				case 22://Users with the condition...
					string condName = three.Text;
					console.Text = "Searching...";

					break;
				default:
					//This shouldnt happen.
					throw new NotImplementedException();
					break;
			}
		}
	}
}