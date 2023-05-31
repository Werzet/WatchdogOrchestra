using System.ComponentModel.DataAnnotations;

namespace WatchdogOrchestra.Models;

public class LoginRequestParameters
{
	[Required(AllowEmptyStrings = false)]
	public string UserName { get; set; } = string.Empty;

	[Required(AllowEmptyStrings = false)]
	public string Password { get; set; } = string.Empty;
}
