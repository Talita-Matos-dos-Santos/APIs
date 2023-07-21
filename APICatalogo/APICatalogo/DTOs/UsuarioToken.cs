namespace APICatalogo.DTOs;

public class UsuarioToken
{
    public bool Authenticated { get; set; } //se ta autenticado
    public DateTime Expiration { get; set; } //a data de expiracao
    public string Token { get; set; } //o token em si
    public string Message { get; set; } //uma mensagem, q pode ser uma mensagem de erro, mensagem de que o token foi gerado com sucesso
}