using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TinyAccountManager.Abstraction
{
    public interface IAccountManager
    {
        Task Save(Account account);
        Task<Account> Get(string serviceId);
        Task<bool> Exists(string serviceId);
        Task Remove(string serviceId);
    }
}
