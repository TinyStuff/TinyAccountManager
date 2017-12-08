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
            if(string.IsNullOrWhiteSpace(account.ServiceId) || string.IsNullOrWhiteSpace(account.Username))
            {
                throw new Exception("serviceId and username must be set.");
            }

            var data = JsonConvert.SerializeObject(account.Properties);

            var secRecord = new SecRecord(SecKind.GenericPassword)
            {
                Account = account.Username,
                Service = account.ServiceId,
                ValueData = NSData.FromString(data, NSStringEncoding.UTF8),
                Accessible = SecAccessible.AfterFirstUnlockThisDeviceOnly
            };

            var old = await Find(account.ServiceId);

            if (old == null)
            {
                var statusCode = SecKeyChain.Add(secRecord);

                return;
            }

            SecKeyChain.Update(old, secRecord);

        }

        private async Task<SecRecord> Find(string serviceId)
        {
            var query = new SecRecord(SecKind.GenericPassword)
            {
                Service = serviceId
            };

            SecStatusCode status;

            var result = SecKeyChain.QueryAsRecord(query, out status);

            if(status == SecStatusCode.Success)
            {
                return result;
            }

            return null;
        }

        public async Task<bool> Exists(string serviceId)
        {
            var result = await Find(serviceId);

            return (result != null);
        }

        public async Task<Account> Get(string serviceId)
        {
            var result = await Find(serviceId);

            if(result == null)
            {
                throw new Exception(string.Format("Account with username: {0}, does not exists", username));
            }

            var account = new Account()
            {
                Username = result.Account,
                Properties = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.ValueData.ToString())
            };

            return account;
        }

        public async Task Remove(string serviceId)
        {
            var result = await Find(serviceId);

            SecKeyChain.Remove(result);
        }
    }
}
