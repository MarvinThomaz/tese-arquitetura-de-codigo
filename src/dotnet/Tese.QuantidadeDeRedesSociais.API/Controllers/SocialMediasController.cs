using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Tese.QuantidadeDeRedesSociais.Api.Controllers
{
    internal class GitHubResponse
    {

    }

    internal class SocialMediaResponse
    {
        public bool RegisterExists { get; internal set; }
    }

    internal class ErrorResponse
    {
        public string Mensagem { get; }

        public ErrorResponse(string mensagem)
        {
            Mensagem = mensagem;
        }
    }

    [ApiController]
    [Route("usuarios/{login}/redes-sociais")]
    public class SocialMediasController : ControllerBase
    {
        public async Task<IActionResult> GetSocialMediasAsync(string login)
        {
            var urlGitHub = @"https://docs.github.com/en/rest/reference/users#get-a-user";
            var httpClient = new HttpClient();

            using (httpClient)
            {
                var gitHubResponse = await httpClient.GetAsync(urlGitHub);

                if (gitHubResponse.IsSuccessStatusCode)
                {
                    try
                    {
                        var gitHubJsonResponse = await gitHubResponse.Content.ReadAsStringAsync();
                        var gitHubResponseObj = JsonSerializer.Deserialize<GitHubResponse>(gitHubJsonResponse);
                        var socialMediaResponse = new SocialMediaResponse
                        {
                            RegisterExists = true
                        };

                        return Ok(gitHubResponse);
                    }
                    catch (Exception ex)
                    {
                        var loggerFactoryType = typeof(ILoggerFactory);
                        var loggerFactory = HttpContext.RequestServices.GetService(loggerFactoryType) as ILoggerFactory;
                        var logger = new Logger<SocialMediasController>(loggerFactory);

                        logger.LogError(ex, ex.Message);

                        var value = new ErrorResponse("Ops! Ocorreu um erro, entre em contato com nossos administradores pela central (xx) xxxx-xxxx.");

                        return base.StatusCode(StatusCodes.Status500InternalServerError, value);
                    }
                }
                else if (gitHubResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    var socialMediaResponse = new SocialMediaResponse
                    {
                        RegisterExists = true
                    };

                    return NotFound(socialMediaResponse);
                }
                else
                {
                    var value = new ErrorResponse("Ops! Ocorreu um erro, entre em contato com nossos administradores pela central (xx) xxxx-xxxx.");

                    return base.StatusCode(StatusCodes.Status500InternalServerError, value);
                }
            }
        }
    }
}
