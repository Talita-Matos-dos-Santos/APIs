namespace APICatalogo.Repository;

//pra poder usar é necessário registrar como um Service no arquivo Program.cs
public interface IUnitOfWork
{
    //para obter uma instancia de cada repositorio que tenho no meu projeto. 
    IProdutoRepository ProdutoRepository { get; }
    ICategoriaRepository CategoriaRepository { get; }
    Task Commit(); //antes era void, agr é task. Qnd eu chamar o savechangesasync isso deve ser feito de forma assincrona, pois é o savechanges que vai la no banco de dados persistir os dados, entao é essa operacao que deve ser assincrona. 
    //o Add, update e delete nao vao no sgbd, por isso nao fizemos alteracoes nesses metodos (em irepository).
}