using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.LanguageServer;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Server;
using ILanguageServer = OmniSharp.Extensions.LanguageServer.Server.ILanguageServer;

namespace StarkCompilerServer
{
    // Check for https://github.com/OmniSharp/omnisharp-roslyn

    class TextDocumentHandler : ITextDocumentSyncHandler
    {
        private readonly OmniSharp.Extensions.LanguageServer.Protocol.Server.ILanguageServer _router;

        private readonly DocumentSelector _documentSelector = new DocumentSelector(
            new DocumentFilter()
            {
                Pattern = "**/*.sk"
            }
        );

        private SynchronizationCapability _capability;

        public TextDocumentHandler(OmniSharp.Extensions.LanguageServer.Protocol.Server.ILanguageServer router)
        {
            _router = router;
        }

        public TextDocumentSyncKind Change { get; } = TextDocumentSyncKind.Incremental;

        public Task<Unit> Handle(DidChangeTextDocumentParams notification, CancellationToken token)
        {
            _router.Window.Log($"Server: DidChangeText:");

            var contentChanges = notification.ContentChanges.ToArray();
            if (contentChanges.Length == 1 && contentChanges[0].Range == null)
            {
                _router.Window.Log($"Change buffer with no range: {contentChanges[0].Text}");

                //var change = contentChanges[0];
                //await _bufferHandler.Handle(new UpdateBufferRequest()
                //{
                //    FileName = Helpers.FromUri(notification.TextDocument.Uri),
                //    Buffer = change.Text
                //});
                return Unit.Task;
            }

            foreach (var change in notification.ContentChanges)
            {
                //_router.Window.Log($"Change: {change.Text} Start: {change.Range.Start.Character} Line:{change.Range.Start.Line} {change.RangeLength}");
                if (change.Range != null)
                {
                    _router.Window.Log($"Change: {change.Text} Start: {change.Range.Start.Character} Line:{change.Range.Start.Line} Length: {change.RangeLength}");
                }
                else
                {
                    _router.Window.Log($"Change: {change.Text} Length: {change.RangeLength}");
                }
            }

            return Unit.Task;
        }

        TextDocumentChangeRegistrationOptions IRegistration<TextDocumentChangeRegistrationOptions>.GetRegistrationOptions()
        {
            return new TextDocumentChangeRegistrationOptions()
            {
                DocumentSelector = _documentSelector,
                SyncKind = Change
            };
        }

        public void SetCapability(SynchronizationCapability capability)
        {
            _capability = capability;
        }

        public async Task<Unit> Handle(DidOpenTextDocumentParams notification, CancellationToken token)
        {
            await Task.Yield();

            _router.Window.Log("yooyoyoyoyooyo");

            _router.Window.LogMessage(new LogMessageParams()
            {
                Type = MessageType.Log,
                Message = "Hello World!!!!"
            });
            return Unit.Value;
        }

        TextDocumentRegistrationOptions IRegistration<TextDocumentRegistrationOptions>.GetRegistrationOptions()
        {
            return new TextDocumentRegistrationOptions()
            {
                DocumentSelector = _documentSelector,
            };
        }

        public Task<Unit> Handle(DidCloseTextDocumentParams notification, CancellationToken token)
        {
            return Unit.Task;
        }

        public Task<Unit> Handle(DidSaveTextDocumentParams notification, CancellationToken token)
        {
            return Unit.Task;
        }

        TextDocumentSaveRegistrationOptions IRegistration<TextDocumentSaveRegistrationOptions>.GetRegistrationOptions()
        {
            return new TextDocumentSaveRegistrationOptions()
            {
                DocumentSelector = _documentSelector,
                IncludeText = true
            };
        }
        public TextDocumentAttributes GetTextDocumentAttributes(Uri uri)
        {
            return new TextDocumentAttributes(uri, "stark");
        }
    }
}