using System;
using System.Net.Http;
using System.Threading.Tasks;
using Foundation;
using Keeper.Core.Sleeper;
using UIKit;

namespace Keeper.iOS
{
    public class ViewController : UIViewController
    {
        private UIImageView _imageView;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.SystemBackgroundColor;


            var textField = new UITextField()
            {
                Placeholder = "username",
                BackgroundColor = UIColor.SecondarySystemBackgroundColor
            };

            View.AddSubview(textField);

            textField.TranslatesAutoresizingMaskIntoConstraints = false;
            textField.LeadingAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeadingAnchor, 16).Active = true;
            textField.TrailingAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TrailingAnchor, -16).Active = true;
            textField.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 16).Active = true;
            textField.HeightAnchor.ConstraintEqualTo(24).Active = true;
            textField.ShouldReturn += TextFieldReturned;

            _imageView = new UIImageView()
            {
                ContentMode = UIViewContentMode.ScaleAspectFit
            };

            View.AddSubview(_imageView);

            _imageView.TranslatesAutoresizingMaskIntoConstraints = false;
            _imageView.LeadingAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeadingAnchor, 16).Active = true;
            _imageView.TrailingAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TrailingAnchor, -16).Active = true;
            _imageView.TopAnchor.ConstraintEqualTo(textField.BottomAnchor, 16).Active = true;
            _imageView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -16).Active = true;
        }

        private bool TextFieldReturned(UITextField textField)
        {
            textField.ResignFirstResponder();
            Console.WriteLine(textField.Text);

            _ = SearchForUserAsync(textField.Text);

            return true;
        }

        private async Task SearchForUserAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return;
            }

            using var httpClient = new HttpClient();
            var sleeperClient = new SleeperClient(httpClient);
            var user = await sleeperClient.GetUserAsync(username);
            var avatar = await sleeperClient.GetAvatarAsync(user.AvatarId);

            InvokeOnMainThread(() => ShowAvatar(avatar));
        }

        private void ShowAvatar(byte[] avatar)
        {
            var data = NSData.FromArray(avatar);
            var image = UIImage.LoadFromData(data);
            _imageView.Image = image;
        }
    }
}
