using Xamarin.Forms;

namespace SQLConnect
{
	public partial class BarcodePage : ContentPage
	{
		//ZXingBarcodeImageView barcode;

		public BarcodePage()
		{
			InitializeComponent();

			id.Text = "#" + Statics.Default.getCreds()[15];

			/*barcode = new ZXingBarcodeImageView
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
			};
			barcode.BarcodeFormat = BarcodeFormat.CODABAR;
			barcode.BarcodeOptions.Width = 300;
			barcode.BarcodeOptions.Height = 150;
			barcode.BarcodeOptions.Margin = 10;
			barcode.BarcodeValue = "asdsagt34ww";

			RelativeLayout holder = new RelativeLayout();
			holder.Children.Add(barcode, Constraint.RelativeToParent((parent) => { return parent.Width / 2 - 150; }),
								Constraint.RelativeToParent((parent) => { return parent.Height / 2 - 75; }),
								Constraint.Constant(300), Constraint.Constant(150));

			Content = holder;*/
		}
	}
}
