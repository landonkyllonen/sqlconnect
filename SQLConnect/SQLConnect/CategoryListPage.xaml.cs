using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SQLConnect
{
	public partial class CategoryListPage : ContentPage
	{
		ObservableCollection<ProductListItem> prodItems;
		ObservableCollection<ProductListItem> prodItemsFiltered;


		public CategoryListPage()
		{
			InitializeComponent();

			List<ProductListItem> prods = Statics.Default.getCatClickedContents();
			Title = Statics.Default.getCatClicked();

			prodItems = new ObservableCollection<ProductListItem>();
			prodItemsFiltered = new ObservableCollection<ProductListItem>();
			foreach (ProductListItem prod in prods)
			{
				prodItems.Add(prod);
			}

			prodList.ItemsSource = prodItems;
			prodList.ItemTapped += onItemSelect;
		}

		void filterItems(object sender, TextChangedEventArgs e)
		{
			prodList.BeginRefresh();

			if (string.IsNullOrWhiteSpace(e.NewTextValue))
				prodList.ItemsSource = prodItems;
			else {
				prodItemsFiltered.Clear();
				prodItemsFiltered = new ObservableCollection<ProductListItem>();
				foreach (ProductListItem item in prodItems)
				{
					if (item.prodName.ToLower().Contains(e.NewTextValue.ToLower()))
					{
						prodItemsFiltered.Add(item);
					}
				}
				prodList.ItemsSource = prodItemsFiltered;
			}

			prodList.EndRefresh();
		}

		void onItemSelect(object sender, ItemTappedEventArgs e)
		{
			return;
		}
	}


}
