﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WatchdogOrchestra.Configuration;
using WatchdogOrchestra.Models;

namespace WatchdogOrchestra.Controllers.Login
{
	[ApiController]
	[Route("/[controller]")]
	public class LoginController : ControllerBase
	{
		private readonly TokenConfiguration _tokenOptions;
		private readonly LoginInfoConfiguration _loginConfiguration;

		public LoginController(IOptions<TokenConfiguration> tokenOptions, IOptions<LoginInfoConfiguration> loginConfiguration)
		{
			_tokenOptions = tokenOptions.Value;
			_loginConfiguration = loginConfiguration.Value;
		}

		[HttpPost]
		public LoginResponse Login([FromBody] LoginRequestParameters requestParameters)
		{
			var userData = _loginConfiguration.Logins.FirstOrDefault(x => x.UserName == requestParameters.UserName);

			if (userData != null && !string.IsNullOrWhiteSpace(userData.Password))
			{
				var hash = GetPasswordHash(requestParameters.Password);

				if (hash == userData.Password)
				{
					return GetToken(_tokenOptions.GetSymmetricSecurityKey());
				}
			}

			throw new LoginException();
		}

		private static string GetPasswordHash(string password)
		{
			return Convert.ToBase64String(SHA512.HashData(Encoding.ASCII.GetBytes(password)));
		}

		private LoginResponse GetToken(SymmetricSecurityKey securityKey)
		{
			var now = DateTime.UtcNow;

			var jwt = new JwtSecurityToken(
				issuer: _tokenOptions.Issuer,
				audience: _tokenOptions.Audience,
				notBefore: now,
				expires: now.AddMonths(1),
				signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256));

			var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

			return new LoginResponse
			{
				Token = encodedJwt 
			};
		}


	}
}
