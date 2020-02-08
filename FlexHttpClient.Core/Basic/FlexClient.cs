﻿using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FlexHttpClient.Core.Basic
{
    public class FlexClient
    {
        private readonly string _host;
        private readonly int _port;
        private readonly int _timeout;
        private TcpClient _tcpClient;

        public FlexClient(string host, int port = 80, int timeout = 5000)
        {
            _tcpClient = new TcpClient();
            _tcpClient.ReceiveTimeout = 2000;
            _tcpClient.SendTimeout = 5000;

            this._host = host;
            this._port = port;
            this._timeout = timeout;
        }

        public async Task<string> Get()
        {
            try
            {
                _tcpClient.ConnectAsync(_host, _port).Wait(_timeout);
                var stream = _tcpClient.GetStream();

                var rawRequest = PrepareRequest();

                var request = Encoding.UTF8.GetBytes(rawRequest);

                ReadOnlyMemory<byte> readOnlyMemory = new Memory<byte>(request);

                await stream.WriteAsync(readOnlyMemory);

                byte[] buffer = new byte[256];
                StringBuilder builder = new StringBuilder();

                WaitForConnection(stream).Wait(_timeout);

                while (stream.DataAvailable)
                {
                    var data  = await stream.ReadAsync(buffer);
                    builder.Append(Encoding.UTF8.GetString(buffer,0,buffer.Length));
                }

                return builder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        async Task WaitForConnection(NetworkStream stream)
        {
            while(!stream.DataAvailable)
            {
                await Task.Delay(100);
            }
        }

        private string PrepareRequest()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("GET /index.html HTTP/1.1");
            stringBuilder.AppendLine();

            return stringBuilder.ToString();
        }
    }
}
