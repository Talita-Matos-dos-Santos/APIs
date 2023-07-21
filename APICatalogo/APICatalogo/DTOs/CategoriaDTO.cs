using Newtonsoft.Json;

namespace APICatalogo.DTOs;

public class CategoriaDTO
{
    public int CategoriaId { get; set; }
    public string Nome { get; set; }
    public string ImagemUrl { get; set; }
    public ICollection<ProdutoDTO> Produtos { get; set; } //preciso dessa coleção de produtos pois tenho definido o método Action que me retorna os produtos para cada categoria.
}