﻿using Xamarin.Forms;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Net.Http;

namespace SQLConnect
{
	public partial class SQLConnectPage : ContentPage
	{
		string[] credentials;
		string user;
			
		public SQLConnectPage()
		{
			Application.Current.MainPage = this;

			InitializeComponent();

			if (Application.Current.Properties.ContainsKey("dispLogo"))
			{
				var id = Application.Current.Properties["dispLogo"] as string;
				logo.Source = id;
			}
			else {
				logo.Source = "logo.png";
			}

			//Load values saved for restart.
			Statics.Default.deconstructAndLoad("cart");

			userentry.SetValue(Entry.TextProperty, "landon");
			userentry.Completed += (sender, e) => passentry.Focus();
			passentry.SetValue(Entry.TextProperty, "mypass");
			passentry.Completed += (sender, e) => logIn(sender,e);
			rememberSwitch.SetValue(Switch.IsToggledProperty, true);
		}

		public async void logIn(object s, EventArgs e){
			s.ToString();
			e.ToString();

			console.TextColor = Color.Red;

			//Set to online
			Statics.Default.setOffline(false);
			//Log in operation
			console.SetValue(Label.TextProperty, "Connecting to server...");

			//Set defaults
			Statics.Default.setCartItems(new ObservableCollection<CartListItem>());

			//Get info for submit.
			user = userentry.Text;
			string pass = passentry.Text;
			DateTime date = DateTime.Today;
			//Replace this with a functional cross-platform device id.
			string id = DependencyService.Get<IDeviceId>().getDeviceId();
			Debug.WriteLine("Your device id is: " + id);

			//Connect to url.
			var client = new HttpClient();

			var content = new MultipartFormDataContent();
			content.Add(new StringContent(user), "user");
			content.Add(new StringContent(pass), "pass");
			content.Add(new StringContent(date.ToString("d")), "date");
			content.Add(new StringContent(id), "id");

			var response = await client.PostAsync("http://cbd-online.net/landon/logInandgetCreds.php", content);

			var output = await response.Content.ReadAsStringAsync();

			//Process the output.
			string[] words = output.Split(new string[] { ";;" }, StringSplitOptions.None);

			client.Dispose();

			string determinant = words[0];

			//If login success, parse credentials
			if (determinant.Equals("True")){
				/*$firstname; ;$lastname; ;$email; ;$phone; ;$body; ;$blood; ;$energy; ;$ctype; ;$ancestry; ;$condInfo; 
				 ;$medInfo; ;$auth; ;$authorized; ;$admin; ;$reviewed; ;$userID; ;$dispensary"*/
				credentials = new string[18];

				for (int i = 1; i < words.Length; i++)
				{
					credentials[i - 1] = words[i];
				}

				//Update static vars
				Statics.Default.setCreds(credentials);
				Statics.Default.setUser(user);

				populateCondsMeds(credentials[9], credentials[10]);

				//await asyncLoadDispensaries();
				console.Text = "Loading Messages...";
				await asyncLoadMessages();
				console.Text = "Loading Dispensaries...";
				await asyncLoadDispensaries();
				console.Text = "Loading Products...";
				await asyncLoadProducts(credentials[16]);
				console.Text = "Loading Orders...";
				await asyncLoadOrders(user);
				console.Text = "Loading Logs...";
				await asyncLoadLogs(user);
				console.Text = "Loading Preferences...";
				await asyncLoadPrefs();
				console.TextColor = Color.Teal;
				console.Text = "Success!";

				//If user has no dispensary, display possible choices. Otherwise, go to home page.
				if (credentials[16].Equals(""))
				{
					await Navigation.PushModalAsync(new DispensaryPage());
				}
				else {
					await Navigation.PushModalAsync(new MasterPage());
				}

				//await Navigation.PushModalAsync(new DispensaryPage());
			}
			else {//Show error
				console.SetValue(Label.TextProperty, determinant);
			}
		}

