using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyAccountManager.Abstraction;

namespace TinyAccountManager.UWP
{
    public class UWPAccountManager : IAccountManager
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
