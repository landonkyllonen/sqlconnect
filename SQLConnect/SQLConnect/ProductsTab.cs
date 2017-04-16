using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SQLConnect
{
	public class ProductsTab : ContentPage
	{
		ObservableCollection<CategoryListItem> categories;
		ObservableCollection<ProductListItem> loadedProducts;

		public ProductsTab()
		{
			Title = "Categories";
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
				var tintbox = new BoxView { Color=Color.Teal, Opacity = 0.1};
				var strip = new BoxView { Color = Color.Teal, Opacity = 0.7 };
				var striptrimtop = new BoxView { Color = Color.FromHex("#eeca0a"), Opacity = 0.9 };
				var striptrimbottom = new BoxView { Color = Color.FromHex("#eeca0a"), Opacity = 0.9 };
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
				holder.Children.Add(strip, Constraint.Constant(0),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Height / 2 - 18;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Width;
									}),
									Constraint.Constant(36));
				holder.Children.Add(striptrimtop, Constraint.Constant(0),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Height / 2 - 19;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Width;
									}),
									Constraint.Constant(1));
				holder.Children.Add(striptrimbottom, Constraint.Constant(0),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Height / 2 +18;
									}),
									Constraint.RelativeToParent((parent) =>
									{
										return parent.Width;
									}),
									Constraint.Constant(1));
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

			ObservableCollection<ProductListItem> catProducts = new ObservableCollection<ProductListItem>();

			//Linear search all products
			foreach (ProductListItem p in loadedProducts)
			{
				//If in the category clicked, add to the list to be passed.
				if (p.prodCategory.Equals(category))
				{
					catProducts.Add(p);
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

