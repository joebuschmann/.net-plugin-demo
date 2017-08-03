using System.Configuration;
using System.Net;
using System.Text;
using Host.Contract;

namespace Plugin.HttpServer
{
    public class HttpService : PluginService
    {
        private HttpListener _httpListener;

        public override async void OnStart(string[] args)
        {
            _httpListener = new HttpListener();

            string[] urls = ConfigurationManager.AppSettings["urls"]?.Split(';');

            foreach (var url in urls)
            {
                _httpListener.Prefixes.Add(url);
            }

            _httpListener.Start();

            while (_httpListener.IsListening)
            {
                HttpListenerContext context = await _httpListener.GetContextAsync();

                string responseBody = "<html><body>Hello from Http Server.</body></html>";
                byte[] responseBytes = Encoding.UTF8.GetBytes(responseBody);

                var response = context.Response;
                response.ContentLength64 = responseBytes.Length;
                response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
                response.OutputStream.Close();
                }
        }

        public override void OnStop()
        {
            _httpListener.Stop();
        }
    }
}
