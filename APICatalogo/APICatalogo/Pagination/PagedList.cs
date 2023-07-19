using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Pagination;

public class PagedList<T> : List<T>
{
    #region PRIVATE SET
    //o private set dessas propriedades abaixo significam que a propriedade só pode ser definida (ter valores atribuidos) aqui dentro dessa classe. Isso será feito qnd os valores cairem no metodo ToPagedList e criar um novo PagedList e esse valores que entrarao como parametro no ToPagedList e que foram calculados la tbm serão passados pro construtor de PagedList e dentro desse construtor eles terão os valores atribuidos/definidos.
    #endregion
    public int CurrentPage { get; private set; } //representa a página atual (pageNumber)
    public int TotalPages { get; private set; } //representa o número total de páginas existentes
    public int PageSize { get; private set; } //tamanho da pagina
    public int TotalCount { get; private set; }
    public bool HasPrevious => CurrentPage > 1; //HasPrevious vai ser true toda vez que CurrentPage for maior que 1.  indica se existe uma página anterior
    public bool HasNext => CurrentPage < TotalPages; //HasNext vai ser true toda vez que CurrentPage for menor que TotalPages. indica se existe uma próxima página

    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        //os parametros aq tem as informacoes q vou tratar na minha API
        /*
         * items: acho q aqui ta a lista de todos os registros já paginados.
         * count: representa o TOTAL de registros que será paginado
         * pageNumber: número da página
         * pageSize: tamanho da página (nro de items/registros que pode ter em uma mesma pagina)
         */

        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize); //total de paginas = numero de items / numero de items q pode ter em uma msm pagina
        AddRange(items); //incluindo os items na lista
    }

    public async static Task<PagedList<T>> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
    {
        //nesse metodo eu passo as informacoes da fonte de dados( source = get(), pelo q entendi ), do pagenumber e pagesize.
        
        //com a fonte eu consigo calcular o numero de items usando o metodo Count(), retorna qnts items eu tenho no total p fazer a paginacao.
        var count = source.Count();
        
        //abaixo é feito a paginação em si:
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(); //acho que o fato de estar indo ao SGBD é um ponto importante pra se decidir fazer de forma assincrona. 
        
        #region TAKE E SKIP
        //skip pagenumber - 1 * pageSize -> pule todas as paginas - 1 (1 = pagina q eu to nao é pra pular) -> supondo pagenumber = 3, e pagesize = 10 -> (3 - 1) * 10 -> pule 2 * 10 = 20. Ou seja, é pra pular os 20 primeiros registros, sendo q tem 10 registros por pagina e que temos 3 paginas e eu vou pular tds menos a que eu to agora, entao vou pular 2 paginas. 2 paginas de 10 registros cada.
        //take(pageSize) -> pega a qntdd de items (10) e coloca em uma pagina, a que eu to agr.
        #endregion

        return new PagedList<T>(items, count, pageNumber, pageSize); //note q o retorno desse metodo aq é um PagedList msm, aqui eu vou criar um novo a partir dos calculos feito acima e dos parametros que entraram. Daqui ja sai direto pro construtor dessa classe.
    }   






}