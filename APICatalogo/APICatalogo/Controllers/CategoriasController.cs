using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("produtos")] //pra caracterizar esse endpoint vms definir aq uma rota. Pra acessar esse endpoint vou ter que informar o nome do controlador (que é categorias) e /produtos, ai ele vai chegar aq. Se eu não fizesse isso aq teriamos um problema, pois teria 2 endpoints de httpGet com a mesma rota, que é o metodo imediatamente abaixo desse e esse aq. 
        public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
        {
            //o método de extensão Include permite carregar entidades relacionadas.
            return _context.Categorias.Include(c=> c.Produtos).ToList(); //vai obter a categoria e junto com ela tbm vai carregar os produtos dessa categoria. lembrando q por padrao nao carregaria
        }
        
        [HttpGet] //como n to colocando nada aq de rota, esse get aq vai atender ao endpoint "/categorias" somente, pois o [Route("controller")] define isso. O endpoint inicial é o nome do controlador, que nesse caso aqui é CategoriasController, só que o sufixo Controller é ignorado.
        public ActionResult<IEnumerable<Categoria>> Get()
        {
            var produtos = _context.Categorias.AsNoTracking().ToList(); //coloquei o AsNoTracking aqui pq tenho certeza que nesse get os objetos não precisam ser alterados, ent nao tem necessidade de o EF ficar acompanhando o objeto. 
            if (produtos is null)
            {
                return NotFound("Produtos não encontrados!");
            }
            return produtos;

        }

        [HttpGet("{id:int}", Name = "ObterCategoria")] //ja defini a rota nomeada ObterCategoria q eu vou usar qnd criar o Post
        public ActionResult<Categoria> Get(int id)
        {
            var categoria = _context.Categorias.FirstOrDefault(p => p.CategoriaId == id);
            if (categoria == null)
            {
                return NotFound("Categoria não encontrada!");
            }
            return Ok(categoria);
        }

        [HttpPost]
        public ActionResult Post(Categoria categoria)
        {
            if (categoria is null)
            {
                return BadRequest();
            }

            _context.Categorias.Add(categoria);
            _context.SaveChanges();

            return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.CategoriaId }, categoria); //retorno 201 created e chamo ObterCategoria passando id da categoria que foi recem criada e ele vai me retornar a categoria que eu acabei de adicionar.
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Categoria categoria)
        {
            if (id != categoria.CategoriaId)
            {
                return BadRequest();
            }
            
            _context.Entry(categoria).State = EntityState.Modified; //uso o metodo entry e altero o estado da categoria p modified, ai o ef vai entender q essa categoria foi modificada 
            _context.SaveChanges(); //ai eu posso salvar no sgbd

            return Ok(categoria); //e retornar a categoria
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var categoria = _context.Categorias.FirstOrDefault(p => p.CategoriaId == id);
            if (categoria == null)
            {
                return NotFound("Categoria não encontrada!");
            }

            _context.Categorias.Remove(categoria);
            _context.SaveChanges();

            return Ok(categoria);
        }
    }
}
