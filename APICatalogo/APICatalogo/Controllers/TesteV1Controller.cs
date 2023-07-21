using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("2.0")] //supondo que ao inves de criar uma nova versao de uma api (ou seja, criar uma outra api) eu queira apenas versionar um método action. Colocando esse atributo aqui eu vou indicar que eu tenho um controlador que tá atendendo a duas versões. Significa que vou ter metodos actions que vai atender a versao 1 e outros que vai atender a versao 2 
    [Route("api/v{v:apiVersion}/teste")] //agr a rota pra chamar a api ficou: api/vnrodaversao/teste -> https://localhost:7073/api/v1/teste
    [ApiController]
    public class TesteV1Controller : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Content("<html><body><h2>TesteV1Controller - V 1.0 </h2></body></html>", "text/html");
        }
        
        [HttpGet, MapToApiVersion("2.0")] //esse atributo faz com que esse metodo get acione a versao 2 da api. Estamos fazendo um versionamento a nível de metodo Action.
        //Embora existe, nao é muito recomendável usar esse método, pois caso tenha muitos em um controlador pode acabar gerando problemas de manutencao etc etc.
        public IActionResult GetVersao2()
        {
            return Content("<html><body><h2>TesteV1Controller - V 2.0 </h2></body></html>", "text/html");
        }
    }
}
