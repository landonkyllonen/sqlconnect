using System.Net.Http;
using System;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

namespace SQLConnect
{
	public partial class DispensaryManagementPage : ContentPage
	{
		Button suspendButton;
		PopupLayout pop;
		StackLayout popup;
		Entry popupInput;
		BoxView darken;

		public DispensaryManagementPage()
		{
			Title = "Dispensary Management";
			pop = new PopupLayout();

			Button addButton = new Button
			{
				Text = "Add Item to Inventory",
				TextColor = Color.White,
				BackgroundColor = Color.Teal
			};
			addButton.Clicked += toAddItem;

			Button editButton = new Button
			{
				Text = "Browse/Edit Inventory",
				TextColor = Color.White,
				BackgroundColor = Color.Teal
			};
			editButton.Clicked += toBrowseEdit;

			suspendButton = new Button
			{
				Text = "Suspend Transactions",
				TextColor = Color.White,
				BackgroundColor = Color.Red
			};
			suspendButton.Clicked += suspendTransactions;

			Label help = new Label
			{
				Text = "Note: If you have lost your code to re-enable transactions, or are having problems with any other managerial task please contact the support team.",
				TextColor=Color.Teal,
				VerticalOptions=LayoutOptions.Start,
				FontSize=16,
				HorizontalTextAlignment=TextAlignment.Center
			};

			darken = new BoxView
			{
				BackgroundColor = Color.Black,
				Opacity=0.3,
				IsVisible=false
			};

			RelativeLayout rel = new RelativeLayout
			{
			};
			rel.Children.Add(addButton,
			                 Constraint.RelativeToParent((parent)=> { return parent.Width / 2 - parent.Width * .2;}),
			                 Constraint.Constant(15),
			                 Constraint.RelativeToParent((parent) =>{ return parent.Width*.45;}),
			                 Constraint.Constant(50));
			rel.Children.Add(editButton,
							 Constraint.RelativeToParent((parent) => { return parent.Width / 2 - parent.Width * .2; }),
							 Constraint.Constant(80),
							 Constraint.RelativeToParent((parent) => { return parent.Width * .45; }),
							 Constraint.Constant(50));
			rel.Children.Add(suspendButton,
							 Constraint.RelativeToParent((parent) => { return parent.Width / 2 - parent.Width * .2; }),
							 Constraint.Constant(145),
							 Constraint.RelativeToParent((parent) => { return parent.Width * .45; }),
							 Constraint.Constant(50));
			rel.Children.Add(help,
			                 Constraint.Constant(15),
							 Constraint.Constant(210),
			                 Constraint.RelativeToParent((parent) => { return parent.Width-30; }),
							 Constraint.Constant(80));
			rel.Children.Add(darken,
							 Constraint.Constant(0),
							 Constraint.Constant(0),
							 Constraint.RelativeToParent((parent) => { return parent.Width; }),
							 Constraint.RelativeToParent((parent) => { return parent.Height; }));

			Label popupPrompt = new Label
			{
				Text="Enter your code:",
				FontSize=18,
				TextColor=Color.Black,
				HorizontalTextAlignment=TextAlignment.Center
			};

			popupInput = new Entry
			{
				TextColor=Color.Black,
				WidthRequest=200,
				FontSize=20,
				HorizontalOptions=LayoutOptions.Center
			};
			popupInput.TextChanged += autoCaps;

			Button yes = new Button
			{
				TextColor = Color.Teal,
				Text = "Enter",
				HorizontalOptions= LayoutOptions.FillAndExpand
			};
			yes.Clicked += submitCode;

			Button no = new Button
			{
				TextColor = Color.Teal,
				Text = "Cancel",
				HorizontalOptions = LayoutOptions.FillAndExpand
			};
			no.Clicked += closePopup;

			StackLayout btnHolder = new StackLayout
			{
				Orientation=StackOrientation.Horizontal,
				HorizontalOptions=LayoutOptions.FillAndExpand
			};
			btnHolder.Children.Add(no);
			btnHolder.Children.Add(yes);

			popup = new StackLayout
			{
				Padding = new Thickness(5, 10, 5, 10),
				WidthRequest = 300, // Important, the Popup hast to have a size to be showed 
				Orientation = StackOrientation.Vertical,
				BackgroundColor=Color.White
			};
			popup.Children.Add(popupPrompt);
			popup.Children.Add(popupInput);
			popup.Children.Add(btnHolder);

			pop.Content = rel;
			Content = pop;

			//If suspended = 1, set btn text to Enable Transactions
			if (Statics.Default.getCreds()[17].Equals("1"))
			{
				suspendButton.Text = "Enable Transactions";
				suspendButton.BackgroundColor = Color.PaleGreen;
			}
		}

