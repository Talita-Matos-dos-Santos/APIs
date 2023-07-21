using System;
using System.Collections.Generic;
using System.Linq;
using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace APICatalogo.Controllers;

//[Authorize(AuthenticationSchemes = "Bearer")] //sem colocar isso aq qualquer um vai poder fazer requisicoes pra minha api de produtos
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

    private readonly IMapper _mapper; //pegando uma instancia do automapper e colocando em uma variavel
    private readonly IUnitOfWork _uof;
    public ProdutosController(IUnitOfWork context, IMapper mapper)
    {
        //fazendo uma injecao de dependencia e obtendo a instancia injetada na variavel _mapper.
        _uof = context;
        _mapper = mapper;
    }

    [HttpGet("menorpreco")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosPrecos()
    {
        var produtos = await _uof.ProdutoRepository.GetProdutosPorPreco(); 
        
        var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos); 
        
        return produtosDto;
    }
    
    [HttpGet] 
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] ProdutosParameters produtosParameters)
    {
        //fromquery significa que vou receber esse valor da querystring que fica na url/no endpoint de qnd faço a consulta
        //https://localhost:7073/api/produtos?pageNumber=2&pageSize=2
        
        var produtos = await _uof.ProdutoRepository.GetProdutos(produtosParameters); 
        
        var metadata = new
        {
            produtos.TotalCount,
            produtos.PageSize,
            produtos.CurrentPage,
            produtos.TotalPages,
            produtos.HasNext,
            produtos.HasPrevious
        };
        
        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata)); 
        
        
        var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos); //fazendo o mapeando dos produtos para uma lista de produtoDTO.

        return produtosDto;
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
    public async Task<ActionResult<ProdutoDTO>> Get(int id) 
    {
        #region QueryString e Get antigo
        //nesse exemplo, o id que vai entrar como parâmetro é o da cadeia de consulta(querystring), mas mesmo assim é necessário informar o id. Supondo que quero o produto com id == 1, ent eu teria que usar a URL -> /api/produtos/1?id=2 -> ele vai trazer o id == 2 pro parametro, pois assim eu defini quando coloquei [FromQuery]
        
        //como nesse método get eu quero obter um produto pelo Id, entao eu tenho que passar o id no request. Pra isso vamos informar a recepção desse Id no atributo HttpGet
        #endregion

        var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);
        var produtoDto = _mapper.Map<ProdutoDTO>(produto); //lembrando que aqui não é uma lista. é um unico produto
        if (produtoDto is null)
        {
            return NotFound("Produto não encontrado!");
        }

        return produtoDto;
    }
    
    [HttpPost]
    public async Task<ActionResult> Post(ProdutoDTO produtoDto)
    {

        var produto = _mapper.Map<Produto>(produtoDto); 

        _uof.ProdutoRepository.Add(produto); 
        //ate aqui a solicitaçao ta rodando de forma sincrona, mas qnd for acionar o commit tem que partir pro assincrono, pois foi assim que ficou definido no UnitOfWork e tbm nos repositorios. os metodos add, update e delete nao sao assincronos, mas o commit é.
        await _uof.Commit(); 
        

        var produtoDTO = _mapper.Map<ProdutoDTO>(produto); 

        return new CreatedAtRouteResult("ObterProduto", new { id = produto.ProdutoId },
            produtoDTO); 
        #region explicações
        //retorna um código de estado http 201 em caso de êxito. Vai adicionar um cabeçalho location à resposta e ele vai especificar a URI para obter esse produto que acabou de ser incluído. 
        
        //Ou seja, agora ele recebo o produto no contexto, persiste no banco de dados, retorna 201 Create e aciona a rota ObterProduto (que ele vai usar o metodo Get com o Id do produto).
        #endregion
    }

    [HttpPut("{id:int:min(1)}")] //colocando o id aqui eu to definindo o template de rota: /api/produtos/{id}. Dessa forma o valor do id vai ser mapeado para o parametro do método Put abaixo  
    public async Task<ActionResult> Put(int id, ProdutoDTO produtoDto)
    {
        #region explicações do que acontece nesse método - antigo

        //ao usar o http post é feita uma atualização completa, tenho que informar tooodos os dados aqui nesse objeto produto. Isso é considerado uma desvantagem dessa abordagem, pois mesmo que eu vá alterar só um campo eu preciso enviar todos os campos pra fazer uma unica alteraçao. 
        //Se quisesse fazer uma att parcial eu tenho que usar um outro método http que é o Patch.
        
        
        //para atualizar um recurso existente: o id é passado na URL, enquanto o produto é enviado no body request

        //Respostas possiveis -> 200 OK, 204 NoContent (esse segundo significa que o servidor processou o request com sucesso mas não esta retornando nenhum conteudo)
        
        //qnd enviar os dados do Produto tbm vou ter q informar o id do produto
        #endregion
        
        if (id != produtoDto.ProdutoId)
        {
            return BadRequest(); //codigo status 400
        }

        var produto = _mapper.Map<Produto>(produtoDto);
        
        _uof.ProdutoRepository.Update(produto); 
        
        //ate aqui a solicitaçao ta rodando de forma sincrona, mas qnd for acionar o commit tem que partir pro assincrono, pois foi assim que ficou definido no UnitOfWork e tbm nos repositorios. os metodos add, update e delete nao sao assincronos, mas o commit é.
        await _uof.Commit(); 
        
        #region estado modificado - Entry - antigo
        
        //como estamos trabalhando em um cenário desconectado, ele precisa ser informado de que a entidade Produto está num estado modificado pra ele poder alterar. 
        //Para isso, utilizamos o metodo Entry do contexto.
        //uso a instancia do contexto, o metodo entry passando qual o objeto e vou definir que seu estado vai estar sendo modificado. Com isso, o EF vai saber que essa entidade precisa ser persistida. 
        #endregion

        var produtoDTO = _mapper.Map<ProdutoDTO>(produto);
        return Ok(produtoDTO); 
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult<ProdutoDTO>> Delete(int id)
    {
        var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id); //como aq estou obtendo o getbyid eu tbm faço isso de forma assincrona, pois ele foi definido assim.
        if (produto is null)
        {
            return NotFound("Produto não encontrado!");
        }

        _uof.ProdutoRepository.Delete(produto);
        await _uof.Commit();

        var produtoDto = _mapper.Map<ProdutoDTO>(produto);
        return Ok(produtoDto); //retorna 200 e o produto q foi excluído
    }

}

