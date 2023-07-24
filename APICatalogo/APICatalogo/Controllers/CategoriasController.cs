using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

namespace APICatalogo.Controllers
{
    [Produces("application/json")]
    //[Authorize(AuthenticationSchemes = "Bearer")] //sem colocar isso aq qualquer um vai poder fazer requisicoes pra minha api de categoria
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] //na aula de swagger ele mostrou q tava assim, mas em momento nenhum anteriormente ele tinha colocado
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uof;

        public CategoriasController(IUnitOfWork context, IMapper mapper)
        {
            _uof = context;
            _mapper = mapper;
        }

        [HttpGet("produtos")] 
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasProdutos()
        {
            var categorias = await _uof.CategoriaRepository.GetCategoriasProdutos();
            var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);
            return categoriasDto;
        }
        
        [HttpGet] 
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get([FromQuery] CategoriasParameters categoriasParameters)
        {
            var categorias = await _uof.CategoriaRepository.GetCategorias(categoriasParameters);
            if (categorias is null)
            {
                return NotFound("Categorias não encontrados!");
            }
            
            var metadata = new
            {
                categorias.TotalCount,
                categorias.PageSize,
                categorias.CurrentPage,
                categorias.TotalPages,
                categorias.HasNext,
                categorias.HasPrevious
            };
        
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            
            
            var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);
            return categoriasDto;

        }

        ///<summary>
        /// Obtem uma Categoria pelo seu Id
        /// </summary>
        /// <param name="id">codigo do categoria</param>
        /// <returns>Objetos Categoria</returns>
        /// 
        [HttpGet("{id:int}", Name = "ObterCategoria")]
        [ProducesResponseType(typeof(ProdutoDTO), StatusCodes.Status200OK)] // Se estiver retornando o objeto ProdutoDTO é pq deu certo e aí retorna o codigo de status 200 ok tambem
        [ProducesResponseType(StatusCodes.Status404NotFound)] //Se não estiver retornando, retorna o código de status 404 not found, pq de certo não vai ter encontrado o id e por isso caiu no not found 
        public async Task<ActionResult<CategoriaDTO>> Get(int id)
        {
            var categoria = await _uof.CategoriaRepository.GetById(p => p.CategoriaId == id);
            if (categoria == null)
            {
                return NotFound("Categoria não encontrada!");
            }

            var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);
            return Ok(categoriaDto);
        }

        
        ///<summary>
        /// Inclui uma nova categoria
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        /// 
        ///     POST api/categorias
        ///     {
        ///         "categoriaId": 1,
        ///         "nome": "categoria1",
        ///         "imagemUrl": "http://teste.net/1.jpg" 
        ///     }
        /// </remarks>
        /// <param name="categoriaDto">objeto categoria</param>
        /// <returns>objeto Categoria incluida</returns>
        /// <remarks>Retorna um objeto Categoria incluído</remarks>
        /// 
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)] //se a criacao deu bom
        [ProducesResponseType(StatusCodes.Status400BadRequest)] //se nao criou por algum motivo
        public async Task<ActionResult> Post([FromBody]CategoriaDTO categoriaDto)
        {
            
            var categoria = _mapper.Map<Categoria>(categoriaDto);
            
            _uof.CategoriaRepository.Add(categoria);
            await _uof.Commit();

            var cat = _mapper.Map<CategoriaDTO>(categoria); 

            return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.CategoriaId }, cat); 
        }

        [HttpPut("{id:int}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))] //inclui o 204 (success), 404 (not found) e 400 (bad request) e o default (error)
        public async Task<ActionResult> Put(int id, CategoriaDTO categoriaDto)
        {
            if (id != categoriaDto.CategoriaId)
            {
                return BadRequest();
            }
            
            var categoria = _mapper.Map<Categoria>(categoriaDto);
            
            _uof.CategoriaRepository.Update(categoria);
            
            await _uof.Commit();

            var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);
            return Ok(categoriaDTO); 
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CategoriaDTO>> Delete(int id)
        {
            var categoria = await _uof.CategoriaRepository.GetById(p => p.CategoriaId == id);
            if (categoria == null)
            {
                return NotFound("Categoria não encontrada!");
            }

            _uof.CategoriaRepository.Delete(categoria);
            await _uof.Commit();

            var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);
            return Ok(categoriaDTO);
        }
    }
}
