using System.Net;
using APICatalogo.Models;
using Microsoft.AspNetCore.Diagnostics;

namespace APICatalogo.Extensions;

public static class ApiExceptionMiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app) //recebo essa instancia
    {
        app.UseExceptionHandler(appError =>
        { //aq eu defino o middleware(useexceptionhandler) q é o codigo que vai fazer o tratamento de erro
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; //aq obtenho o status code 
                context.Response.ContentType = "application/json"; //definindo o tipo de retorno que vou usar, q nesse caso é json

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>(); //aq obtenho informacoes e detalhes do erro
                if (contextFeature != null) 
                {
                    await context.Response.WriteAsync(new ErrorDetails() //qnd for escrever a resposta
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = contextFeature.Error.Message,
                        Trace = contextFeature.Error.StackTrace
                    }.ToString());
                    //retorno o codigo de status, a mensagem e a pilha de erro.
                }
            });
        });
    }
}


