using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace ConversationUiTest
{
    public class ConversationViewModel : ReactiveObject
    {
        public ConversationViewModel(Conversation conversation)
        {
            this.Prompt = new PromptViewModel() { Prompt = conversation.Prompt };
            this.Response = new ResponseViewModel(conversation);
        }

        public PromptViewModel Prompt { get; set; }
        public ResponseViewModel Response { get; set; }
    }

    public class PromptViewModel : ReactiveObject
    {
        public string Icon { get; set; } = "👨";
        public string Prompt { get; set; }
    }

    public class ResponseViewModel : ReactiveObject
    {
        public string Icon { get; set; } = "🤖";

        private string _response;
        public string Response
        {
            get => _response;
            set => this.RaiseAndSetIfChanged(ref _response, value);
        }

        public ResponseViewModel(Conversation c)
        {
            c.Response.Subscribe(x =>
            {
                this.Response += x + ' ';
            });
        }
    }
}
