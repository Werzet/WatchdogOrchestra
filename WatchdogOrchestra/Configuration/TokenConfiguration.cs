using Microsoft.IdentityModel.Tokens;

namespace WatchdogOrchestra.Configuration;

public class TokenConfiguration
{
	public string Issuer { get; set; } = string.Empty;

	public string Audience { get; set; } = string.Empty;

	public string TokenKey { get; set; } = string.Empty;

	public SymmetricSecurityKey GetSymmetricSecurityKey()
	{
		if (string.IsNullOrWhiteSpace(TokenKey))
		{
			throw new ArgumentException("Ключ шифрования не инициализирован.");
		}

		return new SymmetricSecurityKey(Convert.FromBase64String(TokenKey));
	}
}
