using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TinyAccountManager.Droid
{
    public class AccountManager
    {
            public static void Initialize()
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                    Abstraction.AccountManager.Current = new AndroidAccountManager();
                else
                    Abstraction.AccountManager.Current = new AndroidAccountManagerPreM(Application.Context, "AccountManagerSharedPreferences");
        }
    }
}