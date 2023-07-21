using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace APICatalogo.Models;

public class Categoria
{
    public int CategoriaId { get; set; } //EF por convencao vai adotar como chave primária
    
    [Required] //o campo passa a ser obrigatorio, indica com NotNull
    [StringLength(80)] //define q o tamanho padrao maximo é de 80
    public string? Nome { get; set; } //sem a interrogacao o compilador da o alerta que ta non nullable, isso pq la em APICatalogo.csproj tá definido que as propriedades por referencia deverao ser nullable. colocando a ? eu defino a propriedade como nullable.
    
    [Required] //o campo é obrigatório
    [StringLength(300)] //tamanho maximo padrao de 300
    public string? ImagemUrl { get; set; }
    
    [JsonIgnore]
    public ICollection<Produto>? Produtos { get; set; } //uma categoria pode ter muitos produtos. Eu acho que a classe q tem o ICollection é tida como a classe principal. 
    
    public Categoria()
    {
        Produtos = new Collection<Produto>();
        //é responsabilidade de classe de onde vc define a propriedade do tipo coleção, inicializar essa coleção.
    }
}