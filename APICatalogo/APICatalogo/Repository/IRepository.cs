using System.Linq.Expressions;

namespace APICatalogo.Repository;

public interface IRepository<T>
{
    //na interface temos só a assinatura dos métodos, q na classe concreta serão implementadas. 
    
    IQueryable<T> Get(); //o iqueryable ja permite que façamos chamadas assincronas, ent nao vou precisar colocar o task aq. La no controlador ja vou poder fazer essas chamadas assincronas
    Task<T> GetById(Expression<Func<T, bool>> predicate); //consultar por id
    
    //os metodos abaixo nao modificam dados, apenas rastreiam as alteraçoes nas entidades que sao persistidas usando savechanges. Dessa forma, nao é necessario colocar o Task, pois nao tem retorno
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
}