using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SQLConnect
{
	public class Statics
	{
		//Profile page, etc
		string[] credentials;
		string user;
		ObservableCollection<MessageListItem> messages;

		//Products Tab
		Product[] products; //Dispensary specific item info, only has url for pic to be downloaded.

		//To manipulate navigation
		MasterPage master;

		//Passing ListClicked Info to other pages
		MessageListItem msgClicked;
		ProductListItem prodClicked;
		string catClicked;
		List<ProductListItem> catClickedContents;




		// this is the default static instance you'd use like string text = Settings.Default.SomeSetting;
		public readonly static Statics Default = new Statics();

		public void setCreds(string[] credentials)
		{
			this.credentials = credentials;
		}
		public string[] getCreds()
		{
			return this.credentials;
		}

		public void setUser(string user)
		{
			this.user = user;
		}
		public string getUser()
		{
			return this.user;
		}

		public void setMessages(ObservableCollection<MessageListItem> messages)
		{
			this.messages = messages;
		}
		public ObservableCollection<MessageListItem> getMessages()
		{
			return this.messages;
		}

		public void setProducts(Product[] products)
		{
			this.products = products;
		}
		public Product[] getProducts()
		{
			return this.products;
		}

		public void setMaster(MasterPage master)
		{
			this.master = master;
		}
		public MasterPage getMaster()
		{
			return this.master;
		}

		public void setMsgClicked(MessageListItem msgClicked)
		{
			this.msgClicked = msgClicked;
		}
		public MessageListItem getMsgClicked()
		{
			return this.msgClicked;
		}

		public void setProdClicked(ProductListItem prodClicked)
		{
			this.prodClicked = prodClicked;
		}
		public ProductListItem getProdClicked()
		{
			return this.prodClicked;
		}

		public void setCatClicked(string catClicked)
		{
			this.catClicked = catClicked;
		}
		public string getCatClicked()
		{
			return this.catClicked;
		}

		public void setCatClickedContents(List<ProductListItem> catClickedContents)
		{
			this.catClickedContents = catClickedContents;
		}
		public List<ProductListItem> getCatClickedContents()
		{
			return this.catClickedContents;
		}

		public string UrlEncodeParameter(string paramToEncode)
		{
			/*string urlEncodedParam = string.Empty;

			// remove whitespace from search parameter and URL encode it
			urlEncodedParam = paramToEncode.Trim();
			urlEncodedParam = Uri.EscapeDataString(urlEncodedParam);*/

			return paramToEncode;
		}

	}
}
