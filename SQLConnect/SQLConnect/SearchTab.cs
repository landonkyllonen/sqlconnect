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
		Button back, search;
		Label console, lbl1, lbl2;
		string[] choices, piChoices, userChoices;

		ListView resultList;

		public SearchTab()
		{
			RelativeLayout holder = new RelativeLayout();

			lbl1 = new Label
			{
				Text = "What are you looking for?",
				TextColor = Color.Teal,
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center,
				FontSize=17
			};
			holder.Children.Add(lbl1, Constraint.Constant(0), Constraint.Constant(0),
			                    Constraint.RelativeToParent((parent) => { return parent.Width; }), Constraint.Constant(55));

			choices = new string[] {"Select", "Product Info", "Users" };

			piChoices = new string[] {"Select", "Most Popular", "Most Popular by Condition", "Specific Product" };

			userChoices = new string[] { "Select", "That have used...", "With condition..." };

			one = new Picker();
			foreach (string s in choices)
			{
				one.Items.Add(s);
			}
			one.SelectedIndex = 0;
			one.SelectedIndexChanged += oneSelected;
			holder.Children.Add(one, Constraint.RelativeToParent((parent) => { return parent.Width / 2 - 100; }), Constraint.Constant(55),
			                    Constraint.Constant(200), Constraint.Constant(40));

			two = new Picker { IsEnabled = false };
			two.SelectedIndexChanged += twoSelected;
			holder.Children.Add(two, Constraint.RelativeToParent((parent) => { return parent.Width / 2 - 100;}), Constraint.Constant(110),
								Constraint.Constant(200), Constraint.Constant(40));

			three = new Entry { IsEnabled = false };
			holder.Children.Add(three, Constraint.RelativeToParent((parent) => { return parent.Width / 2 - 100;}), Constraint.Constant(175),
								Constraint.Constant(200), Constraint.Constant(40));

			console = new Label
			{
				Text = "",
				TextColor = Color.Red,
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center
			};
			holder.Children.Add(console, Constraint.Constant(0), Constraint.Constant(230),
			                    Constraint.RelativeToParent((parent) => { return parent.Width;}), Constraint.Constant(40));

			search = new Button
			{
				Text = "Search",
				BackgroundColor = Color.Teal,
				TextColor = Color.White
			};
			search.Clicked += executeSearch;
			holder.Children.Add(search, Constraint.RelativeToParent((parent) => { return parent.Width / 2 - 75;}), Constraint.Constant(285),
								Constraint.Constant(150), Constraint.Constant(60));

			lbl2 = new Label
			{
				Text="Searching...",
				TextColor=Color.Teal,
				FontSize=24,
				HorizontalTextAlignment=TextAlignment.Center,
				VerticalTextAlignment=TextAlignment.Center,
				IsVisible=false
			};
			holder.Children.Add(lbl2, Constraint.Constant(0), Constraint.Constant(0),
			                    Constraint.RelativeToParent((parent) => { return parent.Width;}),Constraint.RelativeToParent((parent) => { return parent.Height*.8; }));

			back = new Button
			{
				Text = "Go Back",
				BackgroundColor = Color.Teal,
				TextColor = Color.White,
				IsVisible=false
			};
			back.Clicked += clearSearch;
			holder.Children.Add(back, Constraint.RelativeToParent((parent) => { return parent.Width / 2 - 75;}), Constraint.Constant(10),
								Constraint.Constant(150), Constraint.Constant(45));

			resultList = new ListView { 
				IsVisible=false
			};
			holder.Children.Add(resultList, Constraint.Constant(0), Constraint.Constant(160),
			                    Constraint.RelativeToParent((parent) => { return parent.Width;}), Constraint.RelativeToParent((parent) => { return parent.Height - 160; }));

			Content = holder;
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
					two.SelectedIndex = 0;
					two.IsEnabled = true;
					queryId = 1;
					break;
				case 2:
					foreach (string u in userChoices)
					{
						two.Items.Add(u);
					}
					two.SelectedIndex = 0;
					two.IsEnabled = true;
					queryId = 2;
					break;
				default:
					//None selected.
					two.IsEnabled = false;
					three.IsEnabled = false;
					break;
			}
			three.IsEnabled = false;
		}

		void twoSelected(object s, EventArgs e)
		{
			int indexOne = one.SelectedIndex;

			queryId = one.SelectedIndex + 10 * two.SelectedIndex;

			three.Text = "";

			switch (queryId)
			{
				case 11://Most Popular
						//Do nothing, wait for search to be pressed.
					three.IsEnabled = false;
					break;
				case 21://Most popular with condition...
					three.IsEnabled = true;
					//Need to collect more info.
					break;
				case 31://Specific Product...
					three.IsEnabled = true;
					//Need to collect more info.
					break;
				case 12://Users that have used med...
					three.IsEnabled = true;
					//Need to collect more info.
					break;
				case 22://Users with the condition...
					three.IsEnabled = true;
					//Need to collect more info.
					break;
				default:
					//selected nothing, do nothing.
					break;
			}
		}

		async void executeSearch(object s, EventArgs e)
		{
			console.Text = "";
			if (String.IsNullOrEmpty(three.Text) && queryId != 11)
			{
				console.Text = "You must enter something in the query!";
				return;
			}
			else {
				//Hide regular.
				lbl1.IsVisible = false;
				one.IsVisible = false;
				two.IsVisible = false;
				three.IsVisible = false;
				search.IsVisible = false;

				//Show searching.
				back.IsVisible = true;
				lbl2.IsVisible = true;

				string extraInfo = three.Text;

				//Connect to url.
				var client = new System.Net.Http.HttpClient();

				//Show that we are waiting for a response and wait for it.

				var response = await client.GetAsync("http://cbd-online.net/landon/medicalSearch.php?" +
				                                     "queryId=" + System.Net.WebUtility.UrlEncode(queryId.ToString()) +
				                                     "&extraInfo=" + System.Net.WebUtility.UrlEncode(extraInfo));

				var output = await response.Content.ReadAsStringAsync();

			}
		}

		void clearSearch(object s, EventArgs e)
		{
			queryId = 0;

			//Show regular.
			lbl1.IsVisible = true;
			one.IsVisible = true;
			one.SelectedIndex = 0;
			two.IsVisible = true;
			three.IsVisible = true;
			two.IsEnabled = false;
			three.IsEnabled = false;
			two.Items.Clear();
			three.Text = "";
			search.IsVisible = true;

			//hide searching & results
			back.IsVisible = false;
			lbl2.IsVisible = false;
			resultList.IsVisible = false;
		}
	}
}