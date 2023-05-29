using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace WatchdogOrchestra.Configuration;

public class ConfigureJwtBearerAuthorization : IConfigureNamedOptions<JwtBearerOptions>
{
	private readonly TokenConfiguration _tokenOptions;

	public ConfigureJwtBearerAuthorization(IOptions<TokenConfiguration> tokenOptions)
	{
		_tokenOptions = tokenOptions.Value;
	}

	public void Configure(string? name, JwtBearerOptions options)
	{
		if (options == null)
		{
			throw new ArgumentNullException(nameof(options));
		}

		if (name == JwtBearerDefaults.AuthenticationScheme)
		{
			options.RequireHttpsMetadata = false;

			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidIssuer = _tokenOptions.Issuer,
				ValidateAudience = true,
				ValidAudience = _tokenOptions.Audience,
				ValidateLifetime = true,
				IssuerSigningKey = ,
				ValidateIssuerSigningKey = true
			};
		}
	}

	public void Configure(JwtBearerOptions options)
		=> Configure(string.Empty, options);
}
