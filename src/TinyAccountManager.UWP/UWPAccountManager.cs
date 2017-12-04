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
        public Task<bool> Exists(string username, string serviceId)
        {
            throw new NotImplementedException();
        }

        public Task<Account> Get(string username, string serviceId)
        {
            throw new NotImplementedException();
        }

        public Task Remove(string username, string serviceId)
        {
            throw new NotImplementedException();
        }

        public Task Save(Account account)
        {
            throw new NotImplementedException();
        }
    }
}
