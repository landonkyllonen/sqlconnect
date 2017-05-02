using System.Collections.ObjectModel;

namespace SQLConnect
{
	public class OrderListItem
	{
		public int orderID { get; set; }
		public string orderDate { get; set; }
		public string orderTotal { get; set; }
		public string orderPaymentStatus { get; set; }
		public string orderCompletionStatus { get; set; }
		public ObservableCollection<CartListItem> orderItems { get; set;}
	}
}
