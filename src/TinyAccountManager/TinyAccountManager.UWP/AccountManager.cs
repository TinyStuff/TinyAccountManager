using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyAccountManager.UWP
{
    public class AccountManager
    {
        public static void Initialize()
        {
            Abstraction.AccountManager.Current = new UWPAccountManager();
        }
    }
}
