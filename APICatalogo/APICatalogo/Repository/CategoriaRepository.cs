using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repository;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext contexto) : base(contexto)
    {
        
    }

    public IEnumerable<Categoria> GetCategoriasProdutos()
    {
        return Get().Include(x => x.Produtos); //obtenho todas as categorias e incluo os produtos dessa categoria
    }

    public PagedList<Categoria> GetCategorias(CategoriasParameters categoriasParameters)
    {
        
            //return Get()
            //.OrderBy(on => on.Nome)
            //.Skip((produtosParameters.PageNumber - 1) * produtosParameters.PageSize)
            //.Take(produtosParameters.PageSize)
            //.ToList();

            #region skip e take - pagedlist tem explicacao melhor

            //tem explicaçao mto boa no pdf. Mas resumo do resumo: O método Skip é usado para pular uma determinada quantidade de produtos na lista. A quantidade de produtos pulados é calculada com base no número da página atual e no tamanho da página. Isso é útil para a paginação, onde você deseja exibir apenas uma parte dos resultados por vez.
            //O método Take é usado para selecionar uma determinada quantidade de produtos da lista. A quantidade de produtos selecionados é definida pelo tamanho da página especificado nos parâmetros. Novamente, isso é útil para a paginação, onde você deseja exibir apenas uma quantidade limitada de resultados por página.
            #endregion

            return PagedList<Categoria>.ToPagedList(Get()
                    .OrderBy(c => c.CategoriaId),
                categoriasParameters.PageNumber,
                categoriasParameters.PageSize);
            //esse metodo retorna um pagedlist com todas essas informações passadas aqui e com as operacoes que serao feitas la na propria classe, como a paginação em si, feita na variavel items.
            
    }
}