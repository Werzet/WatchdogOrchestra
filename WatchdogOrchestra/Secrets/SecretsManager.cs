using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using WatchdogOrchestra.Configuration;

namespace WatchdogOrchestra.Secrets;

public static class SecretsManager
{
	public static string FileName => "token.json";

	public static void CreateTokenKey()
	{
		var tokenJson = File.ReadAllText(FileName);

		var jDoc = JsonNode.Parse(tokenJson, new JsonNodeOptions
		{
			PropertyNameCaseInsensitive = true,
		}) ?? new JsonObject();

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

			var newTokenJson = jDoc.ToJsonString(new JsonSerializerOptions() { WriteIndented = true });

			File.WriteAllText(FileName, newTokenJson);
		}
	}

	private static string GenerateTokenKey()
	{
		using var generator = RandomNumberGenerator.Create();

		var bytes = new byte[2048];

		generator.GetBytes(bytes);

		return Convert.ToBase64String(bytes);
	}
}
