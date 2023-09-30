using iot_management_api.Configuration;
using Microsoft.Extensions.Options;

namespace iot_management_api.Helper
{
    public class Encrypter
    {
        private readonly IOptions<PasswordEncryption> _settings;

        public Encrypter(IOptions<PasswordEncryption> settings)
        {
            _settings=settings;
        }

        public string Encrypt(string password)
        {
            string salt = _settings.Value.Key!;
            return BCrypt.Net.BCrypt.HashPassword(password, salt);
        }
    }
}