		public async void offlineLogIn(object s, EventArgs e) {
			s.ToString();
			e.ToString();

			/*$firstname; ;$lastname; ;$email; ;$phone; ;$body; ;$blood; ;$energy; ;$ctype; ;$ancestry; ;$condInfo; 
				 ;$medInfo; ;$auth; ;$authorized; ;$admin; ;$reviewed; ;$userID; ;$dispensary"*/
			credentials = new string[17];
			credentials[0] = "Bob";
			credentials[1] = "Ross";
			credentials[2] = "BobRoss@trees.com";
			credentials[3] = "2505555555";
			credentials[4] = "Average";
			credentials[5] = "O+";
			credentials[6] = "Medium";
			credentials[7] = "N/A";
			credentials[8] = "Unknown";
			credentials[9] = "Chronic Tranquility--Painter's Wrist";
			credentials[10] = "Nature--9001mg--EveryGotDamn Day--Visual;;Anti-Drowsy--80mg--Biweekly--Oral";
			credentials[11] = "ABCDEFGHIJKLMNOP";
			credentials[12] = "1";
			credentials[13] = "1";
			credentials[14] = "";
			credentials[15] = "1";
			credentials[16] = "1";
			Statics.Default.setCreds(credentials);

			//Set to offline
			Statics.Default.setOffline(true);
			//Log in operation
			Statics.Default.setMessages(new ObservableCollection<MessageListItem>());
			Statics.Default.setProducts(new ObservableCollection<ProductListItem>());
			Statics.Default.setUser("");
			Statics.Default.setCartItems(new ObservableCollection<CartListItem>());
			Statics.Default.setOrders(new ObservableCollection<OrderListItem>());
			Statics.Default.setLogs(new ObservableCollection<LogListItem>());

			populateCondsMeds(credentials[9], credentials[10]);

			await Navigation.PushModalAsync(new DispensaryPage());
		}

		async void register(object s, EventArgs e) {
			if (s.ToString().Equals(e.ToString()))
			{
				Debug.WriteLine("Suppressing");
			}
			//Go to register page
			await Navigation.PushModalAsync(new RegisterPage());
		}

		async Task asyncLoadDispensaries()
		{
			ObservableCollection<DispListItem> dispensaries = new ObservableCollection<DispListItem>();

			//Connect to url.
			var client = new HttpClient();

			//Show that we are waiting for a response and wait for it.

			var response = await client.GetAsync("http://cbd-online.net/landon/getDispensaryList.php");

			var output = await response.Content.ReadAsStringAsync();

			//Process the output.
			string[] dispensaryObjects = output.Split(new string[] { ";;" }, StringSplitOptions.None);
			if (dispensaryObjects[0].Length < 2)
			{
				//empty, dont know why output.split would say there is one "" object.
				Debug.WriteLine("There was no dispensary info available.");
				return;
			}

			//bound as 
			foreach (string obj in dispensaryObjects)
			{
				string[] dispensaryComponents = obj.Split(new string[] { "--" }, StringSplitOptions.None);
				Debug.WriteLine(obj);

				string comps = "";
				foreach (string s in dispensaryComponents)
				{
					comps = comps + " " + s;
				}
				Debug.WriteLine(comps);

				dispensaries.Add(new DispListItem { dispName=dispensaryComponents[0], dispId=dispensaryComponents[1], dispCity=dispensaryComponents[2],
					dispAddress=dispensaryComponents[3], dispLogoPath=dispensaryComponents[4], dispImgPath=dispensaryComponents[5]});
			}

			//products now contains all the products loaded from a certain dispensary, save to static for use in deal on front page,
			//as well as for lists.

			Statics.Default.setDispensaries(dispensaries);
		}


