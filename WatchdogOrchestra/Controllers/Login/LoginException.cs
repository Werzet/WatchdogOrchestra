namespace WatchdogOrchestra.Controllers.Login;

public class LoginException : Exception
{
	public LoginException()
		: base("Имя пользователя или пароль указаны неверно") { }
}