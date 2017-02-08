using Xamarin.Forms;

namespace SQLConnect
{
	public class Product
	{
		public string name, category, description, incrementType, imgURL;
		public double price, priceInPoints, discount;
		public bool deal, incentive;

		//Reg
		public Product(string name, string category, string description, string incrementType, double price, string imgURL)
		{
			this.name = name;
			this.category = category;
			this.description = description;
			this.incrementType = incrementType;
			this.price = price;
			this.imgURL = imgURL;
		}

		public Product(string name, string category, string description, string incrementType, double price, string imgURL, double discount, double priceInPoints, bool deal, bool incentive)
		{
			this.name = name;
			this.category = category;
			this.description = description;
			this.incrementType = incrementType;
			this.price = price;
			this.imgURL = imgURL;
			this.discount = discount;
			this.priceInPoints = priceInPoints;
			this.deal = deal;
			this.incentive = incentive;
		}
	}
}
