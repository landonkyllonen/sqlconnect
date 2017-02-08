using System;
namespace SQLConnect
{
	public class MessageListItem
	{
		public string msgTitle { get; set; }
		public string msgFrom { get; set; }
		public string msgDate { get; set; }
		public string msgContent { get; set;}
		public int msgId { get; set; }
		public bool msgViewed { get; set; }
	}
}
