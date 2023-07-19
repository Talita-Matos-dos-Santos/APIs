using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repository;

public class ProdutoRepository : Repository<Produto>, IProdutoRepository //herda da classe repository pois assim teremos acesso a toda aquela implementacao que foi feita nessa classe. Tbm vamos herdar de IProdutoRepository pra ter acesso ao metodo especifico que foi definido ali
{
    public ProdutoRepository(AppDbContext contexto) : base(contexto)
    { 
        //passa pra mim a classe base 
        //esse contexto foi necessario pois se nao o compilador reclama, visto que o Repository<T> implementa esse contexto tbm
    }

    public async Task<IEnumerable<Produto>> GetProdutosPorPreco()
    {
        return await Get().OrderBy(p => p.Preco).ToListAsync(); //da um get e ordena eles pelo preco e retorna uma lista ordenada dessa forma. 
        //esse metodo Get() usado é o da class Repository.
        
        //agr é um metodo assincrono
    }
    
    public async Task<PagedList<Produto>> GetProdutos(ProdutosParameters produtosParameters) 
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

        return await PagedList<Produto>.ToPagedList(Get()
                .OrderBy(p => p.ProdutoId),
            produtosParameters.PageNumber,
            produtosParameters.PageSize);
        //esse metodo retorna um pagedlist com todas essas informações passadas aqui e com as operacoes que serao feitas la na propria classe, como a paginação em si, feita na variavel items.

    }
}