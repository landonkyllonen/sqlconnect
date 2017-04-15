using System;
using System.Net;
using System.IO;
using System.Linq;
//Only thing that changes to droid class is that \/
using Foundation;
using UIKit;
using SQLConnect.iOS;
using System.Diagnostics;
using CoreGraphics;
using System.Drawing;

[assembly: Xamarin.Forms.Dependency(typeof(FileProcessing))]
namespace SQLConnect.iOS  //   /\
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

				NSData data = NSData.FromArray(bytes);
				NSData nsout;
				UIImage img = UIImage.LoadFromData(data);
				UIImage resultImg;
				nfloat originalWidth = img.Size.Width;
				nfloat originalHeight = img.Size.Height;
				nfloat ratio;
				nfloat maxWidth = 600;
				nfloat maxHeight = 400;
				nfloat newWidth;//Width requested.
				nfloat newHeight;

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
					else //If portrait.
					{
						ratio = originalWidth / originalHeight;
						newHeight = 400;//Height requested.
						newWidth = newHeight * ratio;
					}

					// Resize the image
					UIGraphics.BeginImageContext(new SizeF((float)newWidth, (float)newHeight));
					img.Draw(new RectangleF(0, 0, (float)newWidth, (float)newHeight));
					resultImg = UIGraphics.GetImageFromCurrentImageContext();
					UIGraphics.EndImageContext();
				}
				else
				{
					resultImg = img;
				}

				MemoryStream ms;

				switch (type)
				{
					case "jpg":
						ms = new MemoryStream();
						nsout = resultImg.AsJPEG(50);
						return nsout.ToArray();
					case "png":
						ms = new MemoryStream();
						nsout = resultImg.AsJPEG(50);
						return nsout.ToArray();
					default:
						Debug.WriteLine("This format is not accepted.");
						return null;
				}

			}
			catch (WebException e)
			{
				Debug.WriteLine(e.StackTrace);
				return null;
			}
		}
	}
}