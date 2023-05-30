using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration.UserSecrets;
using WatchdogOrchestra.Configuration;

namespace WatchdogOrchestra.Secrets;

public static class SecretsManager
{
	public static void CreateTokenKey()
	{
		var secretId = Assembly.GetExecutingAssembly().GetCustomAttribute<UserSecretsIdAttribute>()?.UserSecretsId;

		if (secretId == null)
		{
			throw new ArgumentNullException(nameof(secretId));
		}

		var secretPath = PathHelper.GetSecretsPathFromSecretsId(secretId);

		var secretsJson = File.ReadAllText(secretPath);

		var jDoc = JsonNode.Parse(secretsJson, new JsonNodeOptions
		{
			PropertyNameCaseInsensitive = true,
		});

		var secrets = jDoc["Security"];

		if (secrets == null)
		{
			secrets = new JsonObject();

			jDoc["Security"] = secrets;
		}

		var tokenKey = secrets[nameof(TokenConfiguration.TokenKey)];

		if (tokenKey == null || string.IsNullOrEmpty(tokenKey.GetValue<string>()))
		{
			secrets[nameof(TokenConfiguration.TokenKey)] = GenerateTokenKey();

			var newSecrets = jDoc.ToJsonString(new JsonSerializerOptions() { WriteIndented = true });

			File.WriteAllText(secretPath, newSecrets);
		}
	}

	private static string GenerateTokenKey()
	{
		using var generator = RandomNumberGenerator.Create();

		var bytes = new byte[2048];

		generator.GetBytes(bytes);

		return Encoding.UTF8.GetString(bytes);
	}
}