		async Task asyncLoadProducts(string dispId)
		{
			Debug.WriteLine(dispId);

			ObservableCollection<ProductListItem> prods;

			//Connect to url.
			var client = new HttpClient();

			var content = new MultipartFormDataContent();
			content.Add(new StringContent(dispId), "dispId");

			var response = await client.PostAsync("http://cbd-online.net/landon/getItemInfo.php", content);


			var output = await response.Content.ReadAsStringAsync();

			//Process the output.
			string[] productObjects = output.Split(new string[] { ";;" }, StringSplitOptions.RemoveEmptyEntries);

			//Separate into product components and turn into product objects.
			prods = new ObservableCollection<ProductListItem>();
			foreach (string obj in productObjects)
			{
				string[] productComponents = obj.Split(new string[] { "--" }, StringSplitOptions.None);

				Debug.WriteLine(obj);
				//FOR DEBUGGING--
				string comps = "";
				foreach (string s in productComponents)
				{
					comps = comps + " " + s;
				}
				Debug.WriteLine(comps);
				//FOR DEBUGGING--
 
				//  0    1   2       3          4       5            6                7                8            9           10                11			12			13
				//Name, Id, Cat, Description, PicUrl, BasePrice, IncentiveFlag, IncentiveBasePrice, DealFlag, DealDiscount, BulkDiscountType, BulkDiscount, BulkInterval, BulkLimit
				bool deal = false;
				bool incentive = false;
				bool oos = false;
				if (productComponents[8].Equals("1")) { deal = true; }
				if (productComponents[6].Equals("1")) { incentive = true; }
				if (productComponents[14].Equals("1")) { oos = true; }

				var prod = new ProductListItem
				{
					prodName = productComponents[0],
					prodCategory = productComponents[2],
					prodDescription = productComponents[3],
					prodImgUrl = productComponents[4],
					prodUnitPrice = double.Parse(productComponents[5]),
					prodUnitPriceIncentive = double.Parse(productComponents[7]),
					prodDealFlag = deal,
					prodIncentiveFlag = incentive,
					prodOutofstock = oos,
					prodDiscount = double.Parse(productComponents[9]),
					prodBulkType = int.Parse(productComponents[10]),
					prodBulkDiscount = double.Parse(productComponents[11]),
					prodBulkInterval = int.Parse(productComponents[12]),
					prodBulkLimit = int.Parse(productComponents[13])
				};

				if (prod.prodDealFlag && !oos) { Statics.Default.setDeal(prod); }

				//bound as name--category--description--imageurl--incrementtype--baseprice--incbaseprice--dealdiscount--dealflag-incflag--bulkdis--bulkdistype;;
				prods.Add(prod);
			}

			//products now contains all the products loaded from a certain dispensary, save to static for use in deal on front page,
			//as well as for lists.

			Statics.Default.setProducts(prods);
		}

		async Task asyncLoadMessages()
		{
			ObservableCollection<MessageListItem> messages = new ObservableCollection<MessageListItem>();

			string auth = credentials[11];

			/*string tobecrypted = "l";
			Debug.WriteLine("About to test out some cryptology. " + tobecrypted);
			string salt = Crypter.Blowfish.GenerateSalt();
			Debug.WriteLine(salt);
			string whatisit = Crypter.Blowfish.Crypt(tobecrypted, salt);
			Debug.WriteLine(whatisit);
			if (Crypter.CheckPassword("l", whatisit))
			{
				Debug.WriteLine("salted crypted text is same as input text");
			}*/

			byte[] saltDefault = { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };

			string authHalf = Statics.Default.getAuthHalf();

			string complete = authHalf + auth;

			//Connect to url.
			var client = new HttpClient();

			var content = new MultipartFormDataContent();
			content.Add(new StringContent(Statics.Default.getUser()), "user");

			var response = await client.PostAsync("http://cbd-online.net/landon/acquireMessages.php", content);

			var output = await response.Content.ReadAsStringAsync();
			//Process the output.
			string[] messageObjects = output.Split(new string[] { ";;" }, StringSplitOptions.None);

			//If the split yields only 1 object with value "", return no messages.
			//Not sure why this value is retrieved in the first place.
			if (messageObjects[0].Equals(""))
			{
				Statics.Default.setMessages(messages);
				return;
			}
			//Separate into components and turn into objects.
			//bound as $title--$msg--$date--$viewed--$from--$id;;
			foreach (string obj in messageObjects)
			{
				string[] messageComponents = obj.Split(new string[] { "--" }, StringSplitOptions.None);

				bool viewed = false;
				if (messageComponents[3].Equals("1")) { viewed = true; }
				messages.Add(new MessageListItem{msgId = int.Parse(messageComponents[5]), msgContent = Crypto.DecryptAes(Convert.FromBase64String(messageComponents[1]), complete, saltDefault),
					msgDate = messageComponents[2], msgFrom = messageComponents[4], msgTitle = Crypto.DecryptAes(Convert.FromBase64String(messageComponents[0]), complete, saltDefault), msgViewed = viewed});
			}

			//messages now contains all the messages for this user that are not deleted, 

			Statics.Default.setMessages(messages);
		}

