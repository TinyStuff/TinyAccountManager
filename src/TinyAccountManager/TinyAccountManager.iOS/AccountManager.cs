using System;
using System.Collections.Generic;
using System.Text;

namespace TinyAccountManager.iOS
{
    public class AccountManager
    {
        public static void Initialize()
        {
            Abstraction.AccountManager.Current = new iOSAccountManager();
        }
    }
}
