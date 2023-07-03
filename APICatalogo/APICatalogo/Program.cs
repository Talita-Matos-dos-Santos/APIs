using System.Text.Json.Serialization;
using APICatalogo.Context;
using APICatalogo.Extensions;
using APICatalogo.Filters;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options=> options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); //A parte do AddJson foi adicionada por mim para definir como o json deve lidar com referencias ciclicas na serializaçao. Nesse caso ele deve ignorar o objeto qnd detectar uma possivel referencia ciclica.

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string mySqlConnectionStr = builder.Configuration.GetConnectionString("DefaultConnection"); //defaultconnection é o nome da string de conexao que eu defini no appsettings.json

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(mySqlConnectionStr, ServerVersion.AutoDetect(mySqlConnectionStr)));

builder.Services.AddScoped<ApiLoggingFilter>(); //configurei o servico com addscoped, que garante que o serviço vai ser criado uma única vez por requisição, entao cada requisicao obtem uma nova instancia do nosso filtro.

#region configurando contexto e string de conexao

//aq eu to usando uma instancia de webapplication na variavel builder, incluindo o meu contexto nos servicos com o metodo AddDbContext e passando o meu contexto AppDbContext e dps to configurando o meu contexto informando o provedor que é o UseMySql que é o provedor do Pomelo e passando a string de conexao q ta na variavel mySqlConnectionStr pra acessar o banco de dados MySql. Com isso eu inclui o serviço do contexto do EF Core no container de injecao de dependencia nativo. Dessa forma eu vou poder injetar uma instancia de AppDbContext nos lugares que eu precisar dela. Vou fazer isso quando criar os repositórios. Uso uma instancia do meu contexto pra acesssar os dados la no EF Core. 

#endregion 



//antes do build é onde eu vou adicionar os serviços de container, aq seria equivalente ao metodo configureservice da classe startup
var app = builder.Build();
//dps do build é onde eu configuro os middlewares. configuro o pipeline do request http, que seria equivalente ao metodo configure da class startup



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureExceptionHandler(); //adicionando o middleware de tratamento de erros 

app.UseHttpsRedirection(); 

app.UseAuthorization(); //esses que começam com use sao middlewares tbm, aq é um middleware de autorizaçao.

app.MapControllers();

app.Run(); //os middlewares run significam que sao o final, é o ultimo q sera executado