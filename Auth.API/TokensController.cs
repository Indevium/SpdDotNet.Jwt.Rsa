using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SpdDotNet.Jwt.Rsa.Lib;

namespace SpdDotNet.Jwt.Rsa
{
    [ApiController]
    [Produces("application/json")]
    [Route("/api/tokens")]
    public class TokensController : ControllerBase
    {
        private readonly ILogger logger;

        public TokensController(ILogger<TokensController> logger)
        {
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> IssueToken()
        {
            var pemFileName = Environment.GetEnvironmentVariable(Defines.PrivateKeyFileEnvironmentVariableName) ??
                              throw new ApplicationException(
                                  $"\"{Defines.PrivateKeyFileEnvironmentVariableName}\" environment variable is not set or empty");

            logger.LogInformation($"Reading private key from file: {pemFileName}");

            var pemDataBytes = await System.IO.File.ReadAllBytesAsync(pemFileName);
            var pemDataString = Encoding.UTF8.GetString(pemDataBytes);

            // Use "-----BEGIN PRIVATE KEY-----" and "-----END PRIVATE KEY-----" headers for PKCS8
            var b64 = pemDataString.Replace("-----BEGIN RSA PRIVATE KEY-----", string.Empty)
                .Replace("-----END RSA PRIVATE KEY-----", string.Empty)
                .Replace("\n", string.Empty)
                .Trim();

            var privateKey = Convert.FromBase64String(b64);

            var rsa = System.Security.Cryptography.RSA.Create();

            // Use rsa.ImportPkcs8PrivateKey(privateKey, out var bytesRead) for PKCS8 keys
            rsa.ImportRSAPrivateKey(privateKey, out var bytesRead);

            logger.LogInformation($"RSA private key bytes read: {bytesRead}");

            var securityKey = new RsaSecurityKey(rsa);

            logger.LogInformation($"RSA security key size: {securityKey.KeySize} bits");

            // Use preferred sign algorithm
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);

            var jwtHeader = new JwtHeader(signingCredentials);

            const string jti = "TokenIndex";

            var jwtPayload = new JwtPayload
            {
                { JwtRegisteredClaimNames.Jti, jti },
                { JwtRegisteredClaimNames.Sub, "TokenSubject" },
                { JwtRegisteredClaimNames.Aud, "TokenAudience" },
                { "CustomClaim", "SomeData" },
            };

            var i = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var e = i + Defines.TokenTtl;

            var iat = new Claim(JwtRegisteredClaimNames.Iat, i.ToString(), ClaimValueTypes.Integer64);
            var exp = new Claim(JwtRegisteredClaimNames.Exp, e.ToString(), ClaimValueTypes.Integer64);

            jwtPayload.AddClaim(iat);
            jwtPayload.AddClaim(exp);

            var token = new JwtSecurityToken(jwtHeader, jwtPayload);
            var tokenHandler = new JwtSecurityTokenHandler();
            var accessToken = tokenHandler.WriteToken(token);

            return Ok(new {accessToken});
        }
    }
}
