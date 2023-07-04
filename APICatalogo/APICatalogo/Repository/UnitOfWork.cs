using APICatalogo.Context;

namespace APICatalogo.Repository;
//pra poder usar é necessário registrar como um Service no arquivo Program.cs
public class UnitOfWork : IUnitOfWork
{
    //primeiro vou definir uma propriedade do tipo de instância de cada repositório
    private ProdutoRepository _produtoRepo;

    private CategoriaRepository _categoriaRepo;
    
    //dps vou precisar de uma instancia de dbcontext que vai ser injetada no construtor
    public AppDbContext _context;
    
    public UnitOfWork(AppDbContext contexto)
    {
        //aq vms fazer a injecao de dependencia de dbcontext
        _context = contexto;
    }
    
    public IProdutoRepository ProdutoRepository
    { //aq implementa oq tem no IUnit
        get
        {
            return _produtoRepo = _produtoRepo ?? new ProdutoRepository(_context); //aq eu to verificando se uma instancia de repositorio é nula, se for nula eu passo uma nova instância do contexto que foi injetado la em cima. Caso contrário fica mesmo a instancia que ja existe no meu repositório.
        }
    }

    public ICategoriaRepository CategoriaRepository
    {
        //aq implementa oq tem no IUnit
        get
        {
            return _categoriaRepo = _categoriaRepo ?? new CategoriaRepository(_context);
        }
    } 
    
    public void Commit()
    {
        //aq implementa oq tem no IUnit
        _context.SaveChanges(); //o savechanges vai persistir as informacoes no banco de dados 
    }

    public void Dispose()
    {
        //serve pra liberar os recursos do meu contexto que eu to injetando.
        _context.Dispose();
    }
}