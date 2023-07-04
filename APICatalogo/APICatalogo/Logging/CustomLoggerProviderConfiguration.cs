namespace APICatalogo.Logging;

public class CustomLoggerProviderConfiguration
{
    //classe de configuracao
    public LogLevel LogLevel { get; set; } = LogLevel.Warning; //ja to iniciando com o warning
    public int EventId { get; set; } = 0; //valor inicial 0
}