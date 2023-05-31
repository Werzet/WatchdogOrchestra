using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WatchdogOrchestra.Configuration;

namespace WatchdogOrchestra.Controllers;

[Route("[controller]")]
public class ServerInstanceController : OrchestraControllerBase
{
	private readonly ServersConfiguration _serverConfiguration;
	private readonly HttpClient _httpClient;

	public ServerInstanceController(IOptions<ServersConfiguration> serverConfiguration, HttpClient httpClient)
	{
		_serverConfiguration = serverConfiguration.Value;
		_httpClient = httpClient;
	}

	[HttpGet]
	public List<InstanceConfiguration> GetList()
	{
		return _serverConfiguration.Instances;
	}

	[HttpPost("{name}/restart")]
	public async Task Restart(string name)
	{
		var instance = _serverConfiguration.Instances.FirstOrDefault(x => x.Name == name)
			?? throw new InstanceNotFoundException(name);

		var watchDogClient = GetClient(instance);

		await watchDogClient.RestartAsync(GetAuthorization(instance), instance.Name);
	}

	[HttpPost("{name}/update")]
	public async Task Update(string name)
	{
		var instance = _serverConfiguration.Instances.FirstOrDefault(x => x.Name == name)
			?? throw new InstanceNotFoundException(name);

		var watchDogClient = GetClient(instance);

		await watchDogClient.UpdateAsync(GetAuthorization(instance), instance.Name);
	}

	private Watchdog.Client GetClient(InstanceConfiguration instance)
	{
		return new Watchdog.Client(instance.Address, _httpClient);
	}

	private static string GetAuthorization(InstanceConfiguration instance)
	{
		return $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{instance.Name}:{instance.ApiToken}"))}";
	}
}