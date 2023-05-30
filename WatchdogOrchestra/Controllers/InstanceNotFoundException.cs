namespace WatchdogOrchestra.Controllers;

public class InstanceNotFoundException : Exception
{
    public InstanceNotFoundException(string instanceName)
        : base($"Сервер с именем {instanceName} не найден")
    { }
}
