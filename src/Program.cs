using CommandLine;
using MihaZupan;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace torping
{
    internal class Program
    {
        class Options
        {
            [Option('i', "socks5-ip", Required = true, HelpText = "Tor proxy IP address.")]
            public string socks5ip { get; set; }

            [Option('p', "socks5-port", Required = true, HelpText = "Tor proxy Port.")]
            public int socks5port { get; set; }

            [Option('u', "url", Required = true, HelpText = "Site URL.")]
            public string url { get; set; }

            [Option('t', "timeout", Required = false, Default = 30, HelpText = "Connection timeout (sec).")]
            public int timeout { get; set; }

            [Option('n', "interval", Required = false, Default = 15, HelpText = "Ping interval (sec).")]
            public int interval { get; set; }

            [Option('r', "retry", Required = false, Default = 5, HelpText = "On error retry interval (sec).")]
            public int retry { get; set; }

            [Option('a', "user-agent", Required = false, Default = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36", HelpText = "User agent.")]
            public string ua { get; set; }

            //// Omitting long name, defaults to name of property, ie "--verbose"
            //[Option(
            //  Default = false,
            //  HelpText = "Prints all messages to standard output.")]
            //public bool Verbose { get; set; }

            //[Value(0, MetaName = "offset", HelpText = "File offset.")]
            //public long? Offset { get; set; }
        }
        static void RunOptions(Options opts)
        {
            //handle options

            // required
            Config.ip = opts.socks5ip;
            Config.port = opts.socks5port;
            Config.url = opts.url;
            // optional
            Config.timeout = opts.timeout;
            Config.interval = opts.interval;
            Config.retry = opts.retry;
            Config.ua = opts.ua;

            Console.WriteLine($"Connection timeout={Config.timeout}s, Ping interval={Config.interval}s, Retry interval={Config.retry}s, User agent={Config.ua}");
            Console.WriteLine($"{Config.ip}:{Config.port} -> {Config.url}");
        }
        static void HandleParseError(IEnumerable<Error> errs)
        {
            //handle errors
            Console.ReadKey(true);
            Environment.Exit(1);
        }

        static class Config
        {
            // required
            public static string ip { get; set; }
            public static int port { get; set; }
            public static string url { get; set; }
            // optional
            public static int timeout { get; set; }
            public static int interval { get; set; }
            public static int retry { get; set; }
            public static string ua { get; set; }
        }

        // ===============================================================
        public static bool success { get; set; }

        static async Task Main(string[] args)
        {
            success = false;

            ParserResult<Options> opt = Parser.Default.ParseArguments<Options>(args)
              .WithParsed(RunOptions)
              .WithNotParsed(HandleParseError);

            while (true)
            {
                string msg;
                int inter;

                Console.Write("Request...");
                await DoMain();
                if (success)
                {
                    msg = "Interval...";
                    inter = Config.interval;
                }
                else
                {
                    msg = "Retry...";
                    inter = Config.retry;
                }
                Console.Write(msg);
                await Task.Delay(inter * 1000);
            }
            //DoMain().GetAwaiter().GetResult();
        }

        // ===============================================================
        static async Task DoMain()
        {
            success = false;

            var proxy = new HttpToSocks5Proxy(Config.ip, Config.port);
            proxy.ResolveHostnamesLocally = false;

            HttpClientHandler handler = null;
            HttpClient client = null;
            HttpResponseMessage result = null;

            var stopwatch = new Stopwatch();
            stopwatch.Reset();

            try
            {
                handler = new HttpClientHandler { Proxy = proxy };
                handler.UseProxy = true;
                handler.AllowAutoRedirect = false;

                client = new HttpClient(handler, true);
                client.Timeout = TimeSpan.FromSeconds(Config.timeout);

                // https://httpbin.org/ip
                // http://eixoaclv7qvnmu5rolbdwba65xpdiditdoyp6edsre3fitad777jr3ad.onion/
                // Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36
                var request = new HttpRequestMessage(HttpMethod.Get, Config.url);
                request.Headers.UserAgent.ParseAdd(Config.ua);
                stopwatch.Start();
                result = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
                stopwatch.Stop();
                success = result.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                CR();
                Console.WriteLine($"{DateTime.Now} | {"ERROR  "} | {ex.Message} (Probably timeout)");
                return;
            }
            finally
            {
                if (client != null) client.Dispose();
                if (handler != null) handler.Dispose();
            }

            if (result != null)
            {
                string content = await result.Content.ReadAsStringAsync();
                CR();
                Console.WriteLine($"{DateTime.Now} | {(result.IsSuccessStatusCode ? "SUCCESS" : "ERROR  ")} | {(int)result.StatusCode} {result.ReasonPhrase} | Length: {content.Length} | Time: {stopwatch.Elapsed.Milliseconds}");
            }
            else
            {
                CR();
                Console.WriteLine($"{DateTime.Now} | {"ERROR  "} | Unknown");
            }
        }

        static void CR()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
        }
    }
}
