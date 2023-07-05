using APICatalogo.Models;
using AutoMapper;

namespace APICatalogo.DTOs.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Produto, ProdutoDTO>().ReverseMap(); //preciso mapear de produto pra produtodto e vice-versa
        CreateMap<Categoria, CategoriaDTO>().ReverseMap();
    }
}