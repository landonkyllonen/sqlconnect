using System;
namespace SQLConnect
{
	public class LogListItem
	{
		public string logTitle { get; set; }
		public string logDate { get; set; }
		public string logText { get; set; }
		public bool logPublic { get; set; }
		public bool logImportant { get; set; }
		public string[] logMeds { get; set; }
	}
}
