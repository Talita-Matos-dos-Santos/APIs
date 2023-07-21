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
    //[Authorize(AuthenticationSchemes = "Bearer")] //sem colocar isso aq qualquer um vai poder fazer requisicoes pra minha api de categoria
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] //na aula de swagger ele mostrou q tava assim, mas em momento nenhum anteriormente ele tinha colocado
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

        [HttpGet("{id:int}", Name = "ObterCategoria")]
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

        [HttpPost]
        public async Task<ActionResult> Post([FromBody]CategoriaDTO categoriaDto)
        {
            
            var categoria = _mapper.Map<Categoria>(categoriaDto);
            
            _uof.CategoriaRepository.Add(categoria);
            await _uof.Commit();

            var cat = _mapper.Map<CategoriaDTO>(categoria); 

            return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.CategoriaId }, cat); 
        }

        [HttpPut("{id:int}")]
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
