using APICatalogo.Models;
using APICatalogo.Pagination;

namespace APICatalogo.Repository;

public interface IProdutoRepository : IRepository<Produto>
{
    //se eu nao tivesse nenhum metodo especifico, poderia deixar aqui vazio msm pq o mais importante é ele herdar de IRepository<Produto>
    //mas vamos colocar só mais um metodo aq.
    //alem da inclusao, exclusao, update e consultas eu tenho um metodo especifico para obter os produtos pelo preco dos produtos
    IEnumerable<Produto> GetProdutosPorPreco();

    IEnumerable<Produto> GetProdutos(ProdutosParameters produtosParameters); //metodo para realizar a paginacao. trata e recebe os produtosparameters

}