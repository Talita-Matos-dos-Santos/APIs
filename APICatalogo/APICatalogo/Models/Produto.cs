namespace APICatalogo.Models;

public class Produto
{
    public int ProdutoId { get; set; } //chave primária
    public string? Nome { get; set; } //string é tipo por referencia, por isso precisa do ? q deve ser nullable
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public string? ImagemUrl { get; set; }
    public float Estoque { get; set; }
    public DateTime DataCadastro { get; set; }
    #region ef, classes anemicas

    //tanto a classe Produto como a classe Categoria não tem comportamento, apenas propriedades. Elas são classes anêmicas.
    //As informacoes q vou gerenciar não estarão armazenadas nas classes, e sim no banco de dados.
    //como vincular as propriedades das classes com as tabelas nos banco de dados, com as informações que estão lá no banco? É aí que entra o Entity Framework para fazermos o mapeamento das entidades.
    #endregion

    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; } //um produto esta relacionado com uma categoria

    
}