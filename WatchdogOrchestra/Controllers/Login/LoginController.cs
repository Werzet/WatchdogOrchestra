using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WatchdogOrchestra.Configuration;
using WatchdogOrchestra.Models;

namespace WatchdogOrchestra.Controllers.Login
{
	[ApiController]
	[Route("[controller]")]
	public class LoginController : ControllerBase
	{
		private readonly TokenConfiguration _tokenOptions;

		public LoginController(IOptions<TokenConfiguration> tokenOptions)
		{
			_tokenOptions = tokenOptions.Value;
		}

		[HttpPost]
		public string Login([FromBody] LoginRequestParameters requestParameters)
		{
			if (requestParameters.UserName == "admin" && requestParameters.Password == "admin")
			{
				return GetToken(_tokenOptions.GetSymmetricSecurityKey());
			}

			throw new LoginException();
		}

		public string GetToken(SymmetricSecurityKey securityKey)
		{
			var now = DateTime.UtcNow;

			var jwt = new JwtSecurityToken(
				issuer: _tokenOptions.Issuer,
				audience: _tokenOptions.Audience,
				notBefore: now,
				expires: now.AddMonths(1),
				signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256));

			var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

			return encodedJwt;
		}


	}
}
