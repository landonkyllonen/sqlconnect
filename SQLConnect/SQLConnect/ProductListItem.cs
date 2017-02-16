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
		public bool prodDealFlag { get; set; }
		public bool prodIncentiveFlag { get; set; }
		public string prodOrderPrice { get; set; }
		public string prodOrderAmount { get; set; }
	}
}
