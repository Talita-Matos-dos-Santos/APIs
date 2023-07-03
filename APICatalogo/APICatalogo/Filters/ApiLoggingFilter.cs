using Microsoft.AspNetCore.Mvc.Filters;

namespace APICatalogo.Filters;

public class ApiLoggingFilter : IActionFilter
{
    private readonly ILogger<ApiLoggingFilter> _logger; //usamos a interface ILogger para definir uma instancia dessa interface pro meu filtro ApiLoggingFilter

    public ApiLoggingFilter(ILogger<ApiLoggingFilter> logger) //injetando a dependencia
    {
        _logger = logger; //obtendo a instancia injetada.
    }
    
    //executa antes da Action
    public void OnActionExecuting(ActionExecutingContext context)
    {
        //definindo o logger de algumas informacoes
        _logger.LogInformation("### Executando -> OnActionExecuting");
        _logger.LogInformation("################################");
        _logger.LogInformation($"{DateTime.Now.ToLongTimeString()}");
        _logger.LogInformation($"ModelState: {context.ModelState.IsValid}");
        _logger.LogInformation("#################################");

    }

    //executa depois da Action
    public void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation("### Executando -> OnActionExecuted");
        _logger.LogInformation("################################");
        _logger.LogInformation($"{DateTime.Now.ToLongTimeString()}");
        _logger.LogInformation("#################################");
    }
}