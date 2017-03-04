using Xamarin.Forms;
using System;

namespace SQLConnect
{
	public class ProfileTab : ContentPage
	{
		string[] credentials;
		Label email;
		Entry phone, ancestry;
		Picker bmi, blood, energy, cancer;
		int savedCancerIndex, savedBmiIndex, savedEnergyIndex, savedBloodIndex;
		string[] bodytypes, cancertypes, bloodtypes, energytypes;

		public ProfileTab()
		{
			//Get statics
			credentials = Statics.Default.getCreds();
			/*$firstname; ;$lastname; ;$email; ;$phone; ;$body; ;$blood; ;$energy; ;$ctype; ;$ancestry; ;$condInfo; 
				 ;$medInfo; ;$auth; ;$authorized; ;$admin; ;$reviewed; ;$userID; ;$dispensary"*/

			//Content separated into 3 components, all inside one stacklayout that will be defined last.
			StackLayout upperComponent = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.FromHex("#00b3b3"),
				Padding = new Thickness(10,0,10,10)
			};

			//Name portion
			Label name = new Label
			{
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Text = credentials[0] +" "+ credentials[1],
				TextColor = Color.White
			};

			StackLayout upperOne = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HeightRequest = 50
			};
			upperOne.Children.Add(new Label { Text = "Name", TextColor=Color.White, HorizontalOptions = LayoutOptions.Start,VerticalOptions = LayoutOptions.CenterAndExpand });
			upperOne.Children.Add(name);

			//Email portion
			email = new Label
			{
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Text = credentials[2],
				TextColor = Color.White
			};

