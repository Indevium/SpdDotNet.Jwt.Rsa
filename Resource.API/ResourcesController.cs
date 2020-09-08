using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SpdDotNet.Jwt.Rsa
{
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [Route("/api/resources")]
    public class ResourcesController : ControllerBase
    {
        private readonly ILogger logger;

        public ResourcesController(ILogger<ResourcesController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult GetData()
        {
            var i = User.FindFirstValue(JwtRegisteredClaimNames.Jti);
            var a = User.FindFirstValue(JwtRegisteredClaimNames.Aud);
            var c = User.FindFirstValue("CustomClaim");

            logger.LogInformation($"Token index: {i}, audience: {a}, custom: {c}");

            return Ok(new {data = "SomeData", time = DateTimeOffset.UtcNow});
        }
    }
}
