using Microsoft.Extensions.Logging;

namespace SpdDotNet.Jwt.Rsa.Lib
{
    public static class Defines
    {
        public const string Environment = "Production";

        public const bool AddKestrelHeaders = false;

        public const string CorsPolicyName = "AllAllow";

        public const LogLevel LogLevel = Microsoft.Extensions.Logging.LogLevel.Information;

        public const int AuthApiPort = 3000;
        public const int ResourceApiPort = 5000;

        public const long TokenTtl = 60 * 8;

        public const string PrivateKeyFileEnvironmentVariableName = "PRIVATE_KEY_FILE";
        public const string PublicKeyFileEnvironmentVariableName = "PUBLIC_KEY_FILE";

    }
}
