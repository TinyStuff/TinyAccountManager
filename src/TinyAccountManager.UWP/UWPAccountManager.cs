using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyAccountManager.Abstraction;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage;
using System.IO;

namespace TinyAccountManager.UWP
{
    public class UWPAccountManager : IAccountManager
    {
        public async Task<bool> Exists(string username, string serviceId)
        {
            var filename = serviceId + "_" + username;

            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);

                if (file == null)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<Account> Get(string username, string serviceId)
        {
            var filename = serviceId + "_" + username;

            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);

            var protectedBuffer = await FileIO.ReadBufferAsync(file);

            var provider = new DataProtectionProvider();

            var unprotectedBuffer = await provider.UnprotectAsync(protectedBuffer);

            var json = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, unprotectedBuffer);

            var account = JsonConvert.DeserializeObject<Account>(json);

            return account;
        }

        public async Task Remove(string username, string serviceId)
        {
            var filename = serviceId + "_" + username;

            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);

            await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }


        public async Task Save(Account account)
        {
            var json = JsonConvert.SerializeObject(account);

            var localFolder = ApplicationData.Current.LocalFolder;

            var provider = new DataProtectionProvider("LOCAL=user");

            var buffer = CryptographicBuffer.ConvertStringToBinary(json, BinaryStringEncoding.Utf8);

            var protectedBuffer = await provider.ProtectAsync(buffer);

            var filename = account.ServiceId + "_" + account.Username;

            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename,CreationCollisionOption.OpenIfExists);
            
            await FileIO.WriteBufferAsync(file, protectedBuffer);
        }
    }
}