		async Task asyncLoadOrders(string username)
		{
			ObservableCollection<OrderListItem> orders = new ObservableCollection<OrderListItem>();

			//Connect to url.
			var client = new HttpClient();

			var content = new MultipartFormDataContent();
			content.Add(new StringContent(username), "user");

			var response = await client.PostAsync("http://cbd-online.net/landon/importOrders.php", content);

			var output = await response.Content.ReadAsStringAsync();

			//Process the output.
			string[] orderObjects = output.Split(new string[] { "##" }, StringSplitOptions.None);

			//If the split yields only 1 object with value "", return no orders.
			//Not sure why this value is retrieved in the first place.
			if (orderObjects[0].Equals(""))
			{
				Statics.Default.setOrders(orders);
				return;
			}

			//Separate into components and turn into objects.
			//bound as $item--$item--$item...;;$date;;$id;;$price;;$paid;;$received##
			foreach (string obj in orderObjects)
			{
				string[] orderComponents = obj.Split(new string[] { ";;" }, StringSplitOptions.None);
				Debug.WriteLine(obj);

				string comps = "";
				foreach (string s in orderComponents)
				{
					comps = comps + " " + s;
				}
				Debug.WriteLine(comps);

				//All items will appear in the first component (before first ";;").
				string[] orderItems = orderComponents[0].Split(new string[] { "--" }, StringSplitOptions.None);

				ObservableCollection<CartListItem> thisOrderItems = new ObservableCollection<CartListItem>();

				//Check that format is correct, should be multiple of 5
				//item bound as: name--amount--unittype--total--rate
				for (int i = 0; i+5 < orderItems.Length&&(orderItems.Length%5<1); i+=5){
					double price = double.Parse(orderItems[i + 3]);
					thisOrderItems.Add(new CartListItem {prodName = orderItems[i], prodAmount = double.Parse(orderItems[i+1]), prodUnitType = orderItems[i+2],
						prodTotal = price.ToString("C"), prodRate = orderItems[i+4]});
				}

				//thisOrderitems is now populated, create the orderItem object
				orders.Add(new OrderListItem {orderDate = orderComponents[1], orderID = int.Parse(orderComponents[2]), orderTotal = orderComponents[3],
					orderPaymentStatus=orderComponents[4], orderCompletionStatus=orderComponents[5], orderItems = thisOrderItems});
			}

			Statics.Default.setOrders(orders);
		}

		async Task asyncLoadLogs(string username)
		{
			ObservableCollection<LogListItem> logs = new ObservableCollection<LogListItem>();

			//Connect to url.
			var client = new HttpClient();

			var content = new MultipartFormDataContent();
			content.Add(new StringContent(username), "user");

			var response = await client.PostAsync("http://cbd-online.net/landon/importLogs.php", content);

			var output = await response.Content.ReadAsStringAsync();

			//Process the output.
			string[] logObjects = output.Split(new string[] { "##" }, StringSplitOptions.None);

			//If the split yields only 1 object with value "", return no orders.
			//Not sure why this value is retrieved in the first place.
			if (logObjects[0].Equals(""))
			{
				Statics.Default.setLogs(logs);
				return;
			}

			//Separate into components and turn into objects.
			//bound as $ttl;;$date;;$txt;;$public;;$important;;$meds##
			//meds as med--med--med?
			foreach (string obj in logObjects)
			{
				string[] logComponents = obj.Split(new string[] { ";;" }, StringSplitOptions.None);
				Debug.WriteLine(obj);

				string comps = "";
				foreach (string s in logComponents)
				{
					comps = comps + " " + s;
				}
				Debug.WriteLine(comps);

				//Get boolean values from strings
				bool pub = false;
				bool imp = false;
				if (logComponents[3].Equals("1")){
					pub = true;
				}
				if (logComponents[4].Equals("1"))
				{
					imp = true;
				}
				//extract med items for this log
				string[] logMedItems = logComponents[5].Split(new string[] { "--" }, StringSplitOptions.None);

				//thisOrderitems is now populated, create the orderItem object
				logs.Add(new LogListItem
				{
					logTitle = logComponents[0],
					logDate = logComponents[1],
					logText = logComponents[2],
					logPublic = pub,
					logImportant = imp,
					logMeds = logMedItems
				});
			}

			Statics.Default.setLogs(logs);
		}

