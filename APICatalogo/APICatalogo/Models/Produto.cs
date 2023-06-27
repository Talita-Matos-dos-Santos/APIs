using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace APICatalogo.Models;

public class Produto
{
    public int ProdutoId { get; set; } //chave primária
    
    [Required] //campo obrigatorio
    [StringLength(80)] //tamanho max de 80
    public string? Nome { get; set; } //string é tipo por referencia, por isso precisa do ? q deve ser nullable
    
    [Required] //campo not null
    [StringLength(300)] //tamanho max de 300
    public string? Descricao { get; set; }
    
    [Column(TypeName = "decimal(10,2)")] //definiu o tamanho de 10 digitos com 2 casas decimais. o Typename é decimal
    public decimal Preco { get; set; }
    
    [Required]
    [StringLength(300)]
    public string? ImagemUrl { get; set; }
    
    public float Estoque { get; set; }
    public DateTime DataCadastro { get; set; }
    #region ef, classes anemicas

    //tanto a classe Produto como a classe Categoria não tem comportamento, apenas propriedades. Elas são classes anêmicas.
    //As informacoes q vou gerenciar não estarão armazenadas nas classes, e sim no banco de dados.
    //como vincular as propriedades das classes com as tabelas nos banco de dados, com as informações que estão lá no banco? É aí que entra o Entity Framework para fazermos o mapeamento das entidades.
    #endregion

    public int CategoriaId { get; set; }
    
    [JsonIgnore] //agr essa propriedade vai ser ignorada na serialização e desserialização (qnd for fazer uma requisição só em Produtos)
    public Categoria? Categoria { get; set; } //um produto esta relacionado com uma categoria

    
}