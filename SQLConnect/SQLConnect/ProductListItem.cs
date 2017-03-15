namespace SQLConnect
{
	public class ProductListItem
	{
		public string prodName { get; set; }
		public string prodCategory { get; set; }
		public string prodDescription { get; set; }
		public string prodImgUrl { get; set; }
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
