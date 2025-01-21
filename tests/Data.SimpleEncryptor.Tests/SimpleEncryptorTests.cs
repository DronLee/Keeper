using Data.Abstractions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Data.SimpleEncryptor.Tests
{
    /// <summary>
    /// Unit tests for SimpleEncryptor
    /// </summary>
    public class SimpleEncryptorTests
    {
        /// <summary>
        /// Checking what if we use different signatures for encryption same data, we get different results
        /// </summary>
        [TestCase("123", "1234")]
        [TestCase("12A", "123a")]
        [TestCase("", "-")]
        public void Encrypt_DifferentSignatures_DifferentResults(string signature1, string signature2)
        {
            var data = GetExampleDataRows();

            var encryptor1 = new SimpleEncryptor(signature1);
            var encryptor2 = new SimpleEncryptor(signature2);
            Assert.AreNotEqual(encryptor1.Encrypt(data), encryptor2.Encrypt(data));
        }

        /// <summary>
        /// Checking what if we use same signatures for encryption same data, we get same results
        /// </summary>
        [TestCase("123")]
        [TestCase("12A")]
        [TestCase("-")]
        [Parallelizable(ParallelScope.All)]
        public void Encrypt_SameSignatures_SameResults(string signature)
        {
            var data = GetExampleDataRows();

            var encryptor1 = new SimpleEncryptor(signature);
            var encryptor2 = new SimpleEncryptor(signature);
            Assert.AreEqual(encryptor1.Encrypt(data), encryptor2.Encrypt(data));
        }

        /// <summary>
        /// Checking what after decryption result is the same as before encryption, if we use same encryptor
        /// </summary>
        [TestCase("4567")]
        [TestCase("123a")]
        [TestCase("..")]
        public void Decrypt_SameDataBeforeEncrypt(string signature)
        {
            var data = GetExampleDataRows();

            var encryptor = new SimpleEncryptor(signature);
            var encryptedData = encryptor.Encrypt(data);
            var decryptedData = encryptor.Decrypt(encryptedData);

            Assert.AreEqual(JsonConvert.SerializeObject(data),
                JsonConvert.SerializeObject(decryptedData));
        }

        private DataRow[] GetExampleDataRows()
        {
            return new[]
            {
                new DataRow
                {
                    Login = "user1",
                    Name = "myPage",
                    Password = "Password",
                        Section = "firstSection"
                },
                new DataRow
                {
                    Login = "admin",
                    Name = "some page",
                    Password = "aaaa1",
                     Section = "secondSection"
                }
            };
        }
    }
}