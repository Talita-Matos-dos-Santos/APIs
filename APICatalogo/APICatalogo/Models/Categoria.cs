using System.Collections.ObjectModel;

namespace APICatalogo.Models;

public class Categoria
{
    public int CategoriaId { get; set; } //EF por convencao vai adotar como chave primária
    public string? Nome { get; set; } //sem a interrogacao o compilador da o alerta que ta non nullable, isso pq la em APICatalogo.csproj tá definido que as propriedades por referencia deverao ser nullable. colocando a ? eu defino a propriedade como nullable.
    public string? ImagemUrl { get; set; }
    public ICollection<Produto>? Produtos { get; set; } //uma categoria pode ter muitos produtos. Eu acho que a classe q tem o ICollection é tida como a classe principal. 
    
    public Categoria()
    {
        Produtos = new Collection<Produto>();
        //é responsabilidade de classe de onde vc define a propriedade do tipo coleção, inicializar essa coleção.
    }
    
    
}