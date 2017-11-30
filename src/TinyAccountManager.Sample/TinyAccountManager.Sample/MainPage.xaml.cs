using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using TinyAccountManager.Abstraction;

namespace TinyAccountManager.Sample
{
	public partial class MainPage : ContentPage
	{
        private IAccountManager accountManager;

		public MainPage()
		{
			InitializeComponent();

            accountManager = AccountManager.Current;
        }

        private async void SaveClicked(object sender, EventArgs e)
        {
            var data = new Dictionary<string, string>();
            data.Add("Age", "29");

            var account = new Account()
            {
                Username = "TinyUser",
                Properties = data
            };
            
            await accountManager.Save(account);
        }

        private async void GetClicked(object sender, EventArgs e)
        {
            var account = await accountManager.Get("TinyUser");

            DisplayAlert("Message", account.Properties["Age"], "OK");
        }

        private async void ExistsClicked(object sender, EventArgs e)
        {
            var exists = await accountManager.Exists("TinyUser");

            DisplayAlert("Message", exists.ToString(), "OK");
        }
    }
}
