using APICatalogo.Models;
using APICatalogo.Pagination;

namespace APICatalogo.Repository;

public interface IProdutoRepository : IRepository<Produto>
{
    //se eu nao tivesse nenhum metodo especifico, poderia deixar aqui vazio msm pq o mais importante é ele herdar de IRepository<Produto>
    //mas vamos colocar só mais um metodo aq.
    //alem da inclusao, exclusao, update e consultas eu tenho um metodo especifico para obter os produtos pelo preco dos produtos
    IEnumerable<Produto> GetProdutosPorPreco();

    PagedList<Produto> GetProdutos(ProdutosParameters produtosParameters); 
    //Antes retornava um IEnumerable de produto, agr será um PagedList de produto, pq é nele que eu vou atribuir valores para as novas propriedades e a paginação. 

}