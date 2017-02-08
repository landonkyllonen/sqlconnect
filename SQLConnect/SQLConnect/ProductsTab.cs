﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SQLConnect
{
	public class ProductsTab : ContentPage
	{
		ObservableCollection<CategoryListItem> categories;
		ProductListItem[] catClickedContents;
		Product[] loadedProducts;

		public ProductsTab()
		{
			//Instantiate Products
			loadedProducts = Statics.Default.getProducts();

			//Instantiate listview
			ListView catList = new ListView();
			catList.RowHeight = 150;

			//Create row layouts
			var categoryDataTemplate = new DataTemplate(() =>
			{
				RelativeLayout holder = new RelativeLayout {HorizontalOptions=LayoutOptions.Fill, VerticalOptions=LayoutOptions.Fill};

				var img = new Image { Aspect = Aspect.AspectFill };
				var tintbox = new BoxView { Color=Color.Teal, Opacity = 0.2};
				var nameLabel = new Label { TextColor = Color.White, FontSize=28, HorizontalTextAlignment = TextAlignment.Center,VerticalTextAlignment=TextAlignment.Center };

				nameLabel.SetBinding(Label.TextProperty, "catName");
				img.SetBinding(Image.SourceProperty, "catImgPath");

				holder.Children.Add(img, Constraint.Constant(0), Constraint.Constant(0),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Width;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Height;
									}));
				holder.Children.Add(tintbox, Constraint.Constant(0),Constraint.Constant(0),
				                    Constraint.RelativeToParent((parent) =>
									{
										return parent.Width;
									}),
				                    Constraint.RelativeToParent((parent) =>
									{
										return parent.Height;
									}));
				holder.Children.Add(nameLabel, Constraint.Constant(0),
				                    Constraint.RelativeToParent((parent) =>
									{
										return parent.Height/2 -25;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Width;
									}),
				                    Constraint.Constant(50));

				return new ViewCell { View = holder };
			});

			catList.ItemTemplate = categoryDataTemplate;

			categories = new ObservableCollection<CategoryListItem>();
			categories.Add(new CategoryListItem { catName = "Flowers", catImgPath = "blueberryyumyumi.jpg" });
			categories.Add(new CategoryListItem { catName = "Concentrates", catImgPath = "concentrate.jpg" });               
			categories.Add(new CategoryListItem { catName = "Edibles", catImgPath = "edibles.jpg" });
			categories.Add(new CategoryListItem { catName = "Glass", catImgPath = "glass.jpg" });
			categories.Add(new CategoryListItem { catName = "Apparel", catImgPath = "apparel.jpg" });

			catList.ItemsSource = categories;
			catList.ItemTapped += CatList_ItemTapped;

			Content = catList;
		}

		void CatList_ItemTapped(object sender, ItemTappedEventArgs e)
		{
			//Get category tapped
			string category = ((CategoryListItem)e.Item).catName;
			//Set static
			Statics.Default.setCatClicked(category);

			//Initialize container

			List<ProductListItem> catProducts = new List<ProductListItem>();

			//Linear search all products
			for (int i = 0; i < loadedProducts.Length; i++)
			{
				Product p = loadedProducts[i];

				//If in the category clicked, add to the list to be passed.
				if (p.category.Equals(category))
				{
					catProducts.Add(new ProductListItem { prodName = p.name, prodCategory = p.category, prodDescription = p.description,
						prodImgUrl = p.imgURL, prodUnitPrice = p.price, prodIncrementType = p.incrementType, prodUnitPriceIncentive = p.priceInPoints,
						prodDiscount = p.discount, prodDealFlag = p.deal, prodIncentiveFlag = p.incentive});
				}
			}

			//Once done, save to passed list
			Statics.Default.setCatClickedContents(catProducts);

			NavigationPage nav = new NavigationPage(new CategoryListPage());
			NavigationPage.SetHasBackButton(nav, true);

			Navigation.PushModalAsync(nav);
		}
	}
}

