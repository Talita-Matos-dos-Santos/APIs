using System;
using System.Collections.Generic;
using System.Linq;
using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers;

[Route("api/[controller]")]
#region explicação sobre ROUTE

//Esse route define a base do endpoint que serão atendidos pelos métodos Action. A rota nesse caso foi definida com api + o nome do controlador, que é Produtos. Vai ficar /api/produtos. ESSA VAI SER A ROTA PADRÃO PRA ACESSAR TODOS OS ENDPOINTS DESSE CONTROLADOR pois ela está definida no inicio do controlador, como se estivesse sendo aplicada pra ProdutosController.
#endregion

[ApiController]
public class ProdutosController : ControllerBase
{
    #region instancia e injecao do AppDbContext

    //injetando uma instancia da classe appdbcontext no contrutor:
    /* private readonly AppDbContext _context;
     * public ProdutosController(AppDbContext context)
    {
       _context = context;
    } 
     */
    #endregion
    
    
    //vms substituir as instancias de appdbcontext para as instancias do unitofwork. Com isso vamos acessar os respectivod repositorios e em cada método dos controladores vms fazer os ajustes necessários. 
    private readonly IUnitOfWork _uof;
    public ProdutosController(IUnitOfWork context)
    {
        _uof = context;
    }

    [HttpGet("menorpreco")]
    public ActionResult<IEnumerable<Produto>> GetProdutosPrecos()
    {
        return _uof.ProdutoRepository.GetProdutosPorPreco().ToList(); //retorna os produtos ordenados do menor preco pro maior.
    }
    
    //esse método vai atender acionando a URL /produtos, pois n tem nada adicional aq em relação a rota, e como ficou definido q o padrao é /api/produtos
    [HttpGet] //indica que esse método vai responder uma requisição get
    public ActionResult<IEnumerable<Produto>> Get()
    {
        //O metodo async foi retirado daqui pois o unitofwork foi feito no metodo sincrono, entao pra nao ter problema...
        #region Usar ou não o método assíncrono?
        //Como estamos acessando o SGBD e isso é uma operação que não depende da minha aplicação, então a utilizaçao do assincronismo se justifica. O assincronismo é util para melhorar a experiência do usuário quando há alguma operação que demanda muito tempo para ser executada.
        //O ganho que ele dá é a execução em paralelo, assim você pode atender mais requisições 
        //A requisição específica não ficará mais rápida em hipótese alguma 
        #endregion
        #region async e await - explicação
        //a palavra async indica q é um método assincrono. a palavra await está indicando que nós queremos aguardar essa operaçao, enquanto isso os recursos usados para processar essa requisição poderão ser usados para processar outras requisições, enquanto essa operaçao (_context.Produtos.AsNoTracking().ToListAsync()) não terminar. 
        #endregion

        var produtos = _uof.ProdutoRepository.Get().ToList(); //uso a instancia de uof q foi injetada, entro no repositorio do controller q eu to, uso o metodo do repository necessário. Nesse caso é o Get(). Lembrando q produtorepository implementa o repository.
        if (produtos is null)
        {
            return NotFound("Produtos não encontrados!");

            #region explicaçao sobre o tipo de retorno desse metodo

            //esse notfound vem da classe abstrata ControllerBase
            //o tipo de retorno q tava esperando é ienumerable de produto. Pra resolver isso usamos o tipo actionresult pra poder retornar tanto action qnt a lista. O Action não retorna nada em específico. Agora, eu posso retornar ou uma lista ou todos os métodos pertencentes aos tipos de retorno suportados pelo Action.
            #endregion
             
        }
        return produtos;
    }

    #region explicação sobre roteamento

