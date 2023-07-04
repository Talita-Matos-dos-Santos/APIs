using APICatalogo.Models;

namespace APICatalogo.Repository;

public interface ICategoriaRepository : IRepository<Categoria>
{
    IEnumerable<Categoria> GetCategoriasProdutos(); //vai me retornar os produtos por categoria. Vou obter as categorias e os produtos das categorias. 
}