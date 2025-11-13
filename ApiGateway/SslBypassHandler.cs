using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace ApiGateway
{
    public class SslBypassHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Ignore SSL certificate validation for localhost
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;

            return base.SendAsync(request, cancellationToken);
        }
    }
}