    //ja nesse metodo eu inclui um parametro Id (que é o id do produto que eu quero retornar). Esse parametro vai compor com a rota padrão definida no atributo ROUTE. 
    //Logo, pra acessar aq tera a url /api/produtos/id.
    //Agora eu coloquei uma restrição de rota. a rota so vai aceitar o id se ele for inteiro e se tiver um valor minimo de 1.
    #endregion
    [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
    #region explicação sobre ObterProduto
    //criei uma rota ObterProduto no método Post. Para acessar essa rota foi necessário adicionar esse Name aqui, que nada mais é que uma definição de uma rota nomeada. Ela diz que pra obter o produto pelo ID pode também usar e chamar a rota ObterProduto
    #endregion
    public ActionResult<Produto> Get(int id) 
    {
        #region QueryString e Get antigo
        //nesse exemplo, o id que vai entrar como parâmetro é o da cadeia de consulta(querystring), mas mesmo assim é necessário informar o id. Supondo que quero o produto com id == 1, ent eu teria que usar a URL -> /api/produtos/1?id=2 -> ele vai trazer o id == 2 pro parametro, pois assim eu defini quando coloquei [FromQuery]
        
        //como nesse método get eu quero obter um produto pelo Id, entao eu tenho que passar o id no request. Pra isso vamos informar a recepção desse Id no atributo HttpGet
        #endregion

        var produto = _uof.ProdutoRepository.GetById(p => p.ProdutoId == id); 
        if (produto is null)
        {
            return NotFound("Produto não encontrado!");
        }

        return produto;
    }

    //aq tbm ta padrao, n tem nada adicional. Portanto a rota será /api/produtos. Como o verbo é diferente do primeiro Get, msm tendo a mesma url que o Get nao tera nenhum problema. 
    [HttpPost]
    public ActionResult Post(Produto produto)
    {
        #region explicações
        //ao colocar só o actionresult eu não vou retornar um tipo. Com isso estou indicando para retornar apenas as mensagens de status HTTP
        //um objeto do tipo Produto vai vir através do Body da requisição. 
        //com a introdução do atributo [ApiController] no inicio dessa api não é mais necessário colocar ([FromBody] Produto produto) e a validação do modelo tbm não é necessária. Já é feito automaticamente. 
        #endregion
        
        _uof.ProdutoRepository.Add(produto); //uso o metodo Add do repositorio. Toda a logica desse metodo esta em Repository, sendo que repository é implementado por produtorepository
        _uof.Commit(); //esse metodo commit é do unitofwork
        #region explicaçao sobre add produto no contexto - antigo

        //como estamos usando o contexto do EF eu tenho que incluir essa informação (que um produto ta sendo adicionado) no contexto, já que estamos trabalhando no modo desconectado. o método Add() vai incluir esse produto no contexto. 
        //Até o momento estamos trabalhando na memória. Para persistir essa informações no SGBD, usamos o método SaveChanges(). Ou seja, com o SaveChange() os dados vão pra tabela, se nao fica so na memoria.
        #endregion
        
        
        //o tipo de retorno que é recomendado retornar pro método POST é o código de status HTTP 201 Create. Pra isso colocamos no cabeçalho location da resposta http a URL do novo recurso.

        return new CreatedAtRouteResult("ObterProduto", new { id = produto.ProdutoId },
            produto); 
        #region explicações
        //retorna um código de estado http 201 em caso de êxito. Vai adicionar um cabeçalho location à resposta e ele vai especificar a URI para obter esse produto que acabou de ser incluído. 
        
        //Ou seja, agora ele recebo o produto no contexto, persiste no banco de dados, retorna 201 Create e aciona a rota ObterProduto (que ele vai usar o metodo Get com o Id do produto).
        #endregion
    }

    [HttpPut("{id:int:min(1)}")] //colocando o id aqui eu to definindo o template de rota: /api/produtos/{id}. Dessa forma o valor do id vai ser mapeado para o parametro do método Put abaixo  
    public ActionResult Put(int id, Produto produto)
    {
        #region explicações do que acontece nesse método - antigo

        //ao usar o http post é feita uma atualização completa, tenho que informar tooodos os dados aqui nesse objeto produto. Isso é considerado uma desvantagem dessa abordagem, pois mesmo que eu vá alterar só um campo eu preciso enviar todos os campos pra fazer uma unica alteraçao. 
        //Se quisesse fazer uma att parcial eu tenho que usar um outro método http que é o Patch.
        
        
        //para atualizar um recurso existente: o id é passado na URL, enquanto o produto é enviado no body request

        //Respostas possiveis -> 200 OK, 204 NoContent (esse segundo significa que o servidor processou o request com sucesso mas não esta retornando nenhum conteudo)
        
        //qnd enviar os dados do Produto tbm vou ter q informar o id do produto
        #endregion
        
        if (id != produto.ProdutoId)
        {
            //verifica se o id que eu to informando como parametro é diferente do produtoId q eu to passando no body request
            return BadRequest(); //codigo status 400
        }
        
        _uof.ProdutoRepository.Update(produto); //note que o entry etc está tudo implementado no proprio update do repository, q é uma classe herdada pelo produtorepository
        _uof.Commit(); 
        
        #region estado modificado - Entry - antigo
        
        //como estamos trabalhando em um cenário desconectado, ele precisa ser informado de que a entidade Produto está num estado modificado pra ele poder alterar. 
        //Para isso, utilizamos o metodo Entry do contexto.
        //uso a instancia do contexto, o metodo entry passando qual o objeto e vou definir que seu estado vai estar sendo modificado. Com isso, o EF vai saber que essa entidade precisa ser persistida. 
        #endregion

        return Ok(produto); //coloca o status 200 e coloca os dados do produto q foram atualizados. 
    }

    [HttpDelete("{id:int:min(1)}")]
    public ActionResult Delete(int id)
    {
        var produto = _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);
        if (produto is null)
        {
            return NotFound("Produto não encontrado!");
        }

        _uof.ProdutoRepository.Delete(produto);
        _uof.Commit();

        return Ok(produto); //retorna 200 e o produto q foi excluído
    }

}

