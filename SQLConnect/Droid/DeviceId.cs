using System.Diagnostics;
using Android.OS;
using System.Net;
using SQLConnect.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(DeviceId))] //You need to put this on iOS/droid class or uwp/etc if you wrote
namespace SQLConnect.Droid
{
	class DeviceId : IDeviceId
	{
		public DeviceId() //I saw on Xamarin documentation that it's important to NOT pass any parameter on that constructor
		{
		}

		public string getDeviceId()
		{
			string id;
			try
			{
				//Serial should work for both phones and tablets, unlike telephony.id.
				id = Build.Serial;
				return id;
			}
			catch (WebException e)
			{
				System.Diagnostics.Debug.WriteLine(e.ToString());
				return null;
			}
		}
	}
}