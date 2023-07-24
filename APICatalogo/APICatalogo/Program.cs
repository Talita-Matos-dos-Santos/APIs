using System.Configuration;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using APICatalogo.Context;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Extensions;
using APICatalogo.Filters;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options=> options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); 
#region addJson//A parte do AddJson foi adicionada por mim para definir como o json deve lidar com referencias ciclicas na serializaçao. Nesse caso ele deve ignorar o objeto qnd detectar uma possivel referencia ciclica.
#endregion


builder.Services.AddEndpointsApiExplorer();

//registra o gerador swagger definindo um ou mais documentos
builder.Services.AddSwaggerGen(c=>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "APICatalogo",
        Description = "Catálogo de Produtos e Categorias",
        TermsOfService = new Uri("https://talita.net/terms"), //termo de licenca com endereco ficticio aq
        Contact = new OpenApiContact()
        {
            //td de ficticio tbm
            Name = "talita",
            Email = "talita@gmail.com",
            Url = new Uri("https://talita.net")
        },
        License = new OpenApiLicense()
        {
            Name = "Usar sobre LICX",
            Url = new Uri("https://talita.net/license")
        }
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"; //definindo o nome do arquivo que quero gerar. a partir do nome do executavel da minha aplicacao (que é APICatalogo) ele vai gerar o nome do arquivo XML. no caso aqui vai ficar APICatalogo.xml
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile); //aqui ta definindo o caminho onde esse arquivo xml sera armazenado 
    c.IncludeXmlComments(xmlPath); //aqui ele vai ler e injetar os comentarios xmls lidos a partir do nosso projeto nesse arquivo


    // c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    // {
    //     Name = "Authorization",
    //     Type = SecuritySchemeType.ApiKey,
    //     Scheme = "Bearer",
    //     BearerFormat = "JWT",
    //     In = ParameterLocation.Header,
    //     Description = "Header de autorização JWT usando o esquema Bearer. \r\n\r\nInforme: 'Bearer:'[espaço] e o seu token.\r\n\r\bExemplo: \'Bearer: 12345abcdef\'"
    // }); //aq definimos o esquema de segurança. Esses atributos ai foram tirados da documentaçao do proprio swagger
    // c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    // {
    //     {
    //         new OpenApiSecurityScheme()
    //         {
    //             Reference = new OpenApiReference()
    //             {
    //                 Type = ReferenceType.SecurityScheme,
    //                 Id = "Bearer"
    //                 //aq estao os requerimentos, que é so que o security deve ser scheme e o id bearer
    //             }
    //         },
    //         new string[] {}
    //     }
    // });
});

string mySqlConnectionStr = builder.Configuration.GetConnectionString("DefaultConnection"); //defaultconnection é o nome da string de conexao que eu defini no appsettings.json

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(mySqlConnectionStr, ServerVersion.AutoDetect(mySqlConnectionStr)));

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders(); //esse codigo habilita o Identity no meu projeto. Dps daq eu vou incluir os middlewares de autenticacao e de autorizacao

//JWT
//adiciona o manipulador de autenticacao e define o esquema de autenticacao usado: Bearer
//valida o emissor, a audiencia e a chave
//usando a chave secreta, valida a assinatura

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidAudience = builder.Configuration["TokenConfiguration:Audience"],
        ValidIssuer = builder.Configuration["TokenConfiguration:Issuer"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]))
    }); //toda vez que chegar uma requisicao com o token vai ser feita essa verificacao pra validar.

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified =
        true; //define que vai assumir a versao padrao quando nenhuma versao for informada pelo cliente
    options.DefaultApiVersion = new ApiVersion(1, 0); //define qual é a versao padrao 
    options.ReportApiVersions =
        true; //permite informar no responde do request a informaçao de compatibilidade de versao
    options.ApiVersionReader = new HeaderApiVersionReader("x-api-version"); //especifica a api por meio do header do cabeçalho http, evitando assim que fiquemos poluindo a url. 
});
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    //antes de eu adicionar esse trecho de código o swagger tava dando problema e não tava abrindo alegando que o erro era por conta de ter dois controllers com a "mesma rota" (TesteV1 e TesteV2)
    //colocando esse trecho de código um campo irá aparecer no swagger independente da forma que você escolheu para fazer o versionamento e você poderá digitar a versão nesse campo, mesmo onde você não versionou ele aparece, mas é só ignorar.
});


//builder.Services.AddScoped<ApiLoggingFilter>(); //configurei o servico com addscoped, que garante que o serviço vai ser criado uma única vez por requisição, entao cada requisicao obtem uma nova instancia do nosso filtro.


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

#region UnitOfWork
//aq eu vou registrar a interface, indicando ela, e toda vez que eu precisar de uma implementação dessa interface, ela vai me dar uma instância da minha UnitOfWork
//lembrando que: AddScoped - em uma aplicação web, CADA REQUEST cria um novo escopo de serviço separado.
//fazendo esse registro da interface, o contêiner DI controla todos os serviços e eles são liberados e descartados quando a sua vida útil terminar. 
#endregion

#region configurando contexto e string de conexao

//aq eu to usando uma instancia de webapplication na variavel builder, incluindo o meu contexto nos servicos com o metodo AddDbContext e passando o meu contexto AppDbContext e dps to configurando o meu contexto informando o provedor que é o UseMySql que é o provedor do Pomelo e passando a string de conexao q ta na variavel mySqlConnectionStr pra acessar o banco de dados MySql. Com isso eu inclui o serviço do contexto do EF Core no container de injecao de dependencia nativo. Dessa forma eu vou poder injetar uma instancia de AppDbContext nos lugares que eu precisar dela. Vou fazer isso quando criar os repositórios. Uso uma instancia do meu contexto pra acesssar os dados la no EF Core. 

#endregion

var mappingConfig = new MapperConfiguration(mc =>
{
    //crio uma nova configuracao incluindo o meu arquivo de configuracao mappingprofile
    mc.AddProfile(new MappingProfile());

});
IMapper mapper = mappingConfig.CreateMapper(); //habilito o meu mapeamento definido na configuração acima
builder.Services.AddSingleton(mapper); //registro como um serviço do tipo singleton, onde só vou ter uma instancia desse serviço para fazer requisições no meu projeto inteiro


//antes do build é onde eu vou adicionar os serviços de container, aq seria equivalente ao metodo configureservice da classe startup
var app = builder.Build();
//dps do build é onde eu configuro os middlewares. configuro o pipeline do request http, que seria equivalente ao metodo configure da class startup



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "APICatalogo");
    });
}

app.ConfigureExceptionHandler(); //adicionando o middleware de tratamento de erros 

app.UseHttpsRedirection(); //middleware para redirecionar para https

app.UseRouting(); //n sei pq motivo se ele nao disse, mas tive que add isso aq. O migration so funcionou qnd coloquei ele. Antes parece que tava alegando algum problema com o AddEntityFrameworkStores
app.UseAuthentication(); 
app.UseAuthorization(); //esses que começam com use sao middlewares tbm, aq é um middleware de autorizaçao.

app.MapControllers();

app.Run(); //os middlewares run significam que sao o final, é o ultimo q sera executado