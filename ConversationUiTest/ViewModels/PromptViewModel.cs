using System.Windows.Input;
using ReactiveUI;

namespace ConversationUiTest
{
    public class PromptViewModel : ReactiveObject
    {
        private readonly ExchangeViewModel _conversation;

        public PromptViewModel(ExchangeViewModel cm)
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
}
