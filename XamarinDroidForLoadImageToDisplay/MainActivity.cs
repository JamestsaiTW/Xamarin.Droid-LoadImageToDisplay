using System.Net.Http;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Widget;
using Android.OS;
using Android.Runtime;

namespace XamarinDroidForLoadImageToDisplay
{
    [Activity(Label = "App1", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var mainImageView = FindViewById<ImageView>(Resource.Id.MainImageView);

            var loadImageFromResourceButton = FindViewById<Button>(Resource.Id.LoadImageFromResourceButton);
            loadImageFromResourceButton.Click += (sender, e) =>
            {
                mainImageView.SetImageResource(Resource.Drawable.Xamarin_Logo1);
            };

            var loadImageFromDrawableButton = FindViewById<Button>(Resource.Id.LoadImageFromDrawableButton);
            loadImageFromDrawableButton.Click += (sender, e) =>
            {
                //此兩行都可以，選擇其中一行執行
                //mainImageView.SetImageDrawable(GetDrawable(Resource.Drawable.Xamarin_Logo2));
                mainImageView.SetImageDrawable(
                    GetDrawable(Resources.GetIdentifier("xamarin_logo2", "drawable", PackageName)));
            };

            var loadImageFromAssertsButton = FindViewById<Button>(Resource.Id.LoadImageFromAssertsButton);
            loadImageFromAssertsButton.Click += (sender, e) =>
            {
                //此兩行都可以，選擇其中一行執行
                //mainImageView.SetImageDrawable(Android.Graphics.Drawables.Drawable.CreateFromStream(Assets.Open("Xamarin_Logo3.png"),null));
                mainImageView.SetImageBitmap(BitmapFactory.DecodeStream(Assets.Open("Xamarin_Logo3.png")));
            };

            var loadImageFromLocalFileButton = FindViewById<Button>(Resource.Id.LoadImageFromLocalFileButton);
            loadImageFromLocalFileButton.Click += async (sender, e) =>
            {
                var fileName = "Xamarin_Logo4.png";
                var fullPath = System.IO.Path.Combine(FilesDir.Path, fileName);
                if (!System.IO.File.Exists(fullPath))
                {
                    using (var stream = Assets.Open(fileName))
                    {
                        using (var file = System.IO.File.Create(fullPath))
                        {
                            await stream.CopyToAsync(file);
                        }
                    }
                }
                mainImageView.SetImageURI(Android.Net.Uri.Parse(fullPath));
            };

            var loadImageFromGalleryButton = FindViewById<Button>(Resource.Id.LoadImageFromGalleryButton);
            loadImageFromGalleryButton.Click += (sender, e) =>
            {
                var intent = new Intent();
                intent.SetType("image/*");
                intent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(intent, "選取圖片"), 999);
            };

            var loadImageFromUrlButton = FindViewById<Button>(Resource.Id.LoadImageFromUrlButton);
            loadImageFromUrlButton.Click += async (sender, e) =>
            {
                mainImageView.SetImageBitmap(
                    await GetImageFromUrlAsync("https://xamarinclassdemo.azurewebsites.net/images/xamarin_logo5.png"));
            };
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 999 && resultCode == Result.Ok)
            {
                var imageView = FindViewById<ImageView>(Resource.Id.MainImageView);
                imageView.SetImageURI(data.Data);
            }
        }
    
        private static async Task<Bitmap> GetImageFromUrlAsync(string url)
        {
            Bitmap imageBitmap = null;
            using (var httpClient = new HttpClient())
            {
                var imageBytes = await httpClient.GetByteArrayAsync(url);

                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }
            return imageBitmap;
        }
    }
}

