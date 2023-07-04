using APICatalogo.Context;
using APICatalogo.Models;

namespace APICatalogo.Repository;

public class ProdutoRepository : Repository<Produto>, IProdutoRepository //herda da classe repository pois assim teremos acesso a toda aquela implementacao que foi feita nessa classe. Tbm vamos herdar de IProdutoRepository pra ter acesso ao metodo especifico que foi definido ali
{
    public ProdutoRepository(AppDbContext contexto) : base(contexto)
    { 
        //passa pra mim a classe base 
        //esse contexto foi necessario pois se nao o compilador reclama, visto que o Repository<T> implementa esse contexto tbm
    }

    public IEnumerable<Produto> GetProdutosPorPreco()
    {
        return Get().OrderBy(p => p.Preco).ToList(); //da um get e ordena eles pelo preco e retorna uma lista ordenada dessa forma. 
        //esse metodo Get() usado é o da class Repository.
    }
}