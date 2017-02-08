﻿using Xamarin.Forms;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.ObjectModel;
using CryptSharp;
using CryptSharp.Utility;

namespace SQLConnect
{
	public partial class SQLConnectPage : ContentPage
	{
		string[] credentials;
		string user;
			
		public SQLConnectPage()
		{
			InitializeComponent();

			userentry.SetValue(Entry.TextProperty, "l");
			passentry.SetValue(Entry.TextProperty, "l");
			rememberSwitch.SetValue(Switch.IsToggledProperty, true);
		}

		public async void logIn(object s, EventArgs e){
			//Log in operation
			console.SetValue(Label.TextProperty, "Connecting to server...");
			//Get info for submit.
			user = userentry.Text;
			string pass = passentry.Text;
			DateTime date = DateTime.Today;
			//Replace this with a functional cross-platform device id.
			string id = "placeholder";

			//Connect to url.
			var client = new System.Net.Http.HttpClient();

			//Show that we are waiting for a response and wait for it.

			var response = await client.GetAsync("http://cbd-online.net/landon/logInandgetCreds.php?" +
			                                     "user=" + UrlEncodeParameter(user) +
			                                     "&pass=" + UrlEncodeParameter(pass) +
			                                     "&date=" + UrlEncodeParameter(date.ToString()) +
			                                     "&id=" + UrlEncodeParameter(id));

			var output = await response.Content.ReadAsStringAsync();

			//Process the output.
			string[] words = output.Split(new string[]{";;"}, StringSplitOptions.None);

			client.Dispose();

			string determinant = words[0];

			//If login success, parse credentials
			if (determinant.Equals("True"))
			{
				/*$firstname; ;$lastname; ;$email; ;$phone; ;$body; ;$blood; ;$energy; ;$ctype; ;$ancestry; ;$condInfo; 
				 ;$medInfo; ;$auth; ;$authorized; ;$admin; ;$reviewed; ;$userID; ;$dispensary"*/
				credentials = new string[17];

				for (int i = 1; i < words.Length; i++)
				{
					credentials[i-1] = words[i];
				}

				console.SetValue(Label.TextProperty, "Success!");

				//Update static vars
				Statics.Default.setCreds(credentials);
				Statics.Default.setUser(user);

				/*if (credentials[16].Equals(""))
				{
					await Navigation.PushModalAsync(new DispensaryPage());
				}
				else {
					await Navigation.PushModalAsync(new MasterPage());
				}*/

				await asyncLoadProducts(credentials[16]);

				await asyncLoadMessages(user);

				await Navigation.PushModalAsync(new DispensaryPage());

			}
			else {//Show error
				console.SetValue(Label.TextProperty, determinant);
			}
		}

		public async void offlineLogIn(object s, EventArgs e) {
			//Log in offline
			await Navigation.PushModalAsync(new SQLConnectPage());
		}

		async void register(object s, EventArgs e) {
			//Go to register page
			await Navigation.PushModalAsync(new RegisterPage());
		}

		async Task asyncLoadProducts(string dispId)
		{
			Debug.WriteLine(dispId);

			Product[] products;

			//Connect to url.
			var client = new System.Net.Http.HttpClient();

			//Show that we are waiting for a response and wait for it.

			var response = await client.GetAsync("http://cbd-online.net/landon/getItemInfo.php?" +
			                                     "dispId=" + UrlEncodeParameter(dispId));

			var output = await response.Content.ReadAsStringAsync();

			//Process the output.
			string[] productObjects = output.Split(new string[] { ";;" }, StringSplitOptions.None);

			//Separate into product components and turn into product objects.
			//bound as name--category--description--imageurl--incrementtype--baseprice--incbaseprice--dealdiscount--dealflag-incflag;;
			products = new Product[productObjects.Length];
			int index = 0;
			foreach (string obj in productObjects)
			{
				string[] productComponents = obj.Split(new string[] { "--" }, StringSplitOptions.None);
				Debug.WriteLine(obj);

				string comps = "";
				foreach (string s in productComponents)
				{
					comps = comps + " " + s;
				}
				Debug.WriteLine(comps);
 
				bool deal = false;
				bool incentive = false;
				if (productComponents[8].Equals("1")) { deal = true; }
				if (productComponents[9].Equals("1")) { incentive = true; }
				products[index] = new Product(productComponents[0], productComponents[1], productComponents[2], productComponents[4],
				                          double.Parse(productComponents[5]), productComponents[3], double.Parse(productComponents[7]), 
				                          double.Parse(productComponents[6]), deal, incentive);
				index++;
			}

			//products now contains all the products loaded from a certain dispensary, save to static for use in deal on front page,
			//as well as for lists.

			Statics.Default.setProducts(products);
		}

		async Task asyncLoadMessages(string user)
		{
			ObservableCollection<MessageListItem> messages = new ObservableCollection<MessageListItem>();

			//Connect to url.
			var client = new System.Net.Http.HttpClient();

			//Show that we are waiting for a response and wait for it.

			var response = await client.GetAsync("http://cbd-online.net/landon/acquireMessages.php?" +
												 "user=" + UrlEncodeParameter(user));

			var output = await response.Content.ReadAsStringAsync();

			//Process the output.
			string[] messageObjects = output.Split(new string[] { ";;" }, StringSplitOptions.None);

			//Separate into components and turn into objects.
			//bound as $title--$msg--$date--$viewed--$from--$id;;
			foreach (string obj in messageObjects)
			{
				string[] messageComponents = obj.Split(new string[] { "--" }, StringSplitOptions.None);
				Debug.WriteLine(obj);

				string comps = "";
				foreach (string s in messageComponents)
				{
					comps = comps + " " + s;
				}
				Debug.WriteLine(comps);

				bool viewed = false;
				if (messageComponents[3].Equals("1")) { viewed = true; }
				messages.Add(new MessageListItem{msgId = int.Parse(messageComponents[5]), msgContent = messageComponents[1],
					msgDate = messageComponents[2], msgFrom = messageComponents[4], msgTitle = messageComponents[0], msgViewed = viewed});
			}

			//messages now contains all the messages for this user that are not deleted, 

			Statics.Default.setMessages(messages);
		}

		public static string UrlEncodeParameter(string paramToEncode)
		{
			/*string urlEncodedParam = string.Empty;

			// remove whitespace from search parameter and URL encode it
			urlEncodedParam = paramToEncode.Trim();
			urlEncodedParam = Uri.EscapeDataString(urlEncodedParam);*/

			return paramToEncode;
		}
	}
}
