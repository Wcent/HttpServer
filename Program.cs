using System;
using System.Net;
using System.Collections.Generic;
using System.Text;

using Common;

namespace HttpServer
{
    class Program
    {
        static HttpListener httpListener;
        static string url = "http://localhost:8080/";

        static Logger logger = Logger.build("Program");

        static void Main(string[] args)
        {
            Run();
        }

        static void Run()
        {
            logger.log(string.Format("启用服务：{0}", url));
            httpListener = new HttpListener();
            httpListener.Prefixes.Add(url);
            httpListener.Start();
            Console.WriteLine("服务已启用，等待请求...按回车确认可结束服务");
            Receive();
            Console.ReadLine();
            httpListener.Close();
            logger.log(string.Format("关闭服务：{0}", url));
        }

        static private void Receive()
        {
            logger.log(string.Format("监听请求..."));
            httpListener.BeginGetContext(HandleRequest, null);
        }

        static private void HandleRequest(IAsyncResult asyncResult) {
            logger.log(string.Format("接收到请求：开始处理"));
            var context = httpListener.EndGetContext(asyncResult);
            var request = context.Request;
            var response = context.Response;
            response.ContentType = "text/plain;charset=UTF-8";
            response.AddHeader("Context-type", "text/plain");
            response.ContentEncoding = Encoding.UTF8;

            try
            {
                string message = parseRequest(request);
                logger.log(string.Format("接收到请求：{0}", message));

                using (var stream = response.OutputStream)
                {
                    var output = Encoding.UTF8.GetBytes(message);
                    stream.Write(output, 0, output.Length);
                }
                response.StatusCode = 200;
            }
            catch(Exception ex)
            {
                response.StatusCode = 500;
                Console.WriteLine("响应异常：{0}", ex.ToString());
                logger.log(string.Format("请求处理异常：{0}", ex.ToString()));
            }
            finally
            {
                Receive();
            }
        }

        private static string parseRequest(HttpListenerRequest request)
        {
            if(request.HttpMethod != "POST" || request.InputStream == null)
            {
                return "非POST请求或请求数据为空";
            }

            var data = new List<byte>();
            int size = 0;
            var buffer = new byte[1024];
            int readLen = 0;
            do
            {
                readLen = request.InputStream.Read(buffer, 0, buffer.Length);
                size += readLen;
                data.AddRange(buffer);
            } while (readLen != 0);

            string text = Encoding.UTF8.GetString(data.ToArray(), 0, size);
            logger.log(text);

            return "接收post数据完成";
        }
    }
}
