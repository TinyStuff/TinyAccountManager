using Foundation;
using Newtonsoft.Json;
using Security;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TinyAccountManager.Abstraction;

namespace TinyAccountManager.iOS
{
    public class iOSAccountManager : IAccountManager
    {
        public async Task Save(Account account)
        {
            var data = JsonConvert.SerializeObject(account.Properties);

            var secRecord = new SecRecord(SecKind.GenericPassword)
            {
                Account = account.Username,
                Generic = account.Password ?? string.Empty,
                ValueData = NSData.FromString(data)
            };

            var old = await Find(account.Username);

            if (old == null)
            {
                SecKeyChain.Add(secRecord);

                return;
            }

            SecKeyChain.Update(old, secRecord);

        }

        private async Task<SecRecord> Find(string username)
        {
            var secRecord = new SecRecord(SecKind.GenericPassword)
            {
                Account = username
            };

            SecStatusCode status;

            var result = SecKeyChain.QueryAsRecord(secRecord, out status);

            if(status == SecStatusCode.Success)
            {
                return result;
            }

            return null;
        }

        public async Task<bool> Exists(string username)
        {
            var result = await Find(username);

            return (result != null);
        }

        public async Task<Account> Get(string username)
        {
            var result = await Find(username);

            if(result == null)
            {
                throw new Exception(string.Format("Account with username: {0}, does not exists", username));
            }

            var account = new Account()
            {
                Username = result.Account,
                Password = result.Generic.ToString(),
                Properties = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.ValueData.ToString())
            };

            return account;
        }
    }
}
