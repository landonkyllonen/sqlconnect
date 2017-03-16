using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SQLConnect
{
	public class Statics
	{
		bool offline = true;

		//Profile page, etc
		string[] credentials;
		string user;
		string authHalf = "GFEDCBA";
		ObservableCollection<MessageListItem> messages;
		ObservableCollection<SimpleListItem> conds;
		ObservableCollection<MedListItem> meds;


		//Products Tab
		ObservableCollection<ProductListItem> products; //Dispensary specific item info, only has url for pic to be downloaded.
		ObservableCollection<CartListItem> cartItems;
		ObservableCollection<OrderListItem> orders;

		//Analytics Tab
		ObservableCollection<LogListItem> logs;

		//Dispensary List
		ObservableCollection<DispListItem> dispensaries;

		//Settings
		int appearInSearch, blockNonContacts;
		ObservableCollection<SimpleListItem> blacklist;

		//To manipulate navigation
		MasterPage master;

		//Passing ListClicked Info to other pages
		MessageListItem msgClicked;
		ProductListItem prodClicked;
		OrderListItem orderClicked;
		LogListItem logClicked;
		string catClicked;
		List<ProductListItem> catClickedContents;

		// this is the default static instance you'd use like string text = Settings.Default.SomeSetting;
		public readonly static Statics Default = new Statics();

		public void setOffline(bool state)
		{
			offline = state;
		}
		public bool isOffline()
		{
			return offline;
		}

		public void setCreds(string[] credentials)
		{
			this.credentials = credentials;
		}
		public string[] getCreds()
		{
			return credentials;
		}

		public void setUser(string user)
		{
			this.user = user;
		}
		public string getUser()
		{
			return user;
		}

		public string getAuthHalf()
		{
			return authHalf;
		}

		public void setMessages(ObservableCollection<MessageListItem> messages)
		{
			this.messages = messages;
		}
		public ObservableCollection<MessageListItem> getMessages()
		{
			return messages;
		}

		public void setConds(ObservableCollection<SimpleListItem> conds)
		{
			this.conds = conds;
		}
		public ObservableCollection<SimpleListItem> getConds()
		{
			return conds;
		}

		public void setMeds(ObservableCollection<MedListItem> meds)
		{
			this.meds = meds;
		}
		public ObservableCollection<MedListItem> getMeds()
		{
			return meds;
		}

		public void setProducts(ObservableCollection<ProductListItem> products)
		{
			this.products = products;
		}
		public ObservableCollection<ProductListItem> getProducts()
		{
			return products;
		}

		public void setCartItems(ObservableCollection<CartListItem> cartItems)
		{
			this.cartItems = cartItems;
		}
		public ObservableCollection<CartListItem> getCartItems()
		{
			return cartItems;
		}

		public void setOrders(ObservableCollection<OrderListItem> orders)
		{
			this.orders = orders;
		}
		public ObservableCollection<OrderListItem> getOrders()
		{
			return orders;
		}

		public void setLogs(ObservableCollection<LogListItem> logs)
		{
			this.logs = logs;
		}
		public ObservableCollection<LogListItem> getLogs()
		{
			return logs;
		}

		public void setDispensaries(ObservableCollection<DispListItem> dispensaries)
		{
			this.dispensaries = dispensaries;
		}
		public ObservableCollection<DispListItem> setDispensaries()
		{
			return dispensaries;
		}

		public void setAppearInSearch(int appearInSearch)
		{
			this.appearInSearch = appearInSearch;
		}
		public int getAppearInSearch()
		{
			return appearInSearch;
		}

		public void setBlockNonContacts(int blockNonContacts)
		{
			this.blockNonContacts = blockNonContacts;
		}
		public int getBlockNonContacts()
		{
			return blockNonContacts;
		}

		public void setBlacklist(ObservableCollection<SimpleListItem> blacklist)
		{
			this.blacklist = blacklist;
		}
		public ObservableCollection<SimpleListItem> getBlacklist()
		{
			return blacklist;
		}

		public void setMaster(MasterPage master)
		{
			this.master = master;
		}
		public MasterPage getMaster()
		{
			return master;
		}

		public void setMsgClicked(MessageListItem msgClicked)
		{
			this.msgClicked = msgClicked;
		}
		public MessageListItem getMsgClicked()
		{
			return msgClicked;
		}

		public void setProdClicked(ProductListItem prodClicked)
		{
			this.prodClicked = prodClicked;
		}
		public ProductListItem getProdClicked()
		{
			return prodClicked;
		}

		public void setOrderClicked(OrderListItem orderClicked)
		{
			this.orderClicked = orderClicked;
		}
		public OrderListItem getOrderClicked()
		{
			return orderClicked;
		}

		public void setLogClicked(LogListItem logClicked)
		{
			this.logClicked = logClicked;
		}
		public LogListItem getLogClicked()
		{
			return logClicked;
		}

		public void setCatClicked(string catClicked)
		{
			this.catClicked = catClicked;
		}
		public string getCatClicked()
		{
			return catClicked;
		}

		public void setCatClickedContents(List<ProductListItem> catClickedContents)
		{
			this.catClickedContents = catClickedContents;
		}
		public List<ProductListItem> getCatClickedContents()
		{
			return catClickedContents;
		}

		public string UrlEncodeParameter(string paramToEncode)
		{
			/*string urlEncodedParam = string.Empty;

			// remove whitespace from search parameter and URL encode it
			urlEncodedParam = paramToEncode.Trim();
			urlEncodedParam = Uri.EscapeDataString(urlEncodedParam);*/

			return paramToEncode;
		}

		public void clearAll()
		{
			credentials = null;
			user = null;
			messages = null;
			conds = null;
			meds = null;

			//Products Tab
			products = null;
			orders = null;

			//Dispensary List
			dispensaries = null;

			//Analytics Tab
			logs = null;

			//To manipulate navigation
			master = null;

			//Passing ListClicked Info to other pages
			msgClicked = null;
			prodClicked = null;
			orderClicked = null;
			catClicked = null;
			catClickedContents = null;
		}
	}
}
