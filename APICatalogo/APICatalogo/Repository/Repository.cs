using System.Linq.Expressions;
using APICatalogo.Context;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repository;

public class Repository<T> : IRepository<T> where T : class//o T é um tipo que só pode ser uma classe, nao vou passar nada no repositorio nem usar nenhum tipo que nao seja uma classe.
{
    protected AppDbContext _context;

    public Repository(AppDbContext contexto)
    {
        _context = contexto;
        //injetando uma instancia do meu contexto. Isso só é possível pois eu registrei essa classe (appdbcontext) como um serviço no arquivo program.cs
    }
    
    
    public IQueryable<T> Get()
    {
        return _context.Set<T>().AsNoTracking();
        //o método Set<T> do contexto retorna uma instância DbSet<T> para o acesso a entidades de determinado tipo no contexto.
        //AsNoTracking foi utilizado pois como é uma consulta eu quero desabilitar o rastreamento de entidades, aumentando o desempenho.
    }

    public async Task<T> GetById(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().AsNoTracking().SingleOrDefaultAsync(predicate);
        //temos o delegate func como parametro de entrada e sera usada uma expressao lambda do tipo para comparar o id do produto ou categoria como criterio.
        //o predicate é pra validar o criterio, se ele é false ou true
        
        //agora esse metodo é assincrono.
        //retorna um task<t>
    }

    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
        //recebe uma entidade to tipo e usam a instancia do contexto para realizar a operacao de add
    }

    public void Update(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified; //recebo uma entidade, defino seu estado como Modified, informando ao contexto que a entidade foi alterada.
        _context.Set<T>().Update(entity); //ai uso o update para atualizar a entidade.
    }

    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }
}