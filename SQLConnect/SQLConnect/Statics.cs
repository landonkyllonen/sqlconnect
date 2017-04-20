using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SQLConnect
{
	public class Statics
	{
		//States
		bool offline = true;
		bool editing = false;

		//Profile page, etc
		string[] credentials;
		string user;
		string authHalf = "GFEDCBA";
		ObservableCollection<MessageListItem> messages;
		ObservableCollection<SimpleListItem> conds;
		ObservableCollection<MedListItem> meds;


		//Products Tab
		ProductListItem deal;
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
		ObservableCollection<SimpleListItem> contacts;

		//To manipulate navigation
		MasterPage master;
		MasterList masterList;

		//Passing ListClicked Info to other pages
		MessageListItem msgClicked;
		ProductListItem prodClicked;
		OrderListItem orderClicked;
		LogListItem logClicked;
		string catClicked;
		ObservableCollection<ProductListItem> catClickedContents;

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

		public void setEditing(bool state)
		{
			editing = state;
		}
		public bool IsEditing()
		{
			return editing;
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

		public void setDeal(ProductListItem deal)
		{
			this.deal = deal;
		}
		public ProductListItem getDeal()
		{
			return deal;
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
			//Save this locally for reload.
			serializeAndSave("cart");
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
		public ObservableCollection<DispListItem> getDispensaries()
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

		public void setContacts(ObservableCollection<SimpleListItem> contacts)
		{
			this.contacts = contacts;
		}
		public ObservableCollection<SimpleListItem> getContacts()
		{
			return contacts;
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

		public void setMasterList(MasterList masterList)
		{
			this.masterList = masterList;
		}
		public MasterList getMasterList()
		{
			return masterList;
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

		public void setCatClickedContents(ObservableCollection<ProductListItem> catClickedContents)
		{
			this.catClickedContents = catClickedContents;
		}
		public ObservableCollection<ProductListItem> getCatClickedContents()
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

		public void serializeAndSave(string property)
		{
			switch (property)
			{
				case "all":
					break;
				case "cart":
					string saved = "";
					foreach (CartListItem c in cartItems)
					{
						saved += c.prodName + ";;" + c.prodAmount + ";;" + c.prodRate + ";;" + c.prodTotal + ";;" + c.prodUnitType + ";;" + c.prodIsFlower + ";;" + c.prodIsRegular+"~~";
					}
					//Cut of last 2 delims.
					saved = saved.Substring(0, saved.Length - 2);
					App.Current.Properties.Add("cart", saved);
					//Now make sure these properties are saved in case the app is killed.
					App.Current.SavePropertiesAsync();
					break;
				default:
					break;
			}
		}

		/*This is a fairly complex method, which could probably be simplified.
		 * Parameter 'property' is the Statics property from local memory that you want to load into App memory.
		*/
		public void deconstructAndLoad(string property)
		{
			try
			{
				//If that property has been saved...
				if (App.Current.Properties.ContainsKey(property))
				{
					//Load it
					var data = App.Current.Properties[property] as string;
					//Parse the string and load the specified property with its information.
					switch (property)
					{
						case "cart":
							//Translate string into cart items.
							ObservableCollection<CartListItem> cartLoaded = new ObservableCollection<CartListItem>();
							string[] items = data.Split(new string[] { "~~" }, StringSplitOptions.None);
							foreach (string s in items)
							{
								string[] components = s.Split(new string[] { ";;" }, StringSplitOptions.None);
								cartLoaded.Add(new CartListItem
								{
									prodName = components[0],
									prodAmount = double.Parse(components[1]),
									prodRate = components[2],
									prodTotal = components[3],
									prodUnitType = components[4],
									prodIsFlower = bool.Parse(components[6]),
									prodIsRegular = bool.Parse(components[7])
								});
							}

							//Now that every local cartitem has been parsed, if item still exists in loaded products, update the prices of cart item.
							//If item does not exist in loaded products, discard it.
							foreach (CartListItem c in cartLoaded)
							{
								//If product is a flower, we calculate the price differently.
								if (c.prodIsFlower)
								{
									foreach (ProductListItem p in products)
									{
										//If item is found...
										if (p.prodName.Equals(c.prodName))
										{
											double[] medicalPrices;
											double oz = c.prodAmount;
											int bulktype = p.prodBulkType;

											//Generate price list for flower based on bulk discount type.

											double discountMult;
											if (p.prodDealFlag)
											{
												discountMult = 1 - p.prodDiscount;
											}
											else
											{
												discountMult = 1;
											}
											switch (bulktype)
											{
												case 0:
													//No discount.
													medicalPrices = new double[5];
													medicalPrices[0] = p.prodUnitPrice * discountMult;
													medicalPrices[1] = 3.54688 * p.prodUnitPrice * discountMult;
													medicalPrices[2] = 3.54688 * 2 * p.prodUnitPrice * discountMult;
													medicalPrices[3] = 3.54688 * 2 * 2 * p.prodUnitPrice * discountMult;
													medicalPrices[4] = 3.54688 * 2 * 2 * 2 * p.prodUnitPrice * discountMult;
													break;
												case 1:
													//Linear discount progression, 1 oz being the maximum discount(specified amount)
													medicalPrices = new double[5];
													medicalPrices[0] = p.prodUnitPrice * discountMult;//None
													medicalPrices[1] = 3.54688 * p.prodUnitPrice * (1 - p.prodBulkDiscount * 1 / 4) * discountMult;// fourth of discount
													medicalPrices[2] = 3.54688 * 2 * p.prodUnitPrice * (1 - p.prodBulkDiscount * 1 / 2) * discountMult;// half of discount
													medicalPrices[3] = 3.54688 * 2 * 2 * p.prodUnitPrice * (1 - p.prodBulkDiscount * 3 / 4) * discountMult;// three/fourths of discount
													medicalPrices[4] = 3.54688 * 2 * 2 * 2 * p.prodUnitPrice * (1 - p.prodBulkDiscount) * discountMult;// max discount
													break;
												case 2:
													//each step up gives half the discount of the previous step.
													medicalPrices = new double[5];
													medicalPrices[0] = p.prodUnitPrice * discountMult;
													medicalPrices[1] = 3.54688 * p.prodUnitPrice * (1 - (p.prodBulkDiscount / 2)) * discountMult;//Half discount
													medicalPrices[2] = 3.54688 * 2 * p.prodUnitPrice * (1 - (p.prodBulkDiscount * 3 / 4)) * discountMult;//Fourth more
													medicalPrices[3] = 3.54688 * 2 * 2 * p.prodUnitPrice * (1 - (p.prodBulkDiscount * 7 / 8)) * discountMult;//Eighth more
													medicalPrices[4] = 3.54688 * 2 * 2 * 2 * p.prodUnitPrice * (1 - (p.prodBulkDiscount * 15 / 16)) * discountMult;//Sixteenth more
													break;
												default:
													//No discount.
													medicalPrices = new double[5];
													medicalPrices[0] = p.prodUnitPrice * discountMult;
													medicalPrices[1] = 3.54688 * p.prodUnitPrice * discountMult;
													medicalPrices[2] = 3.54688 * 2 * p.prodUnitPrice * discountMult;
													medicalPrices[3] = 3.54688 * 2 * 2 * p.prodUnitPrice * discountMult;
													medicalPrices[4] = 3.54688 * 2 * 2 * 2 * p.prodUnitPrice * discountMult;
													break;
											}

											//Generate new total value.
											int priceAmount = -1;
											if (c.prodUnitType.Equals("g"))
											{
												priceAmount = 0;
											}
											else
											{
												switch ((int)(oz * 8))
												{
													case 1:
														priceAmount = 1;
														break;
													case 2:
														priceAmount = 2;
														break;
													case 4:
														priceAmount = 3;
														break;
													case 8:
														priceAmount = 4;
														break;
													default:
														break;
												}
											}
											//Set new total.
											c.prodTotal = medicalPrices[priceAmount].ToString();

											//Calculate new rate.
											string rate;
											switch (priceAmount)
											{
												case 0:
													rate = "(" + (medicalPrices[0]).ToString("C") + "/g)";
													break;
												case 1:
													rate = "(" + (medicalPrices[1] / 3.54688).ToString("C") + "/g)";
													break;
												case 2:
													rate = "(" + (medicalPrices[2] / (3.54688 * 2)).ToString("C") + "/g)";
													break;
												case 3:
													rate = "(" + (medicalPrices[3] / (3.54688 * 4)).ToString("C") + "/g)";
													break;
												default:
													rate = "(" + (medicalPrices[4] / (3.54688 * 8)).ToString("C") + "/g)";
													break;
											}
											//Set new rate
											c.prodRate = rate;

											//Add this updated cart item into memory.
											this.cartItems.Add(c);
											//No longer search for this item, already found, move to next.
											break;
										}
										//Item is not found in dispensary inventory.
										else
										{
											//This item should not be kept on app close, at end of both loops we will resave only cart items that could be updated.
										}
									}
								}
								else
								{
									//Item is not flower
								}
							}
							//Now all of the cart items that could be are updated, resave this to local memory.
							serializeAndSave("cart");
							//END OF LOADING CART FROM MEMORY.
							break;
						default:
							break;
					}

				}
				else
				{
					Debug.WriteLine("There was no value " + property + " saved to local memory.");
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine(property + " was found, but there was a problem processing it's values.");
				Debug.WriteLine(e.StackTrace);
			}
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
