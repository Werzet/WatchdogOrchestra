namespace WatchdogOrchestra.Configuration;

public class LoginInfoConfiguration
{
	public Login[] Logins { get; set; } = Array.Empty<Login>();
}

public class Login
{
	public string UserName { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
}