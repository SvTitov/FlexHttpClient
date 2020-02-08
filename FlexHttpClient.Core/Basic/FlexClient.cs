using System;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
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

        public async Task<string> Get(string path, string query = null)
        {
            try
            {
                _tcpClient.ConnectAsync(_host, _port).Wait(_timeout);

                var stream = _tcpClient.GetStream();

                var request = GetRequest(path);

                await WriteToStream(stream, request);

                var builder = await ReadData(_tcpClient, stream);

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

        private byte[] GetRequest(string path)
        {
            var request = Encoding.UTF8.GetBytes(PrepareRequest(path));
            return request;
        }

        private async Task<StringBuilder> ReadData(TcpClient client, NetworkStream stream)
        {
            byte[] buffer = new byte[client.ReceiveBufferSize];
            StringBuilder builder = new StringBuilder();

            WaitForConnection(stream).Wait(_timeout);

            while (stream.DataAvailable)
            {
                var readBytes = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (readBytes <= 0)
                    break;

                builder.Append(Encoding.UTF8.GetString(buffer, 0, buffer.Length));
            }

            return builder;
        }

        private async Task WriteToStream(NetworkStream stream, byte[] request)
        {
            ReadOnlyMemory<byte> readOnlyMemory = new Memory<byte>(request);

            await stream.WriteAsync(readOnlyMemory);
        }

        private string PrepareRequest(string path)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"GET /{path} HTTP/1.1");
            stringBuilder.AppendLine($"HOST: {RemoveDomain(_host)}");
            stringBuilder.AppendLine($"User-Agent: flexhttp/0.1");
            stringBuilder.AppendLine("Accept: */*");
            stringBuilder.AppendLine();

            return stringBuilder.ToString();
        }

        private string RemoveDomain(string host)
        {
            return Regex.Replace(host, @"^(?:http(?:s)?://)?(?:www(?:[0-9]+)?\.)?", string.Empty, RegexOptions.IgnoreCase);
        }
    }
}
