
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SQLConnect
{
	public partial class HelpPage : ContentPage
	{
		ObservableCollection<SimpleListItem> items;
		public HelpPage()
		{
			InitializeComponent();

			items = new ObservableCollection<SimpleListItem>();

			populateHelpItems();

			helpList.ItemsSource = items;
			helpList.ItemSelected += helpSelect;
		}

		void helpSelect(object s, SelectedItemChangedEventArgs e)
		{
			helpList.IsVisible = false;
			helpText.Text = (e.SelectedItem as SimpleListItem).content;
			helpText.IsVisible = true;
			backButton.IsVisible = true;
		}

		void backToList(object s, EventArgs e)
		{
			helpText.IsVisible = false;
			helpList.IsVisible = true;
			backButton.IsVisible = false;
		}

		private void populateHelpItems()
		{
			items.Add(new SimpleListItem { labelName = "Navigation",
				content ="Pressing the icon in the top left of your screen will reveal the different SECTIONS of this app, as well as a log out option.\n\n" +
				" Each SECTION contains TABS related to its SECTION. Using this scheme, you should be able to find what you're looking for by exploring each tab."});

			items.Add(new SimpleListItem
			{
				labelName = "How do purchases work?",
				content = "Once you've selected a dispensary, you will be able to view their products. \n\nThe current method for making purchases is as follows: " +
				"1. Once you've selected an item and added it to your cart, you can confirm your order. \n\n2. Payment is redirected through interac payable to the dispensary. " +
				"\n\n3. Once payment has been made, your order will be flagged as paid and the dispensary will prepare your order for pickup. \n\n4. Once you've collected your " +
				"product(s) at the dispensary, your order will be marked as complete. Each step of this process can be viewed from your Orders tab in the Products section."
			});

			items.Add(new SimpleListItem
			{
				labelName = "Is my information private?",
				content = "All messages have an encryption unique to each user. " +
				"\n\nInformation is collected regarding which products you've tried and what conditions/medications you have for search purposes. " +
				"\n\nThat is, if a user looks for other users with your condition and you have 'Appear in Searches' enabled, which is disabled by default, " +
				"your username, but no other information, will appear to them."
			});

			items.Add(new SimpleListItem
			{
				labelName = "Who can I contact if I have a problem?",
				content = "Contact the dispensary from which you made a purchase. \n\nIf you have a technical problem or your purchase issue was not resolved, feel free to contact the support team " +
				"with information relevant to your problem. (Proof of purchase, etc)"
			});
		}
	}
}
