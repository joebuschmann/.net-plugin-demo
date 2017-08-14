﻿using System.Configuration;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Host.Contract;

namespace Plugin.HttpServer
{
    public class HttpService : PluginService
    {
        private HttpListener _httpListener;
        private int _hitCount = 0;

        public override void OnStart(string[] args)
        {
            StartHttpListener();
            WaitForRequests().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    throw t.Exception.InnerException;
                }
            });
        }

        private void StartHttpListener()
        {
            _httpListener = new HttpListener();

            string[] urls = ConfigurationManager.AppSettings["urls"]?.Split(';');

            foreach (var url in urls)
            {
                _httpListener.Prefixes.Add(url);
            }

            _httpListener.Start();
        }

        private async Task WaitForRequests()
        {
            while (_httpListener.IsListening)
            {
                HttpListenerContext context = await _httpListener.GetContextAsync();
                
                _hitCount++;
                string responseBody = $"<html><body>Hello from Http Server. You are visitor <em>#{_hitCount}</em>.</body></html>";
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
