using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
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

			gatherItemImages();
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


		async void gatherItemImages()
		{
			Debug.WriteLine("Gathering images for " + prodItems.Count + " items...");
			var client = new HttpClient();

			//Download image for each item.
			foreach (ProductListItem p in prodItems)
			{
				var contentSent = new MultipartFormDataContent();
				contentSent.Add(new StringContent(p.prodName), "name");
				contentSent.Add(new StringContent(Statics.Default.getCreds()[16]), "dispId");
				var response = await client.PostAsync("http://cbd-online.net/landon/downloadPictures.php", contentSent);

				byte[] output = await response.Content.ReadAsByteArrayAsync();

				if (output == null)
				{
					Debug.WriteLine("No data for " + p.prodName + " gathered.");
				}
				else
				{
					Debug.WriteLine("Printing output for " + p.prodName + ":");
					Debug.WriteLine(output.Length);
				}

				//Update list picture
				p.prodImgSrc = ImageSource.FromStream(() => new MemoryStream(output));

				//Find and update the filtered list pic as well.
				foreach (ProductListItem f in prodItemsFiltered)
				{
					if (p.prodName.Equals(f.prodName))
					{
						f.prodImgSrc = p.prodImgSrc;
						break;
					}
				}
			}
		}


		void onItemSelect(object sender, ItemTappedEventArgs e)
		{
			Statics.Default.setProdClicked((ProductListItem)e.Item);
			NavigationPage nav = new NavigationPage(new ProductPage(false));
			NavigationPage.SetHasBackButton(nav, true);
			Navigation.PushModalAsync(nav);
		}
	}


}
