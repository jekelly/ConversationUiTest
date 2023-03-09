namespace ConversationUiTest
{
    using ConversationUiTest.Models;
    using DynamicData;
    using DynamicData.Binding;
    using ReactiveUI;
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reflection;

    internal class ConversationViewModel : ReactiveObject
    {
        private readonly ReadOnlyObservableCollection<ExchangeViewModel> _exchanges;

        public ReadOnlyObservableCollection<ExchangeViewModel> Exchanges => _exchanges;

        // TODO this is awkward here - what happens if you Ask on a conversation that is complete?
        private ObservableAsPropertyHelper<bool> _isComplete;
        public bool IsComplete => _isComplete.Value;

        public ConversationViewModel(Conversation conversation)
        {
            conversation.Exchanges
                .Connect()
                .Transform(e =>new ExchangeViewModel(e))
                .Bind(out _exchanges)
                .Subscribe();


            _isComplete = _exchanges
                .Select(x => x.WhenAnyValue(vm => vm.IsComplete))
                .CombineLatest(x => x.All(x => x))
                .DistinctUntilChanged()
                .Do(x => Debug.WriteLine($"_isComplete: {x}"))
                .ToProperty(this, vm => vm.IsComplete);
        }
    }
}