using APICatalogo.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string mySqlConnectionStr = builder.Configuration.GetConnectionString("DefaultConnection"); //defaultconnection é o nome da string de conexao que eu defini no appsettings.json

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(mySqlConnectionStr, ServerVersion.AutoDetect(mySqlConnectionStr)));

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();