using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TinyAccountManager.Abstraction
{
    public interface IAccountManager
    {
        Task Save(Account account);
        Task<Account> Get(string username);
        Task<bool> Exists(string username);
    }
}
