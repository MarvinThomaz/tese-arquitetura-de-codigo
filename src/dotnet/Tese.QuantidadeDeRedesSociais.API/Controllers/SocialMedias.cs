using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Tese.QuantidadeDeRedesSociais.API.Controllers
{
    [ApiController]
    [Route("[usuarios/{login}]/redes-sociais")]
    public class WeatherForecastController : ControllerBase
    {
        public async Task<IActionResult> GetSocialMediasAsync(string login)
        {
            return Ok();
        }
    }
}
