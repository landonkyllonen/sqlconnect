using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Android.Graphics;
using SQLConnect.Droid; //My droid project

[assembly: Xamarin.Forms.Dependency(typeof(FTP))] //You need to put this on iOS/droid class or uwp/etc if you wrote
namespace SQLConnect.Droid
{
	class FileProcessing : IFileProcessing
	{
		public FileProcessing() //I saw on Xamarin documentation that it's important to NOT pass any parameter on that constructor
		{
		}

		/// Upload File to Specified FTP Url with username and password and Upload Directory if need to upload in sub folders
		///Base FtpUrl of FTP Server
		///Local Filename to Upload
		///Username of FTP Server
		///Password of FTP Server
		///[Optional]Specify sub Folder if any
		/// Status String from Server
		public byte[] compress(byte[] bytes, string type)
		{
			try
			{
				byte[] uncompressed = bytes;

				Bitmap bmp = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
				Bitmap scaled;
				float originalWidth = bmp.Width;
				float originalHeight = bmp.Height;
				float ratio;
				float newWidth;//Width requested.
				float newHeight;

				//If image is bigger than 600x400 in either dimension.
				if (originalWidth > maxWidth || originalHeight > maxHeight)
				{
					//Scale the larger dimension to its max and keep aspect
					if (originalWidth > originalHeight)
					{
						ratio = originalHeight / originalWidth;
						newWidth = 600;//Width requested.
						newHeight = newWidth * ratio;
					}
					else 
					{
						ratio = originalWidth / originalHeight;
						newHeight = 400;//Height requested.
						newWidth = newHeight * ratio;
					}
					scaled = Bitmap.CreateScaledBitmap(bmp, (int)newWidth, (int)newHeight, false);
				}
				else
				{
					scaled = bmp;
				}

				MemoryStream ms;

				switch (type)
				{
					case "jpg":
						ms = new MemoryStream();
						bmp.Compress(Bitmap.CompressFormat.Jpeg, 50, ms);
						return ms.ToArray();
					case "png":
						ms = new MemoryStream();
						bmp.Compress(Bitmap.CompressFormat.Jpeg, 50, ms);
						return ms.ToArray();
					default:
						Debug.WriteLine("This format is not accepted.");
						return null;
				}

			}
			catch (WebException e)
			{
				Debug.WriteLine(e.ToString());
				return null;
			}
		}
	}
}