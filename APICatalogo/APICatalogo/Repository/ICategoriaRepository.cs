using APICatalogo.Models;
using APICatalogo.Pagination;

namespace APICatalogo.Repository;

public interface ICategoriaRepository : IRepository<Categoria>
{
    Task<PagedList<Categoria>> GetCategorias(CategoriasParameters categoriasParameters); //vai me retornar os produtos por categoria. Vou obter as categorias e os produtos das categorias. 

    Task<IEnumerable<Categoria>> GetCategoriasProdutos();
}