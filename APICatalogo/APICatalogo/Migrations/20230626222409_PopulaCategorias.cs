using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APICatalogo.Migrations
{
    public partial class PopulaCategorias : Migration
    {
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("Insert into Categorias(Nome, ImagemUrl) Values ('Bebidas', 'bebidas.jpg')"); //uso uma instancia de migrationbuilder e uso um metodo chamado Sql que vai executar uma instrução SQL no SGBD. Entre aspas eu tenho que por a instruçao sql. Primeiro eu coloco o nome das colunas que quero incluir e depois os valores
            mb.Sql("Insert into Categorias(Nome, ImagemUrl) Values('Lanches', 'lanches.jpg')");
            mb.Sql("Insert into Categorias(Nome, ImagemUrl) Values('Sobremesas', 'sobremesas.jpg')");

        }

        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("Delete from Categorias");
        }
    }
}