			StackLayout upperTwo = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HeightRequest = 50
			};
			upperTwo.Children.Add(new Label { Text = "Email", TextColor = Color.White, HorizontalOptions = LayoutOptions.Start,VerticalOptions = LayoutOptions.CenterAndExpand });
			upperTwo.Children.Add(email);

			//Phone portion
			phone = new Entry
			{
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalTextAlignment = TextAlignment.Center,
				WidthRequest = 125,
				Text = credentials[3],
				TextColor = Color.White
			};

			StackLayout upperThree = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest=80,
				HeightRequest = 50
			};
			upperThree.Children.Add(new Label { Text = "Phone", TextColor = Color.White, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.CenterAndExpand });
			upperThree.Children.Add(phone);

			upperComponent.Children.Add(upperOne);
			upperComponent.Children.Add(upperTwo);
			upperComponent.Children.Add(upperThree);
			upperComponent.Children.Add(new Label
			{
				FontSize = 15,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Text = "*No changes, aside from conditions/medications will be saved until you press \"Save Profile\"*",
				TextColor = Color.FromHex("#cfe7df")
			});
			upperComponent.Children.Add(new Label
			{
				FontSize = 15,
				Text = "The following fields are OPTIONAL but helpful to complete.",
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				TextColor = Color.FromHex("#cfe7df")
			});

			//Controls for expanding midComponent
			StackLayout midControls = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Start,
				BackgroundColor = Color.Teal,
				Padding = new Thickness(10, 0, 10, 0)
			};

			Button expandMid = new Button
			{
				TextColor = Color.White,
				FontSize = 25,
				BackgroundColor=Color.Teal,
				Text = "+",
				HorizontalOptions = LayoutOptions.EndAndExpand
			};

			midControls.Children.Add(new Label
			{
				Text = "Press the icon to show/hide additional fields",
				TextColor = Color.White,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand
			});

			midControls.Children.Add(expandMid);

			//NEXT COMPONENT

			StackLayout midComponent = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.White,
				Padding = new Thickness(10,0,10,0),
				IsVisible=false
			};

			//First
			bmi = new Picker
			{
				HorizontalOptions = LayoutOptions.EndAndExpand,
				WidthRequest = 140,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				TextColor=Color.Black
			};
			//Configure types
			bodytypes = new string[]{"N/A", "Under-average", "Average", "Over-average"};
			savedBmiIndex = 0;
			for (int i = 0; i < bodytypes.Length; i++) {
				bmi.Items.Add(bodytypes[i]);
				if (credentials[4].Equals(bodytypes[i]))
				{
					savedBmiIndex = i;
				}
			}
			bmi.SetValue(Picker.SelectedIndexProperty, savedBmiIndex);

			Button helpBmi = new Button
			{
				Text = "?",
				TextColor = Color.White,
				BackgroundColor = Color.FromHex("#8C8984"),
				BorderRadius = 5
			};

			StackLayout midOne = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HeightRequest = 50
			};
			midOne.Children.Add(new Label { Text = "Body Type",TextColor = Color.Black, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.CenterAndExpand });
			midOne.Children.Add(bmi);
			midOne.Children.Add(helpBmi);

			//Second
			blood = new Picker
			{
				HorizontalOptions = LayoutOptions.EndAndExpand,
				WidthRequest=140,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				TextColor = Color.Black
			};
			//Configure
			bloodtypes = new string[]{ "N/A", "O+","O-","A+","A-","B+","B-","AB+","AB-"};
			savedBloodIndex = 0;
			for (int i = 0; i < bloodtypes.Length; i++)
			{
				blood.Items.Add(bloodtypes[i]);
				if (credentials[5].Equals(bloodtypes[i]))
				{
					savedBloodIndex = i;
				}
			}
			blood.SetValue(Picker.SelectedIndexProperty, savedBloodIndex);

			StackLayout midTwo = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HeightRequest = 50
			};
			midTwo.Children.Add(new Label { Text = "Blood Type",TextColor = Color.Black, HorizontalOptions = LayoutOptions.Start,VerticalOptions = LayoutOptions.CenterAndExpand });
			midTwo.Children.Add(blood);
			midTwo.Children.Add(new Button { Text = "", TextColor = Color.White, BackgroundColor=Color.White, IsEnabled=false});

			//Third
			energy = new Picker
			{
				HorizontalOptions = LayoutOptions.EndAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest=140,
				TextColor = Color.Black
			};
			//Configure
			energytypes = new string[]{"N/A", "Low", "Medium", "High"};
			savedEnergyIndex = 0;
			for (int i = 0; i < energytypes.Length; i++)
			{
				energy.Items.Add(energytypes[i]);
				if (credentials[6].Equals(energytypes[i]))
				{
					savedEnergyIndex = i;
				}
			}
			energy.SetValue(Picker.SelectedIndexProperty, savedEnergyIndex);

			Button helpEnergy = new Button
			{
				Text = "?",
				TextColor = Color.White,
				BackgroundColor = Color.FromHex("#8C8984"),
				BorderRadius=5
			};

			StackLayout midThree = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HeightRequest = 50
			};
			midThree.Children.Add(new Label { Text = "Energy Level",TextColor = Color.Black, HorizontalOptions = LayoutOptions.Start,VerticalOptions = LayoutOptions.CenterAndExpand });
			midThree.Children.Add(energy);
			midThree.Children.Add(helpEnergy);

			//Fourth
			cancer = new Picker
			{
				HorizontalOptions = LayoutOptions.EndAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest=140,
				TextColor = Color.Black
			};
			//Configure
			cancertypes = new string[]{"N/A", "None", "Skin Cancer", "Lung Cancer", "Breast Cancer", "Prostate Cancer", "Colorectal Cancer", "Bladder Cancer", "Melanoma", "Lymphoma", "Kidney Cancer","Leukemia", "Other"};
			savedCancerIndex = 0;
			for (int i = 0; i < cancertypes.Length; i++)
			{
				cancer.Items.Add(cancertypes[i]);
				if (credentials[7].Equals(cancertypes[i]))
				{
					savedCancerIndex = i;
				}
			}
			cancer.SetValue(Picker.SelectedIndexProperty, savedCancerIndex);

			Button helpCancer = new Button
			{
				Text = "?",
				TextColor = Color.White,
				BackgroundColor = Color.FromHex("#8C8984"),
				BorderRadius=5
			};

			StackLayout midFour = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HeightRequest = 50
			};
			midFour.Children.Add(new Label { Text = "C-Type",TextColor = Color.Black, HorizontalOptions = LayoutOptions.Start,VerticalOptions = LayoutOptions.CenterAndExpand });
			midFour.Children.Add(cancer);
			midFour.Children.Add(helpCancer);

			//Fifth
			ancestry = new Entry
			{
				Text=credentials[8],
				HorizontalOptions = LayoutOptions.EndAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest=140,
				TextColor = Color.Black
			};


			Button helpAncestry = new Button
			{
				Text = "?",
				TextColor=Color.White,
				BackgroundColor=Color.FromHex("#8C8984"),
				BorderRadius=5
			};

			StackLayout midFive = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HeightRequest = 50
			};
			midFive.Children.Add(new Label { Text = "Ancestry",TextColor = Color.Black, HorizontalOptions = LayoutOptions.Start,VerticalOptions = LayoutOptions.CenterAndExpand });
			midFive.Children.Add(ancestry);
			midFive.Children.Add(helpAncestry);

			//Sixth
			Button editConds = new Button
			{
				Text = "View/Edit",
				TextColor = Color.White,
				BackgroundColor = Color.Teal,
				HorizontalOptions = LayoutOptions.EndAndExpand
			};
			editConds.Clicked += changeConds;

			StackLayout midSix = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HeightRequest = 50
			};
			midSix.Children.Add(new Label { Text = "Conditions",TextColor = Color.Black, HorizontalOptions = LayoutOptions.Start,VerticalOptions = LayoutOptions.CenterAndExpand });
			midSix.Children.Add(editConds);

			//Sixth
			Button editMeds = new Button
			{
				Text = "View/Edit",
				TextColor = Color.White,
				BackgroundColor = Color.Teal,
				HorizontalOptions = LayoutOptions.EndAndExpand
			};
			editMeds.Clicked += changeMeds;

			StackLayout midSeven = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HeightRequest = 50
			};
			midSeven.Children.Add(new Label { Text = "Medications",TextColor = Color.Black, HorizontalOptions = LayoutOptions.Start,VerticalOptions = LayoutOptions.CenterAndExpand });
			midSeven.Children.Add(editMeds);
			//Create help tags for mid 1,3,4,5
			Label helpBmiTag = new Label
			{
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				TextColor = Color.Teal,
				FontSize = 15,
				HorizontalTextAlignment = TextAlignment.Center,
				Text = "Indicate whether you are over, under, or pretty close to what you would consider an average weight for someone of your height",
				IsVisible = false
			};
			Label helpEnergyTag = new Label
			{
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.Teal,
				FontSize = 15,
				Text = "Indicated whether you: Are often idle or mostly performing low intensity exercises (LOW), " +
					"Are often moving/regularly performing exercises that get your heart pumping (MEDIUM), " +
					"Are very active/regularly performing vigorous workouts (HIGH)",
				IsVisible = false
			};
			Label helpCancerTag = new Label
			{
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.Teal,
				FontSize = 15,
				Text = "Indicate, if applicable, what type of cancer you are treating, otherwise, leave blank",
				IsVisible = false
			};
			Label helpAncestryTag = new Label
			{
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.Teal,
				FontSize=15,
				Text = "Indicate, if known, your most dominant ancestral background (e.g. Irish)",
				IsVisible = false
			};
			//Add all parts to midcomponent
			midComponent.Children.Add(midOne);
			midComponent.Children.Add(helpBmiTag);
			midComponent.Children.Add(midTwo);
			midComponent.Children.Add(midThree);
			midComponent.Children.Add(helpEnergyTag);
			midComponent.Children.Add(midFour);
			midComponent.Children.Add(helpCancerTag);
			midComponent.Children.Add(midFive);
			midComponent.Children.Add(helpAncestryTag);
			midComponent.Children.Add(midSix);
			midComponent.Children.Add(midSeven);

			//Create main control buttons for whole page.
			Button save = new Button
			{
				Text = "Save Profile",
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				BackgroundColor = Color.Teal,
				TextColor = Color.White
			};
			save.Clicked += saveProfile;

			Button manageApprovals = new Button
			{
				Text = "Manage Approvals",
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				BackgroundColor = Color.Teal,
				TextColor = Color.White,
				IsVisible = false
			};

			//Now add everything to an outer stacklayout
			StackLayout outer = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.White,
				Spacing=0
			};

			outer.Children.Add(upperComponent);
			outer.Children.Add(midControls);
			outer.Children.Add(midComponent);
			outer.Children.Add(save);
			outer.Children.Add(manageApprovals);

			//Visibility Handlers

			//Additional info
			expandMid.Clicked += delegate
			{
				if (midComponent.IsVisible)
				{
					midComponent.SetValue(IsVisibleProperty, false);
					expandMid.SetValue(Button.TextProperty, "+");
				}
				else {
					midComponent.SetValue(IsVisibleProperty, true);
					expandMid.SetValue(Button.TextProperty, "-");
				}
			};

			//Help tags
			helpBmi.Clicked += delegate
			{
				if (helpBmiTag.IsVisible){helpBmiTag.SetValue(IsVisibleProperty, false);}
				else {helpBmiTag.SetValue(IsVisibleProperty, true);}
			};
			helpEnergy.Clicked += delegate
			{
				if (helpEnergyTag.IsVisible) { helpEnergyTag.SetValue(IsVisibleProperty, false); }
				else { helpEnergyTag.SetValue(IsVisibleProperty, true); }
			};
			helpCancer.Clicked += delegate
			{
				if (helpCancerTag.IsVisible) { helpCancerTag.SetValue(IsVisibleProperty, false); }
				else { helpCancerTag.SetValue(IsVisibleProperty, true); }
			};
			helpAncestry.Clicked += delegate
			{
				if (helpAncestryTag.IsVisible) { helpAncestryTag.SetValue(IsVisibleProperty, false); }
				else { helpAncestryTag.SetValue(IsVisibleProperty, true); }
			};

			Content = new ScrollView{Content = outer};
		}

		public async void saveProfile(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();

			//Connect to url.
			var client = new System.Net.Http.HttpClient();

			//Show that we are waiting for a response and wait for it.

			var response = await client.GetAsync("http://cbd-online.net/landon/updateDetails.php?" +
			                                     "email="+ email.Text +
			                                     "&phone=" + phone.Text +
			                                     "&body=" + bodytypes[savedBmiIndex] +
			                                     "&blood=" + bloodtypes[savedBloodIndex] +
			                                     "&ctype=" + cancertypes[savedCancerIndex] +
			                                     "&energy=" + energytypes[savedEnergyIndex] +
			                                     "&ancestry=" + ancestry.Text +
			                                     "&conds=" + credentials[9] + 
			                                     "&meds=" + credentials[10]);

			var output = await response.Content.ReadAsStringAsync();

			//If output says it was successful, then also change locally.
			/*$firstname; ;$lastname; ;$email; ;$phone; ;$body; ;$blood; ;$energy; ;$ctype; ;$ancestry; ;$condInfo; 
				 ;$medInfo; ;$auth; ;$authorized; ;$admin; ;$reviewed; ;$userID; ;$dispensary"*/
			if (output.Equals("True")){
				//Success
				credentials[2] = email.Text;
				credentials[3] = phone.Text;
				credentials[4] = bodytypes[savedBmiIndex];
				credentials[5] = bloodtypes[savedBloodIndex];
				credentials[6] = energytypes[savedEnergyIndex];
				credentials[7] = cancertypes[savedCancerIndex];
				credentials[8] = ancestry.Text;
				Statics.Default.setCreds(credentials);

				//Provide feedback.
			}
		}

		public void changeMeds(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();

			Navigation.PushModalAsync(new MedicationsPage());
		}

		public void changeConds(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();

			Navigation.PushModalAsync(new ConditionsPage());
		}
	}
}

