using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Windows.Input;

namespace ConversationUiTest
{
    public class ExchangeViewModel : ReactiveObject
    {
        public ExchangeViewModel(Exchange conversation)
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
}
