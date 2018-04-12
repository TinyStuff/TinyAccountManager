using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Security;
using Android.Security.Keystore;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Math;
using Java.Security;
using Java.Util;
using Javax.Crypto;
using Javax.Crypto.Spec;
using Javax.Security.Auth.X500;
using Newtonsoft.Json;
using TinyAccountManager.Abstraction;
using File = System.IO.File;

//using Calendar = Android.Icu.Util.Calendar;

namespace TinyAccountManager.Droid
{
    public class AndroidAccountManagerPreM : IAccountManager
    {
        private const string AndroidKeyStore = "AndroidKeyStore";
        private const string RSAMode =  "RSA/ECB/PKCS1Padding";
        private const string AESMode = "AES/ECB/PKCS7Padding";
        private const string EncryptedAESKey = "EncryptedAESKey";

        private readonly Context _context;
        private readonly KeyStore _keyStore;
        private readonly string _sharedPreferenceName;

        public AndroidAccountManagerPreM(Context context, string sharedPreferenceName)
        {
            _context = context;
            _sharedPreferenceName = sharedPreferenceName;

            _keyStore =  KeyStore.GetInstance(AndroidKeyStore);
            _keyStore.Load(null);
        }

        #region Public members
        public async Task Save(Account account)
        {
            if (string.IsNullOrWhiteSpace(account.ServiceId))
            {
                throw new Exception("serviceId must be set.");
            }

            GenerateAndStoreKey(account.ServiceId);

            var c = Cipher.GetInstance(AESMode, "BC");
            c.Init(CipherMode.EncryptMode, GetSecretKey(_context, account.ServiceId));
            var iv = c.GetIV();

            var json = JsonConvert.SerializeObject(account);

            var bytes = Encoding.UTF8.GetBytes(json);

            var encodedBytes = c.DoFinal(bytes);

            var base64String = Convert.ToBase64String(encodedBytes);

            var secretAccount = new SecretAccount()
            {
                EncryptedAccount = base64String,
                IV = iv
            };

            var secretAccountJson = JsonConvert.SerializeObject(secretAccount);

            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            var filePath = Path.Combine(documentsPath, account.ServiceId);
            System.IO.File.WriteAllText(filePath, secretAccountJson);
        }

        public async Task<Account> Get(string serviceId)
        {
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            var filePath = Path.Combine(documentsPath, serviceId);

            var secretAccountJson = File.ReadAllText(filePath);

            var secretAccount = JsonConvert.DeserializeObject<SecretAccount>(secretAccountJson);

            var cipher = Cipher.GetInstance(AESMode, "BC");
            cipher.Init(CipherMode.DecryptMode, GetSecretKey(_context, serviceId));

            var encryptedBytes = Convert.FromBase64String(secretAccount.EncryptedAccount);

            var decryptedBytes = cipher.DoFinal(encryptedBytes);

            var json = Encoding.UTF8.GetString(decryptedBytes);

            var account = JsonConvert.DeserializeObject<Account>(json);

            return account;
        }

        public async Task<bool> Exists(string serviceId)
        {
            try
            {
                var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

                var filePath = Path.Combine(documentsPath, serviceId);

                return File.Exists(filePath);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task Remove(string serviceId)
        {
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            var filePath = Path.Combine(documentsPath, serviceId);

            File.Delete(filePath);
        }
        #endregion

        #region Private members

        private IKey GetSecretKey(Context context, string keyAlias)
        {
            var pref = context.GetSharedPreferences(_sharedPreferenceName, FileCreationMode.Private);
            var enryptedKeyB64 = pref.GetString(EncryptedAESKey, null);
            // need to check null, omitted here
            var encryptedKey = Convert.FromBase64String(enryptedKeyB64);
            var key = RSADecrypt(encryptedKey, keyAlias);
            return new SecretKeySpec(key, "AES");
        }


        private void GenerateAndStoreKey(string keyAlias)
        {
            var pref = _context.GetSharedPreferences(_sharedPreferenceName, FileCreationMode.Private);
            var enryptedKeyB64 = pref.GetString(EncryptedAESKey, null);
            if (string.IsNullOrWhiteSpace(enryptedKeyB64))
            {
                var key = new byte[16];
                var secureRandom = new SecureRandom();
                secureRandom.NextBytes(key);
                var encryptedKey = RSAEncrypt(key, keyAlias);
                enryptedKeyB64 = Convert.ToBase64String(encryptedKey);
                var edit = pref.Edit();
                edit.PutString(EncryptedAESKey, enryptedKeyB64);
                edit.Commit();
            }
        }

        private void GenerateRSAKeyPairs(string alias)
        {
            // Generate the RSA key pairs
            if (!_keyStore.ContainsAlias(alias))
            {
                // Generate a key pair for encryption
                var start = Calendar.GetInstance(Locale.English);
                var end = Calendar.GetInstance(Locale.English);
                end.Add(CalendarField.Year, 30);
                var spec = new KeyPairGeneratorSpec.Builder(_context)
                    .SetAlias(alias)
                    .SetSubject(new X500Principal("CN=" + alias))
                    .SetSerialNumber(BigInteger.Ten)
                    .SetStartDate(start.Time)
                    .SetEndDate(end.Time)
                    .Build();
                var kpg = KeyPairGenerator.GetInstance(KeyProperties.KeyAlgorithmRsa, AndroidKeyStore);
                kpg.Initialize(spec);
                kpg.GenerateKeyPair();
            }
        }

        private byte[] RSAEncrypt(byte[] secret, string keyAlias)
        {
            var privateKeyEntry = (KeyStore.PrivateKeyEntry)_keyStore.GetEntry(keyAlias, null);

            if (privateKeyEntry == null)
            {
                GenerateRSAKeyPairs(keyAlias);
                privateKeyEntry = (KeyStore.PrivateKeyEntry)_keyStore.GetEntry(keyAlias, null);
            }

            // Encrypt the text
            var cipher = Cipher.GetInstance(RSAMode, "AndroidOpenSSL");
            cipher.Init(CipherMode.EncryptMode, privateKeyEntry.Certificate.PublicKey);

            return cipher.DoFinal(secret);
        }

        private byte[] RSADecrypt(byte[] encrypted, string keyAlias)
        {
            var privateKeyEntry = (KeyStore.PrivateKeyEntry)_keyStore.GetEntry(keyAlias, null);
            var cipher = Cipher.GetInstance(RSAMode, "AndroidOpenSSL");
            cipher.Init(CipherMode.DecryptMode, privateKeyEntry.PrivateKey);

            return cipher.DoFinal(encrypted);
        }

        #endregion
    }
}