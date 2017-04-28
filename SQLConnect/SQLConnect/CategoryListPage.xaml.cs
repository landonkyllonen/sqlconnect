using System;
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

			prodItems = Statics.Default.getCatClickedContents();
			Title = Statics.Default.getCatClicked();

			prodItemsFiltered = new ObservableCollection<ProductListItem>();

			prodList.ItemsSource = prodItems;
			prodList.ItemTapped += onItemSelect;

			gatherItemImages();
		}


		void filterItems(object sender, TextChangedEventArgs e)
		{
			prodList.BeginRefresh();

			if (string.IsNullOrWhiteSpace(e.NewTextValue))
			{
				prodList.ItemsSource = prodItems;
			}
			else
			{
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

			ObservableCollection<ProductListItem> pulled = Statics.Default.getProducts();
			int place;

			//Download image for each item.
			foreach (ProductListItem p in prodItems)
			{
				if (p.prodImgSrc == null)
				{
					//Get place for update
					place = pulled.IndexOf(p);

					var contentSent = new MultipartFormDataContent();
					contentSent.Add(new StringContent(p.prodName), "name");
					contentSent.Add(new StringContent(Statics.Default.getCreds()[16]), "dispId");

					Debug.WriteLine("Sending " + p.prodName + " with dispId " + Statics.Default.getCreds()[16]);

					var response = await client.PostAsync("http://cbd-online.net/landon/downloadPictures.php", contentSent);

					byte[] output = await response.Content.ReadAsByteArrayAsync();

					if (output.Length < 1)
					{
						Debug.WriteLine("No data for " + p.prodName + " gathered.");
					}
					else
					{
						Debug.WriteLine("Printing byte[] length for " + p.prodName + ":");
						Debug.WriteLine(output.Length);
					}

					//Update list picture
					ImageSource src = ImageSource.FromStream(() => new MemoryStream(output));
					p.prodImgSrc = src;

					//Find and update the filtered list pic as well.
					foreach (ProductListItem f in prodItemsFiltered)
					{
						if (p.prodName.Equals(f.prodName))
						{
							f.prodImgSrc = p.prodImgSrc;
							break;
						}
					}

					//If was updated, update the static list.
					if (p.prodImgSrc != null)
					{
						pulled.RemoveAt(place);
						pulled.Insert(place, p);
					}
				}
			}
			//Update static to save pictures for future use.
			Statics.Default.setProducts(pulled);
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
