using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SpdDotNet.Jwt.Rsa.Lib;

namespace SpdDotNet.Jwt.Rsa
{
    public class TokenValidatorPostConfigure : IPostConfigureOptions<JwtBearerOptions>
    {
        private readonly ILogger logger;
        private readonly RSA rsa;

        public TokenValidatorPostConfigure(ILogger<TokenValidatorPostConfigure> logger)
        {
            this.logger = logger;
            rsa = RSA.Create();
        }

        public void PostConfigure(string name, JwtBearerOptions options)
        {
            if (name != JwtBearerDefaults.AuthenticationScheme)
            {
                return;

            }

            var pemFileName = Environment.GetEnvironmentVariable(Defines.PublicKeyFileEnvironmentVariableName) ??
                              throw new ApplicationException(
                                  $"\"{Defines.PublicKeyFileEnvironmentVariableName}\" environment variable is not set or empty");

            logger.LogInformation($"Reading public key from file: {pemFileName}");

            var pemDataBytes = System.IO.File.ReadAllBytes(pemFileName);
            var pemDataString = Encoding.UTF8.GetString(pemDataBytes);

            // Use "-----BEGIN PUBLIC KEY-----" and "-----END PUBLIC KEY-----" headers for X509
            var b64 = pemDataString.Replace("-----BEGIN RSA PUBLIC KEY-----", string.Empty)
                .Replace("-----END RSA PUBLIC KEY-----", string.Empty)
                .Replace("\n", string.Empty)
                .Trim();

            var publicKey = Convert.FromBase64String(b64);

            // Use rsa.ImportSubjectPublicKeyInfo() for X509 keys
            rsa.ImportRSAPublicKey(publicKey, out var bytesRead);

            logger.LogInformation($"RSA public key bytes read: {bytesRead}");

            var securityKey = new RsaSecurityKey(rsa);

            logger.LogInformation($"RSA security key size: {securityKey.KeySize} bits");

            options.TokenValidationParameters.IssuerSigningKey = securityKey;
        }
    }
}
