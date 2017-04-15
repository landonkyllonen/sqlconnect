using System.ComponentModel;
using Xamarin.Forms;

namespace SQLConnect
{
	public class ProductListItem : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this,
					new PropertyChangedEventArgs(propertyName));
			}
		}

		string name;
		ImageSource imgsrc;
		
		public string prodName { 
			get { return name; }
			set { name = value; OnPropertyChanged("prodName"); }
		}
		public string prodCategory { get; set; }
		public string prodDescription { get; set; }

		public string prodImgUrl { get; set; }
		public ImageSource prodImgSrc {
			get { return imgsrc; }
			set { imgsrc = value; OnPropertyChanged("prodImgSrc"); }
		}

		public double prodUnitPrice { get; set; }
		public string prodIncrementType { get; set; }
		public double prodUnitPriceIncentive { get; set; }
		public double prodDiscount { get; set; }
		public double prodBulkDiscount { get; set; }//Top discount % achievable by purchasing in bulk.
		public int prodBulkType { get; set; }//Method by which discount % progresses as you purchase more.
		public bool prodDealFlag { get; set; }//Whether not on sale.
		public bool prodIncentiveFlag { get; set; }//Whether or not purchasable with loyalty points.
		public string prodOrderPrice { get; set; }
		public string prodOrderAmount { get; set; }
	}
}
