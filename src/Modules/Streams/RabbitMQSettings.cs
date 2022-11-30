namespace Streams;

public class RabbitMQSettings
{
    public string Host        { get; set; } = "localhost";
    public string UserName    { get; set; } = "guest";
    public string Password    { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
}