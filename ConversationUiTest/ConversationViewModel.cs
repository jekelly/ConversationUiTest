using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using ReactiveUI;

namespace ConversationUiTest
{
    public class ConversationViewModel : ReactiveObject
    {
        public ConversationViewModel(Conversation conversation)
        {
            this.Prompt = new PromptViewModel(this) { Prompt = conversation.Prompt };
            this.Response = new ResponseViewModel(conversation);
            var complete = Observable.Create<bool>(x =>
            {
                conversation.Response.Subscribe(s => { }, () => { x.OnNext(true); x.OnCompleted(); });
                return () => { };
            });
            _isComplete = complete.ToProperty(this, o => o.IsComplete, scheduler: RxApp.MainThreadScheduler);

            this.EditCommand = ReactiveCommand.Create(() =>
            {
                this.IsEditing = true;
            });

            this.CancelEditCommand = ReactiveCommand.Create(() =>
            {
                this.IsEditing = false;
            });
        }

        public PromptViewModel Prompt { get; set; }
        public ResponseViewModel Response { get; set; }
        private ObservableAsPropertyHelper<bool> _isComplete;
        public bool IsComplete => _isComplete.Value;

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set => this.RaiseAndSetIfChanged(ref _isEditing, value);
        }

        public ICommand EditCommand { get; }
        public ICommand CancelEditCommand { get; }
    }


    public class PromptViewModel : ReactiveObject
    {
        private readonly ConversationViewModel _conversation;

        public PromptViewModel(ConversationViewModel cm)
        {
            _conversation = cm;
            _isEditing = cm
                .WhenAnyValue(c => c.IsEditing)
                .ToProperty(this, c => c.IsEditing);
        }

        public ICommand EditCommand => _conversation.EditCommand;
        private ObservableAsPropertyHelper<bool> _isEditing;
        public bool IsEditing => _isEditing.Value;
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