		async Task asyncLoadPrefs() {
			//Connect to url.
			var client = new HttpClient();

			var content = new MultipartFormDataContent();
			content.Add(new StringContent(Statics.Default.getUser()), "user");

			var response = await client.PostAsync("http://cbd-online.net/landon/getPrefs.php", content);

			var output = await response.Content.ReadAsStringAsync();
			//Response as Appear;;Block;;Contacts1--contacts2...;;Blacklist1--blacklist2

			string[] components = output.Split(new string[] { ";;" }, StringSplitOptions.None);

			if (components.Length > 1)
			{
				Statics.Default.setAppearInSearch(int.Parse(components[0]));
				Statics.Default.setBlockNonContacts(int.Parse(components[1]));

				string[] cComps = components[3].Split(new string[] { "--" }, StringSplitOptions.None);
				ObservableCollection<SimpleListItem> contacts = new ObservableCollection<SimpleListItem>();
				if (cComps[0].Equals(""))
				{
					Statics.Default.setContacts(contacts);
				}
				else {
					foreach (string contact in cComps)
					{
						contacts.Add(new SimpleListItem { labelName = contact });
					}
					//Set static
					Statics.Default.setContacts(contacts);
				}

				string[] blComps = components[3].Split(new string[] { "--" }, StringSplitOptions.None);
				ObservableCollection<SimpleListItem> blacklist = new ObservableCollection<SimpleListItem>();
				if (blComps[0].Equals(""))//If none found, set static to empty list.
				{
					Statics.Default.setBlacklist(blacklist);
				}
				else {
					foreach (string blocked in blComps)
					{
						blacklist.Add(new SimpleListItem { labelName = blocked });
					}
					//Set static
					Statics.Default.setBlacklist(blacklist);
				}
			}
			else {
				Debug.WriteLine("Problem importing user preferences.");
			}
		}

		private void populateCondsMeds(string conds, string meds)
		{
			//First conditions, split into undelimited condition names, then set the global variable for future use.
			string[] condsSeparated = conds.Split(new string[] { "--" }, StringSplitOptions.None);
			ObservableCollection<SimpleListItem> conditions = new ObservableCollection<SimpleListItem>();

			foreach (string name in condsSeparated)
			{
				conditions.Add(new SimpleListItem { labelName=name });
			}
			Statics.Default.setConds(conditions);

			//Repeat for medications, must split twice.
			string[] medObjects = meds.Split(new string[] { "##" }, StringSplitOptions.None);
			ObservableCollection<MedListItem> medications = new ObservableCollection<MedListItem>();

			foreach (string med in medObjects)
			{
				string[] medComponents = med.Split(new string[] { "--" }, StringSplitOptions.None);
				try
				{
					medications.Add(new MedListItem { medName = medComponents[0], medDose = medComponents[1], medFrequency = medComponents[2], medMethod = medComponents[3] });
				}
				catch (IndexOutOfRangeException e)
				{
					Debug.WriteLine("Medications received from server were not in proper format. w--x--y--z##");
					Debug.WriteLine(e.StackTrace);
				}
			}
			Statics.Default.setMeds(medications);
		}

		public static string UrlEncodeParameter(string paramToEncode)
		{
			return WebUtility.UrlEncode(paramToEncode);
		}

		public string getDeviceID()
		{
			//Platform-specific.
			return null;
		}
	}
}
