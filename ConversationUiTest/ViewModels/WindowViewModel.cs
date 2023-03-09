using ConversationUiTest.Models;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;

namespace ConversationUiTest
{
    internal class WindowViewModel : ReactiveObject
    {
        private readonly ReadOnlyObservableCollection<ConversationViewModel> _conversations;
        public ReadOnlyObservableCollection<ConversationViewModel> Conversations => _conversations;

        private readonly ReactiveCommand<Unit, Unit> _raiseQueryCommand;
        public ICommand RaiseQueryCommand => _raiseQueryCommand;

        private string _newQuery;
        public string NewQuery {
            get => _newQuery;
            set => this.RaiseAndSetIfChanged(ref _newQuery, value);
        }

        public ObservableCollection<QuickActionViewModel> QuickActions { get; }

        public WindowViewModel()
        {
            var _cs = CompositionRoot.ConversationService;
            var viewModelCache = _cs.Conversations
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Transform(x => new ConversationViewModel(x))
                .AsObservableCache();

            viewModelCache
                .Connect()
                .Bind(out _conversations)
                .Subscribe();

            var allDone = viewModelCache.Connect().MergeMany(vm => vm.WhenAnyValue(v => v.IsComplete));

            this.QuickActions = new ObservableCollection<QuickActionViewModel>();
            this.QuickActions.Add(new QuickActionViewModel("fix", "🛠", this, () => MessageBox.Show("fix me"), s => !s.Contains("foo")));
            this.QuickActions.Add(new QuickActionViewModel("repair", "🤕", this, () => MessageBox.Show("repair me"), s => !s.Contains("bar")));
            this.QuickActions.Add(new QuickActionViewModel("explain", "👩‍🏫", this,() => MessageBox.Show("explain me"), s => !s.Contains("baz")));


            allDone.Subscribe(isComplete =>
            {
                Debug.WriteLine($"Thinks something {isComplete}");
            });


            //.Transform(conversation =>
            //{
            //    return Observable.Create<bool>(o =>
            //    {
            //        o.OnNext(false);
            //        conversation.Response.Subscribe(s => { }, () => { o.OnNext(true); o.OnCompleted(); });
            //        return () => { };
            //    });
            //})


            _raiseQueryCommand = ReactiveCommand.Create(() =>
            {
                Conversation c = new Conversation("hello");
                _cs.AddConversation(c);
                c.Ask(_newQuery, "this is my response",  split: true);
                this.NewQuery = string.Empty;
            }, allDone);
        }
    }

    public static class SampleExtentions
    {
        public static void Dump<T>(this IObservable<T> source, string name)
        {
            source.Subscribe(
            i => Debug.WriteLine("{0}-->{1}", name, i),
            ex => Debug.WriteLine("{0} failed-->{1}", name, ex.Message),
            () => Debug.WriteLine("{0} completed", name));
        }
    }
}

