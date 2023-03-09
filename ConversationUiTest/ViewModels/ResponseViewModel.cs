using System;
using System.Reactive.Linq;
using ReactiveUI;

namespace ConversationUiTest
{
    public class ResponseViewModel : ReactiveObject
    {
        public string Icon { get; set; } = "🤖";

        private string _response;
        public string Response
        {
            get => _response;
            set => this.RaiseAndSetIfChanged(ref _response, value);
        }

        public ResponseViewModel(Exchange c)
        {
            c.Response.Subscribe(x =>
            {
                this.Response += x + ' ';
            });
        }
    }
}
