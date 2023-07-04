namespace APICatalogo.Repository;

//pra poder usar é necessário registrar como um Service no arquivo Program.cs
public interface IUnitOfWork
{
    //para obter uma instancia de cada repositorio que tenho no meu projeto. 
    IProdutoRepository ProdutoRepository { get; }
    ICategoriaRepository CategoriaRepository { get; }
    void Commit();
}