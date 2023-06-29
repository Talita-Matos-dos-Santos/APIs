using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers;

[Route("api/[controller]")] //Esse route define a base do endpoint que serão atendidos pelos métodos Action. A rota nesse caso foi definida com api + o nome do controlador, que é Produtos. Vai ficar /api/produtos. ESSA VAI SER A ROTA PADRÃO PRA ACESSAR TODOS OS ENDPOINTS DESSE CONTROLADOR pois ela está definida no inicio do controlador, como se estivesse sendo aplicada pra ProdutosController.
[ApiController]
public class ProdutosController : ControllerBase
{
    //injetando uma instancia da classe appdbcontext no contrutor:

    private readonly AppDbContext _context;

    public ProdutosController(AppDbContext context)
    {
        _context = context;
    }

    //esse método vai atender acionando a URL /produtos, pois n tem nada adicional aq em relação a rota, e como ficou definido q o padrao é /api/produtos
    [HttpGet] //indica que esse método vai responder uma requisição get
    public ActionResult<IEnumerable<Produto>> Get()
    {
        //IEnumerable permite adiar a execução, ou seja, ele trabala por demanda. Usando o IEnumerable eu não preciso ter toda a coleção na memória. Ele é mais otimizado que o list nesse caso.

        var produtos = _context.Produtos.ToList();
        if (produtos is null)
        {
            return NotFound("Produtos não encontrados!"); //esse notfound vem da classe abstrata ControllerBase
            //o tipo de retorno q tava esperando é ienumerable de produto. Pra resolver isso usamos o tipo actionresult pra poder retornar tanto action qnt a lista. O Action não retorna nada em específico. Agora, eu posso retornar ou uma lista ou todos os métodos pertencentes aos tipos de retorno suportados pelo Action. 
        }
        return produtos;
    }

    //ja nesse metodo eu inclui um parametro Id (que é o id do produto que eu quero retornar). Esse parametro vai compor com a rota padrão definida no atributo ROUTE. 
    //Logo, pra acessar aq tera a url /api/produtos/id.
    //Agora eu coloquei uma restrição de rota. a rota so vai aceitar o id se ele for inteiro e se tiver um valor minimo de 1.
    [HttpGet("{id:int:min(1)}", Name = "ObterProduto")] //criei uma rota ObterProduto no método Post. Para acessar essa rota foi necessário adicionar esse Name aqui, que nada mais é que uma definição de uma rota nomeada. Ela diz que pra obter o produto pelo ID pode também usar e chamar a rota ObterProduto
    public ActionResult<Produto> Get(int id)
    {
        //como nesse método get eu quero obter um produto pelo Id, entao eu tenho que passar o id no request. Pra isso vamos informar a recepção desse Id no atributo HttpGet

        var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);
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
        if (produto is null)
        {
            return BadRequest();
            //ou seja, se me passar uma informação de um produto invalida, ele retorna o badrequest e nao inclui o produto.
        }
        
        
        //ao colocar só o actionresult eu não vou retornar um tipo. Com isso estou indicando para retornar apenas as mensagens de status HTTP
        //um objeto do tipo Produto vai vir através do Body da requisição. 
        //com a introdução do atributo [ApiController] no inicio dessa api não é mais necessário colocar ([FromBody] Produto produto) e a validação do modelo tbm não é necessária. Já é feito automaticamente. 
        
        _context.Produtos.Add(produto); //como estamos usando o contexto do EF eu tenho que incluir essa informação (que um produto ta sendo adicionado) no contexto, já que estamos trabalhando no modo desconectado. o método Add() vai incluir esse produto no contexto. 
        //Até o momento estamos trabalhando na memória. Para persistir essa informações no SGBD, usamos o método SaveChanges(). Ou seja, com o SaveChange() os dados vão pra tabela, se nao fica so na memoria.
        _context.SaveChanges();
        
        //o tipo de retorno que é recomendado retornar pro método POST é o código de status HTTP 201 Create. Pra isso colocamos no cabeçalho location da resposta http a URL do novo recurso.

        return new CreatedAtRouteResult("ObterProduto", new { id = produto.ProdutoId },
            produto); //retorna um código de estado http 201 em caso de êxito. Vai adicionar um cabeçalho location à resposta e ele vai especificar a URI para obter esse produto que acabou de ser incluído. 
        
        //Ou seja, agora ele recebo o produto no contexto, persiste no banco de dados, retorna 201 Create e aciona a rota ObterProduto (que ele vai usar o metodo Get com o Id do produto).
    }

    [HttpPut("{id:int:min(1)}")] //colocando o id aqui eu to definindo o template de rota: /api/produtos/{id}. Dessa forma o valor do id vai ser mapeado para o parametro do método Put abaixo  
    public ActionResult Put(int id, Produto produto)
    {
        //ao usar o http post é feita uma atualização completa, tenho que informar tooodos os dados aqui nesse objeto produto. Isso é considerado uma desvantagem dessa abordagem, pois mesmo que eu vá alterar só um campo eu preciso enviar todos os campos pra fazer uma unica alteraçao. 
        //Se quisesse fazer uma att parcial eu tenho que usar um outro método http que é o Patch.
        
        
        //para atualizar um recurso existente: o id é passado na URL, enquanto o produto é enviado no body request

        //Respostas possiveis -> 200 OK, 204 NoContent (esse segundo significa que o servidor processou o request com sucesso mas não esta retornando nenhum conteudo)
        
        //qnd enviar os dados do Produto tbm vou ter q informar o id do produto
        if (id != produto.ProdutoId)
        {
            //verifica se o id que eu to informando como parametro é diferente do produtoId q eu to passando no body request
            return BadRequest(); //codigo status 400
        }
        
        //como estamos trabalhando em um cenário desconectado, ele precisa ser informado de que a entidade Produto está num estado modificado pra ele poder alterar. 
        //Para isso, utilizamos o metodo Entry do contexto.
        _context.Entry(produto).State = EntityState.Modified;
        _context.SaveChanges(); //uso a instancia do contexto, o metodo entry passando qual o objeto e vou definir que seu estado vai estar sendo modificado. Com isso, o EF vai saber que essa entidade precisa ser persistida. 

        return Ok(produto); //coloca o status 200 e coloca os dados do produto q foram atualizados. 
    }

    [HttpDelete("{id:int:min(1)}")]
    public ActionResult Delete(int id)
    {
        var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);
        if (produto is null)
        {
            return NotFound("Produto não encontrado!");
        }

        _context.Produtos.Remove(produto);
        _context.SaveChanges();

        return Ok(produto); //retorna 200 e o produto q foi excluído
    }

}

