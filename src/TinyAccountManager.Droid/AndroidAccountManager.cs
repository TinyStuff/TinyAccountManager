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
using Java.Security;
using Android.Security;
using Android.Security.Keystore;
using Javax.Crypto;
using Javax.Crypto.Spec;
using Newtonsoft.Json;
using System.IO;

namespace TinyAccountManager.Droid
{
    public class AndroidAccountManager : IAccountManager
    {
        public const string AesMode = "AES/GCM/NoPadding";
        public const string KeyStoreType = "AndroidKeyStore";

        private IKey GetKey(string alias)
        {
          

            var keyStore = KeyStore.GetInstance(KeyStoreType);
            keyStore.Load(null);

            if (!keyStore.ContainsAlias(alias))
            {
                var generator = KeyGenerator.GetInstance(KeyProperties.KeyAlgorithmAes, KeyStoreType);
                generator.Init(new KeyGenParameterSpec.Builder(alias, KeyStorePurpose.Encrypt | KeyStorePurpose.Decrypt)
                    .SetBlockModes(KeyProperties.BlockModeGcm)
                    .SetEncryptionPaddings(KeyProperties.EncryptionPaddingNone)
                    .SetRandomizedEncryptionRequired(false)
                    .Build());

                generator.GenerateKey();
            }

            var key = keyStore.GetKey(alias, null);

            return key;
        }

        private string GetAlias(string username, string serviceId)
        {
            return serviceId + "_" + username;
        }

        public async Task<bool> Exists(string username, string serviceId)
        {
            try
            {
                var alias = GetAlias(username, serviceId);

                var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

                var filePath = Path.Combine(documentsPath, alias);

                return File.Exists(filePath);
            }
            catch (Exception)
            {
                return false;
            } 
        }

        public async Task<Account> Get(string username, string serviceId)
        {
            var alias = GetAlias(username, serviceId);

            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            var filePath = Path.Combine(documentsPath, alias);

            var secretAccountJson = File.ReadAllText(filePath);

            var secretAccount = JsonConvert.DeserializeObject<SecretAccount>(secretAccountJson);

            var key = GetKey(alias);

            var cipher = Cipher.GetInstance(AesMode);
            cipher.Init(CipherMode.DecryptMode, key, new GCMParameterSpec(128, secretAccount.IV));

            var encryptedBytes = Convert.FromBase64String(secretAccount.EncryptedAccount);

            var decryptedBytes = cipher.DoFinal(encryptedBytes);

            var json = Encoding.UTF8.GetString(decryptedBytes);

            var account = JsonConvert.DeserializeObject<Account>(json);

            return account;
        }

        public async Task Remove(string username, string serviceId)
        {
            var alias = GetAlias(username, serviceId);

            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            var filePath = Path.Combine(documentsPath, alias);

            File.Delete(filePath);
        }

        public async Task Save(Account account)
        {
            if (string.IsNullOrWhiteSpace(account.ServiceId) || string.IsNullOrWhiteSpace(account.Username))
            {
                throw new Exception("serviceId and username must be set.");
            }

            var alias = GetAlias(account.Username, account.ServiceId);

            var key = GetKey(alias);

            var cipher = Cipher.GetInstance(AesMode);
            cipher.Init(CipherMode.EncryptMode, key);

            var iv = cipher.GetIV();

            var json = JsonConvert.SerializeObject(account);

            var bytes = Encoding.UTF8.GetBytes(json);

            var encodedBytes = cipher.DoFinal(bytes);

            var base64String = Convert.ToBase64String(encodedBytes);

            var secretAccount = new SecretAccount()
            {
                EncryptedAccount = base64String,
                IV = iv
            };

            var secretAccountJson = JsonConvert.SerializeObject(secretAccount);

            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            var filePath = Path.Combine(documentsPath, alias);
            System.IO.File.WriteAllText(filePath, secretAccountJson);

        } 
    }

    internal class SecretAccount
    {
        public string EncryptedAccount { get; set; }
        public byte[] IV { get; set; }
    }
}