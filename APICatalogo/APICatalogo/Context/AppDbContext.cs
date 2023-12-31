using APICatalogo.Models;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        //DbContextOptions<AppDbContext> é pra poder configurar o meu contexto no ef e essa configuraçao vai ser feita na classe base que é o DbContext
    }

    public DbSet<Categoria>? Categorias { get; set; }
    public DbSet<Produto>? Produtos { get; set; }
    
    //feito isso, precisamos informar qual a string de conexao que sera usada para fazer a comunicação com o banco de dados MySql
}