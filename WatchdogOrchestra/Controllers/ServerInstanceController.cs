using Microsoft.AspNetCore.Mvc;

namespace WatchdogOrchestra.Controllers;

[Route("[controller]")]
public class ServerInstanceController : OrchestraControllerBase
{
	[HttpGet("list")]
	public void GetList()
	{

	}

	public async Task Restart()
	{

	}

	public async Task Update()
	{

	}
}
