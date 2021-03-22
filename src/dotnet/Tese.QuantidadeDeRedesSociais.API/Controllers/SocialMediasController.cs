using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Tese.QuantidadeDeRedesSociais.Api.Controllers
{
    internal class GitHubResponse
    {
        public string Login { get; set; }
    }

    internal class SocialMediaResponse
    {
        public bool RegisterExists { get; set; }
    }

    internal class ErrorResponse
    {
        public string Message { get; set; }
    }

    [ApiController]
    [Route("usuarios/{login}/redes-sociais")]
    public class SocialMediasController : ControllerBase
    {
        public async Task<IActionResult> GetSocialMediasAsync(string login)
        {
            var urlGitHub = $"https://api.github.com/api/v3/user";
            var httpClient = new HttpClient();

            using (httpClient)
            {
                var gitHubRequest = new HttpRequestMessage { RequestUri = new Uri(urlGitHub) };
                var gitHubBasicCredentials = $"{Environment.GetEnvironmentVariable("GITHUB_USERNAME")}:{Environment.GetEnvironmentVariable("GITHUB_PERSONAL_ACCESS_TOKEN")}";
                var mediaTypeWithQualityHeaderValue = new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json");

                gitHubRequest.Headers.Accept.Add(mediaTypeWithQualityHeaderValue);
                gitHubRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", gitHubBasicCredentials);

                var gitHubResponse = await httpClient.SendAsync(gitHubRequest);

                if (gitHubResponse.IsSuccessStatusCode)
                {
                    var gitHubJsonResponse = await gitHubResponse.Content.ReadAsStringAsync();

                    try
                    {
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

                        return UnexpectedError();
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
                    return UnexpectedError();
                }
            }
        }

        private IActionResult UnexpectedError()
        {
            var value = new ErrorResponse { Message = "Ops! Ocorreu um erro, entre em contato com nossos administradores pela central (xx) xxxx-xxxx." };

            return base.StatusCode(StatusCodes.Status500InternalServerError, value);
        }
    }
}
