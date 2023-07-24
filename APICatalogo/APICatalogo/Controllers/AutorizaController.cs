using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using APICatalogo.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace APICatalogo.Controllers
{
    [Produces("application/json")] //define que o formato do retorno será esse la no swagger. Antes de colocar ficavam outras opcoes tbm, como text/plain e text/json
    [Route("api/[controller]")]
    [ApiController]
    public class AutorizaController : ControllerBase
    {
        //identityuser tem as informacoes do usuario como email, nome do usuario...
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration; //aq eu vou precisar de uma instancia de iconfiguration pra poder ler as informacoes do arquivo appsettings.json

        public AutorizaController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            //aq eu vou injetar instancias de usermanager e signinmanager. Isso é necessario pra que eu possa usar essas instancias e definir as acoes de registrar usuario e as acoes de login
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration; //agr ja posso ler as informacoes do appsettings.json
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            return "AutorizaController :: Acesso em : " + DateTime.Now.ToLongDateString();
            //serve so pra verificar se essa api ta atendendo. Manda a data q foi acessada tbm
        }
        
        ///<summary>
        /// Registra um novo usuário
        /// </summary>
        ///<param name="model">Um objeto UsuarioDTO</param>
        /// <returns>Status 200 e o token para o cliente</returns>
        /// 
        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser([FromBody] UsuarioDTO model)
        {
            var user = new IdentityUser()
            {
                //aq eu to criando uma instancia do meu usuario do identity, passando algumas informacoes q foram informadas no corpo da requisicao
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password); //note que aq na criaçao do usuário ta sendo passado as informacoes do user, q tem os dados do new IdentityUser, e este tem username, email... aq tbm é passado a senha, que tbm sera adquirida pelo body do request e passado pro usuarioDTO.

            if (!result.Succeeded) //se o usuario n foi criado c sucesso
            {
                return BadRequest(result.Errors);
            }

            await _signInManager.SignInAsync(user, false);
            #region SignInManager
             //a classe SignInManager é responsável por gerenciar o processo de autenticação (login) de usuários. Ela trabalha em conjunto com a classe UserManager para fornecer uma camada de abstração para a autenticação, permitindo que você lide com o processo de login de forma mais simples e segura.
            //Autenticação do usuário: O SignInManager fornece métodos para autenticar um usuário com base nas credenciais fornecidas, como nome de usuário e senha. Ele valida essas credenciais em relação ao sistema de armazenamento usado pelo Identity (normalmente um banco de dados) e, se as credenciais forem válidas, cria um cookie de autenticação para o usuário, permitindo que ele permaneça autenticado em solicitações subsequentes.
            //como aqui usamos na persistencia o "false", então:
            //ele autentica o usuário no aplicativo com um cookie de autenticação não persistente. Isso significa que o usuário será autenticado apenas para a sessão atual do navegador e o cookie de autenticação não será mantido após o fechamento do navegador.
            //false (não persistente): O cookie de autenticação é armazenado apenas para a sessão atual do navegador. Se o usuário fechar o navegador ou navegar para outra página após o login, o cookie será descartado e o usuário será solicitado a fazer login novamente quando retornar ao aplicativo.
            #endregion
            return Ok(GeraToken(model)); //se o usuario for registrado com sucesso eu vou chamar o geratoken, passando as informacoes do usuariodto pra ele
            
            //ou seja, o metodo registra/cria o usuario e retorna um 200 ok
        }

        ///<summary>
        /// Verifica as credenciais de um usuário
        /// </summary>
        ///<param name="userInfo">Um objeto do tipo UsuarioDTO</param>
        /// <returns>Status 200 e o token para o cliente</returns>
        /// <remarks>retorna o Status 200 e o token para o cliente</remarks>
        /// 
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UsuarioDTO userInfo)
        {
            //verifica se o modelo é válido
            if (!ModelState.IsValid)
            {
                //na vdd isso aqui não é necessario pois o [ApiController] no começo dessa api ja faz isso automaticamente.
                return BadRequest(ModelState.Values.SelectMany(e => e.Errors));
            }
            
            
            //verifica as credenciais do usuário e retorna um valor
            var result = await _signInManager.PasswordSignInAsync(userInfo.Email, userInfo.Password,
                isPersistent: false, lockoutOnFailure: false);
            // Aqui, o objeto _signInManager é usado para verificar as credenciais do usuário. O método PasswordSignInAsync realiza a autenticação do usuário com base no email e senha fornecidos no objeto userInfo.
            //lockoutOnFailure: false: Esse parâmetro indica que o bloqueio da conta de usuário não será acionado em caso de falha na autenticação.

            if (result.Succeeded)
            {
                return Ok(GeraToken(userInfo));
                // Se a autenticação for bem-sucedida (as credenciais forem válidas), o método retorna um resultado HTTP 200 OK. Isso significa que o login foi realizado com sucesso.
                //se o login for feito com sucesso eu vou chamar o geratoken aq e passar as informacoes do usuario pro metodo geratoken
            }
            else
            {
                //Caso as credenciais fornecidas não sejam válidas e a autenticação falhe, o fluxo do código passa para o bloco else.
                ModelState.AddModelError(string.Empty, "Login Inválido...");
                return BadRequest(ModelState);
            }
        }

        private UsuarioToken GeraToken(UsuarioDTO userInfo)
        {
            //vou passar pra esse metodo as informacoes do usuario. Essas informacoes so podem ser passadas dps que o usuario fizer o registro e o login
            
            
            //define declarações do usuário
            var claims = new[]
            {
                //isso aqui nao é obrigatorio, foi feito pra tornar o token mais seguro. é usado pra gerar o token tbm
                new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.Email),
                new Claim("meuPet", "pipoca"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            
            //gera uma chave privada com base em um algoritmo simétrico HMAC
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:key"])); //aq ta lendo aquela informacao da chave secreta que ta la no appsettings.json. Ou seja, pegou aquela chave la e gerou uma chave privada aqui
            
            //gera a assinatura digital do token usando o algoritmo Hmac e a chave privada
            var credenciais = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); //usando esse algoritmo hmacsha256 ele vai gerar uma assinatura digital, que tem como base essa chave privada
            
            //tempo de expiração do token
            var expiracao = _configuration["TokenConfiguration:ExpireHours"]; //aq ta salvando na variavel o tempo de expiracao com base no que ta la no appsettings.json, no tokenconfiguration
            var expiration = DateTime.UtcNow.AddHours(double.Parse(expiracao)); //aqui ta convertendo o valor de expiracao pra double e gerando uma data no formato Utc.
            
            //classe que representa um token JWT e gera o token
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration["TokenConfiguration:Issuer"],
                audience: _configuration["TokenConfiguration:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credenciais
            ); //as credenciais é a assinatura digital 
            //ate aqui ja foi gerado o token
            
            //agr vou retornar os dados do token e informacoes
            return new UsuarioToken()
            {
                Authenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token), //aq serializa o token e informa ele
                Expiration = expiration,
                Message = "Token JWT OK"
            };
        }
        
        
    }
}
