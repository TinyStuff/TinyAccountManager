using System;

namespace TinyAccountManager.Abstraction
{
    public static class AccountManager
    {
        public static IAccountManager Current { get; set; }
    }
}
