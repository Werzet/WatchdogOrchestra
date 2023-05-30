using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WatchdogOrchestra.Configuration;

namespace WatchdogOrchestra.Controllers;

[Route("[controller]")]
public class ServerInstanceController : OrchestraControllerBase
{
	private readonly ServersConfiguration _serverConfiguration;

	public ServerInstanceController(IOptions<ServersConfiguration> serverConfiguration)
	{
		_serverConfiguration = serverConfiguration.Value;
	}

	[HttpGet("list")]
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

		await watchDogClient.RestartAsync(string.Empty, instance.Name);
	}

	[HttpPost("{name}/update")]
	public async Task Update(string name)
	{
		var instance = _serverConfiguration.Instances.FirstOrDefault(x => x.Name == name)
			?? throw new InstanceNotFoundException(name);

		var watchDogClient = GetClient(instance);

		await watchDogClient.UpdateAsync(string.Empty, instance.Name);
	}

	private static Watchdog.Client GetClient(InstanceConfiguration instance)
	{
		var client = new HttpClient();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{instance.Name}:{instance.ApiToken}")));

		return new Watchdog.Client(instance.Address, client);
	}
}