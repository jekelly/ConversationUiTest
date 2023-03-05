using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using System.Windows.Input;
using System.Diagnostics;

namespace ConversationUiTest
{
    public class Conversation
    {
        public string Prompt { get; }
        public IObservable<string> Response { get; }
        public Guid Id { get; }
        public string Title { get; }
        public Conversation(string prompt, string response, bool split)
        {
            this.Id = Guid.NewGuid();
            this.Prompt = prompt;
            if (split)
            {
                this.Response = response
                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToObservable()
                    .Zip(Observable.Interval(TimeSpan.FromMilliseconds(200)), (word, _) => word);
            }
            else
            {
                this.Response = Observable.Return(response);
            }
        }
    }

    public class ConversationService
    {
        private readonly SourceCache<Conversation, Guid> _conversations;
        public IObservableCache<Conversation, Guid> Conversations => _conversations.AsObservableCache();

        public ConversationService()
        {
            _conversations = new SourceCache<Conversation, Guid>(c => c.Id);
        }

        public void AddConversation(string prompt, string response, bool split = false) => _conversations.AddOrUpdate(new Conversation(prompt, response, split));
    }

    internal class WindowViewModel : ReactiveObject
    {
        private readonly ConversationService _cs = new ConversationService();
        private readonly ReadOnlyObservableCollection<ConversationViewModel> _conversations;
        public ReadOnlyObservableCollection<ConversationViewModel> Conversations => _conversations;

        private readonly ReactiveCommand<Unit, Unit> _raiseQueryCommand;
        public ICommand RaiseQueryCommand => _raiseQueryCommand;

        private string _newQuery;
        public string NewQuery
        {
            get => _newQuery;
            set => this.RaiseAndSetIfChanged(ref _newQuery, value);
        }

        public WindowViewModel()
        {
            _cs.AddConversation("Explain quantum computing in simple terms", "Quantum computing is a type of computing that uses quantum mechanics to process information. Traditional computers use bits, which can only be either 0 or 1. Quantum computers, on the other hand, use quantum bits, or qubits, which can be 0, 1, or a superposition of both.\r\n\r\nThis allows quantum computers to perform certain calculations much faster than traditional computers. For example, a quantum computer could factor large numbers exponentially faster than a traditional computer, which is important for cryptography and cybersecurity.\r\n\r\nQuantum computing is still a developing field, and the technology is not yet widely available. However, researchers are actively working on building more powerful quantum computers, and there is a lot of excitement about the potential applications of this technology in fields like drug discovery, financial modeling, and more.");
            _cs.AddConversation("describe how to do grisaille painting", "Grisaille painting is a technique used to create a monochromatic painting in shades of gray, typically used as an underpainting for a later color glaze or as a finished painting style. Here are the basic steps for creating a grisaille painting:\r\n\r\nChoose a reference image or subject and sketch it onto your painting surface using charcoal or graphite. You can also use a transparent projector to project the image onto the surface.\r\n\r\nMix your gray colors using black and white paint. The exact ratio of black to white will depend on the desired shade and value. Start with a mid-tone gray and mix lighter and darker shades as needed.\r\n\r\nApply the gray paint to the canvas in thin, even layers, building up the value and form of the subject. Use a small brush to create detail and texture.\r\n\r\nAllow the grisaille layer to dry completely. If you are using the technique as an underpainting, you can apply a transparent color glaze over the top once the grisaille layer is dry.\r\n\r\nIf you are using the grisaille technique as a finished painting style, you can add additional detail and texture to the gray layer with a small brush or other painting tools. You can also create highlights and shadows using lighter and darker shades of gray.\r\n\r\nOverall, the key to creating a successful grisaille painting is to focus on creating a strong sense of value and form using only shades of gray. This can be a challenging technique, but it can also produce striking and dramatic results.");
            _cs.AddConversation("what are michael bungay stenier's 7 coaching questions", "Michael Bungay Stanier's 7 coaching questions are:\r\n\r\nWhat's on your mind?\r\nAnd what else?\r\nWhat's the real challenge here for you?\r\nWhat do you want?\r\nHow can I help?\r\nIf you're saying yes to this, what are you saying no to?\r\nWhat was most useful or valuable for you in this conversation?\r\nThese questions are designed to help the coach facilitate a productive conversation and empower the coachee to identify and solve their own challenges.", true);

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
                _cs.AddConversation(_newQuery, "this is the response", split: true);
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

