using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WatchdogOrchestra.Models;

namespace WatchdogOrchestra.Controllers.Login
{
	[ApiController]
	public class LoginController : ControllerBase
	{
		[HttpPost]
		public string Login([FromBody] LoginRequestParameters requestParameters)
		{
			if (requestParameters.UserName == "admin" && requestParameters.Password == "admin")
			{
				return GetToken(new SymmetricSecurityKey(GenerateTokenKey()));
			}

			throw new LoginException();
		}

		public string GetToken(SymmetricSecurityKey securityKey)
		{
			var now = DateTime.UtcNow;

			var jwt = new JwtSecurityToken(
				notBefore: now,
				expires: now.AddMonths(1),
				signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256));

			var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

			return encodedJwt;
		}

		private static byte[] GenerateTokenKey()
		{
			using var generator = RandomNumberGenerator.Create();

			var bytes = new byte[2048];

			generator.GetBytes(bytes);

			return bytes;
			//return Encoding.ASCII.GetString(bytes);
		}
	}
}
