﻿using System;
using System.Linq;
using Xamarin.Forms;

namespace SQLConnect
{
	public partial class RegisterPage : ContentPage
	{
		private static Random random = new Random();

		public RegisterPage()
		{
			InitializeComponent();
		}

		public static string RandomString(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
			  .Select(s => s[random.Next(s.Length)]).ToArray());
		}

		public async void register(object s, EventArgs e)
		{
			s.ToString();
			e.ToString();
			//Reset console
			console.IsVisible = false;

			//Process input for errors
			string[] entries = new string[7];
			entries[0] = user.Text;
			entries[1] = pass.Text;
			entries[2] = first.Text;
			entries[3] = last.Text;
			entries[4] = email.Text;
			entries[5] = phone.Text;
			entries[6] = RandomString(9);//Auth

			//Check nulls
			for (int i = 0; i < entries.Length; i++)
			{
				if (entries[i] == null)
				{
					entries[i] = "";
				}
			}

			//Check empty
			for (int i = 0; i < entries.Length; i++)
			{
				if (entries[i].Length<1)
				{
					console.Text = "You missed an entry!";
					console.IsVisible = true;
					return;
				}
			}

			//Check user and pass
			if (entries[0].Length < 5) {
				console.Text = "Your username must be at least 4 characters.";
				console.IsVisible = true;
				return;
			}
			if (entries[1].Length < 5)
			{
				console.Text = "Your password must be at least 4 characters.";
				console.IsVisible = true;
				return;
			}
			if (!hasNumberAndLetter(entries[1]))
			{
				console.Text = "Your password must contain at least 1 letter and number.";
				console.IsVisible = true;
				return;
			}

			//Connect to url.
			var client = new System.Net.Http.HttpClient();

			//Show that we are waiting for a response and wait for it.

			var response = await client.GetAsync("http://cbd-online.net/landon/testregister.php?" +
			                                     "user=" + entries[0] +
												 "&pass=" + entries[1] +
												 "&first=" + entries[2] +
			                                     "&last=" +entries[3] +
			                                     "&phone=" +entries[4] +
			                                     "&email=" + entries[5]+
												 "&auth=" +entries[6]);

			var output = await response.Content.ReadAsStringAsync();

			//Process the output.
			//string[] words = output.Split(new string[] { ";;" }, StringSplitOptions.None);

			if (output.Equals("True"))
			{
				console.IsVisible = true;
				console.Text = "There was a problem on our end, sorry!";
			}
			else {
				//IMPLEMENT A REGISTER CONFIRMATION PAGE, ALSO FIX URL ENCODING ON ENTRIES.
				await Navigation.PushModalAsync(new RegisterConfirmationPage());
			}
		}
			   
		public static bool hasNumberAndLetter(string text){
			bool hasLetter = false; 
			bool hasDigit = false;
			char[] chars = text.ToCharArray();
			for (int i = 0; i < chars.Length && !(hasLetter && hasDigit); i++)
			{
				char c = chars[i];
				if (!hasLetter) hasLetter = char.IsLetter(c);
				if (!hasDigit) hasDigit = char.IsDigit(c);
			}
			return (hasLetter && hasDigit);
		}
	}
}
