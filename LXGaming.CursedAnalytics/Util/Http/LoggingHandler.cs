using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace LXGaming.CursedAnalytics.Util.Http {

    public class LoggingHandler : DelegatingHandler {

        public LoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler) {
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            Log.Debug("Connecting to {Uri}", request.RequestUri);
            return base.SendAsync(request, cancellationToken);
        }
    }
}