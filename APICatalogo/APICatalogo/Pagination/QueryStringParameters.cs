namespace APICatalogo.Pagination;

public class QueryStringParameters
{
    private const int maxPageSize = 50; //esse é o número total de registros que eu retorno na paginação. Assim, toda vez que eu consultar na minha API se eu não informar nada pro PageSize, o numero max de registro q ela vai retornar é 50.
    public int PageNumber { get; set; } = 1; //se o usuario n informar nenhum valor, o valor inicial sera 1
    private int _pageSize = 10; //se o usuario n informar nenhum valor, o valor inicial sera 10

    public int PageSize
    {
        get
        {
            return _pageSize; //retorna o tamanho da pagina
        }
        set
        {
            _pageSize = (value > maxPageSize) ? maxPageSize : value; //qnd for atribuir um valor pra _pageSize, verifico se o valor que está sendo passado for maior que o valor máximo permitido, então eu vou atribuir o valor máximo. Se não for maior que o valor máximo, então eu atribuo o valor que foi passado mesmo.  
        }
    }
}