using System.Linq.Expressions;

namespace APICatalogo.Repository;

public interface IRepository<T>
{
    //na interface temos só a assinatura dos métodos, q na classe concreta serão implementadas. 
    
    IQueryable<T> Get(); //retornar uma lista de um tipo, que vai retornar um iqueryable. Retornando um iqueryable eu posso depois customizar essa consulta. 
    T GetById(Expression<Func<T, bool>> predicate); //consultar por id
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
}