		void toAddItem(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();

			NavigationPage nav = new NavigationPage(new AddProductPage());
			NavigationPage.SetHasBackButton(nav, true);
			Navigation.PushModalAsync(nav);
		}

		void toBrowseEdit(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();

			NavigationPage nav = new NavigationPage(new ProductsTab());
			NavigationPage.SetHasBackButton(nav, true);
			Navigation.PushModalAsync(nav);
		}

		async void suspendTransactions(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();

			if (Statics.Default.getCreds()[17].Equals("1")){
				//Figure out how to open an input box.
				darken.IsVisible = true;
				pop.ShowPopup(popup, Constraint.Constant(20), Constraint.Constant(100));
				popupInput.Focus();
			}
			else
			{
				//Give manager an unlock code to write down, suspend transactions until code is entered or support is called.
				var answer = await DisplayAlert("Suspend Transactions", "Are you sure? No customer can purchase from this dispensary until this is disabled!", "I'm sure", "No");

				//Generate random code.

				string unlockCode = RandomString(6);

				if (answer)
				{
					suspendButton.Text = "Enable Transactions";
					suspendButton.BackgroundColor = Color.PaleGreen;

					//Connect to url.
					var client = new HttpClient();
					var content = new MultipartFormDataContent();
					content.Add(new StringContent(Statics.Default.getCreds()[16]), "dispId");
					content.Add(new StringContent("0"), "operation");
					content.Add(new StringContent(unlockCode), "code");

					//Show that we are waiting for a response and wait for it.

					var response = await client.PostAsync("http://cbd-online.net/landon/toggleTransactions.php", content);

					var output = await response.Content.ReadAsStringAsync();

					if (output.Equals("true\n")){
						await DisplayAlert("Transaction unlock code", "Write this code down to enable transactions again later:\n" + unlockCode, "Okay");

						//Update locally.
						string[] pulled = Statics.Default.getCreds();
						pulled[17] = "1";
						Statics.Default.setCreds(pulled);
					}
					else
					{
						await DisplayAlert("Error!", "Something went wrong whilst disabling transactions.", "Okay");
						Debug.WriteLine(output);
					}
				}
			}
		}

		void closePopup(object s, EventArgs e)
		{

			if (pop.IsPopupActive)
			{
				pop.DismissPopup();
			}
			darken.IsVisible = false;
		}

		async void submitCode(object s, EventArgs e)
		{
			//Get code and close dialog.
			string code = popupInput.Text;
			if (pop.IsPopupActive)
			{
				pop.DismissPopup();
			}
			darken.IsVisible = false;

			//Connect to url.
			var client = new HttpClient();
			var content = new MultipartFormDataContent();
			content.Add(new StringContent(Statics.Default.getCreds()[16]), "dispId");
			content.Add(new StringContent("1"), "operation");
			content.Add(new StringContent(code), "code");

			var response = await client.PostAsync("http://cbd-online.net/landon/toggleTransactions.php", content);

			var output = await response.Content.ReadAsStringAsync();

			if (output.Equals("true\n"))
			{
				suspendButton.Text = "Suspend Transactions";
				suspendButton.BackgroundColor = Color.Red;
				await DisplayAlert("Success", "Transactions have been enabled once again.", "Ok");

				//Update locally.
				string[] pulled = Statics.Default.getCreds();
				pulled[17] = "0";
				Statics.Default.setCreds(pulled);
			}
			else
			{
				await DisplayAlert("Failed", "The code you entered was not accepted.", "Ok");
				Debug.WriteLine(output);
			}
		}

		string RandomString(int length)
		{
			Random random = new Random();
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[random.Next(s.Length)]).ToArray());
		}

		void autoCaps(object s, TextChangedEventArgs e)
		{
			popupInput.Text = e.NewTextValue.ToUpper();
		}
	}
}
