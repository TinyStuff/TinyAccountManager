using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TinyAccountManager.Abstraction;

namespace TinyAccountManager.Droid
{
    public class AndroidAccountManager : IAccountManager
    {
        public Task Create(Account account)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Exists(string username)
        {
            throw new NotImplementedException();
        }

        public Task<Account> Get(string username)
        {
            throw new NotImplementedException();
        }
    }
}