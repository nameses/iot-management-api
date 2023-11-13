using iot_management_api.Configuration;
using iot_management_api.Helper;
using Microsoft.Extensions.Options;
using Moq;

namespace Iot_xunit_tests.Tests.Helper
{
    public class EncrypterTests
    {
        [Fact]
        public void Encrypt_ShouldEncryptPassword()
        {
            // Arrange
            var encryptionConfigMock = new Mock<IOptions<PasswordEncryption>>();
            encryptionConfigMock.Setup(config => config.Value).Returns(new PasswordEncryption
            {
                Key = "$2a$10$iQbE6XllxYBgNt/cktFC0u"
            });

            var encrypterWithSalt = new Encrypter(encryptionConfigMock.Object);
            var password = "password";

            // Act
            var encryptedPassword = encrypterWithSalt.Encrypt(password);

            // Assert
            Assert.NotNull(encryptedPassword);
            Assert.NotEqual(encryptedPassword, password);
        }
        [Fact]
        public void Encrypt_ShouldEncryptPasswordWithDifferentSaltsDifferently()
        {
            // Arrange
            var encryptionConfigMock1 = new Mock<IOptions<PasswordEncryption>>();
            encryptionConfigMock1.Setup(config => config.Value).Returns(new PasswordEncryption
            {
                Key = "$2a$10$iQbE6XllxYBgNt/cktFC0u"
            });
            var encryptionConfigMock2 = new Mock<IOptions<PasswordEncryption>>();
            encryptionConfigMock2.Setup(config => config.Value).Returns(new PasswordEncryption
            {
                Key = "$2a$10$iQbE6XllxYBgNt/cktFC2u"
            });

            var encrypterWithSalt1 = new Encrypter(encryptionConfigMock1.Object);
            var encrypterWithSalt2 = new Encrypter(encryptionConfigMock2.Object);
            var password = "password";

            // Act
            var encryptedPassword1 = encrypterWithSalt1.Encrypt(password);
            var encryptedPassword2 = encrypterWithSalt2.Encrypt(password);

            // Assert
            Assert.NotNull(encryptedPassword1);
            Assert.NotNull(encryptedPassword2);
            Assert.NotEqual(password, encryptedPassword1);
            Assert.NotEqual(password, encryptedPassword2);

            Assert.NotEqual(encryptedPassword1, encryptedPassword2);

        }
    }
}
