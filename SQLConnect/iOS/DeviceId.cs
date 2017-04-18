using System.Net;
using SQLConnect.iOS;
using System.Diagnostics;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(DeviceId))]
namespace SQLConnect.iOS  //   /\
{
	class DeviceId : IDeviceId
	{
		public DeviceId() //I saw on Xamarin documentation that it's important to NOT pass any parameter on that constructor
		{
		}

		public string getDeviceId()
		{
			try
			{
				string id = UIDevice.CurrentDevice.IdentifierForVendor.ToString();
				return id;
			}
			catch (WebException e)
			{
				Debug.WriteLine(e.StackTrace);
				return null;
			}
		}
	}
}