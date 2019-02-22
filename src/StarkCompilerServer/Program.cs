using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Server;
using ILanguageServer = OmniSharp.Extensions.LanguageServer.Protocol.Server.ILanguageServer;

namespace StarkCompilerServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //System.Diagnostics.Debugger.Launch();
            //while (!System.Diagnostics.Debugger.IsAttached)
            //{
            //    await Task.Delay(100);
            //}

            var options = new LanguageServerOptions()
                .WithInput(Console.OpenStandardInput())
                .WithOutput(Console.OpenStandardOutput())
                .WithLoggerFactory(new LoggerFactory())
                .AddDefaultLoggingProvider()
                .WithMinimumLogLevel(LogLevel.Trace)
                .WithHandler<TextDocumentHandler>();

            options.OnInitialize(Delegate);

            var server = await LanguageServer.From(options);
            await server.WaitForExit;
        }

        private static Task Delegate(OmniSharp.Extensions.LanguageServer.Server.ILanguageServer server, InitializeParams request)
        {
            server.Window.Log("Initialize received: " + request.RootPath);
            return Unit.Task;
        }
    }

    public class ServerInit : IInitializeHandler
    {
        private ILanguageServer _router;

        public ServerInit(OmniSharp.Extensions.LanguageServer.Protocol.Server.ILanguageServer router)
        {
            _router = router;
        }


        public Task<InitializeResult> Handle(InitializeParams request, CancellationToken cancellationToken)
        {
            _router.Window.Log("Initialize received: " + request.RootPath);

            return Task.FromResult(new InitializeResult()
            {
                Capabilities = new ServerCapabilities()
                {
                    TextDocumentSync =
                    {
                        Kind = TextDocumentSyncKind.Incremental,
                    }
                }
            });
        }
    }
}
