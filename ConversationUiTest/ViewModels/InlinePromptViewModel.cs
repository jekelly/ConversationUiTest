namespace ConversationUiTest.ViewModels
{
    using ReactiveUI;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Windows;

    internal class InlinePromptViewModel : ReactiveObject
    {
        private bool _showPrompt;
        public bool IsInlinePromptVisible
        {
            get => _showPrompt;
            set => this.RaiseAndSetIfChanged(ref _showPrompt, value);
        }

        private readonly ObservableAsPropertyHelper<Visibility> _promptVisibility;
        public Visibility PromptVisibility => _promptVisibility.Value;

        public InlinePromptViewModel()
        {
            var cs = CompositionRoot.ConversationService;

            _promptVisibility = this
                .WhenAnyValue(x => x.IsInlinePromptVisible)
                .Select(b => b ? Visibility.Visible : Visibility.Collapsed)
                .ToProperty(this, vm => vm.PromptVisibility);
        }
    }
}
