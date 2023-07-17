using Adapters.PostgresSQL.Settings;
using Npgsql;
using Polly;
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace Adapters.PostgresSQL.Connection
{
    public class SQLConnection : ISQLConnection
    {
        //private readonly string _connectionString;
        private readonly string _keypass = "umtesteaes";
        private readonly ILogger<SQLConnection> _logger;
        private readonly IDbConnection _session;


        public SQLConnection(ILogger<SQLConnection> logger, PostgresSettings settings)
        {
            _logger = logger;

            var connectionStringBuilder = new NpgsqlConnectionStringBuilder()
            {
                Host = settings.Host,
                Database = settings.Database,
                Username = settings.User,
                Password = settings.Password //DecryptPassword(settings.Password)
            };

      
            _session = new NpgsqlConnection(connectionStringBuilder.ToString());
            TryConnect();
        }

        public IDbConnection Connection()
        {
            if (_session.State != ConnectionState.Open)
                TryConnect();

            return _session;
        }

        private void TryConnect()
        {
            _logger.LogInformation("[Database] PostgresSQL tentando conectar...");

            var policy = Policy.Handle<PostgresException>()
                                   .Or<Exception>()
                                .WaitAndRetryForever(retryAttempt => TimeSpan.FromSeconds(5), (ex, time) =>
                                {
                                    _logger.LogWarning($"[Database] PostgresSQL nao conectou depois de Timeout {time.TotalSeconds:n1}s ERRO: {ex.Message}");
                                });

            policy.Execute(() =>
            {
                _session.Open();                
            });
        }

        public string DecryptPassword(string encryptpassword)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(encryptpassword);
                
                byte[] passwordBytes = Encoding.UTF8.GetBytes(_keypass);
                byte[] salt = new byte[16]; // Salt should be the same value used during encryption

                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(passwordBytes, salt, iterations: 1000, HashAlgorithmName.SHA256);
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);

                using (MemoryStream memoryStream = new MemoryStream(plainTextBytes))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }


        public  string Encrypt(string plainText, string password)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] salt = new byte[16]; // Salt should be a randomly generated value

                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(passwordBytes, salt, iterations: 1000, HashAlgorithmName.SHA256);
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    }

                    return Encoding.ASCII.GetString(memoryStream.ToArray());
                }
            }
        }

    }
}
