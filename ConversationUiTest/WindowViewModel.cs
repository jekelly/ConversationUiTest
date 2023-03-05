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

namespace ConversationUiTest
{
    public class Conversation
    {
        public string Prompt { get; }
        public IObservable<string> Response { get; }
        public Guid Id { get; }
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

        public void AddConversation(string prompt, string response, bool split) => _conversations.AddOrUpdate(new Conversation(prompt, response, split));
    }

    internal class WindowViewModel : ReactiveObject
    {
        private readonly ConversationService _cs = new ConversationService();
        private readonly ReadOnlyObservableCollection<ConversationViewModel> _conversations;
        public ReadOnlyObservableCollection<ConversationViewModel> Conversations => _conversations;

        public WindowViewModel()
        {
            for (int i = 0; i < 3; i++)
            {
                _cs.AddConversation("Explain quantum computing in simple terms", "Quantum computing is a type of computing that uses quantum mechanics to process information. Traditional computers use bits, which can only be either 0 or 1. Quantum computers, on the other hand, use quantum bits, or qubits, which can be 0, 1, or a superposition of both.\r\n\r\nThis allows quantum computers to perform certain calculations much faster than traditional computers. For example, a quantum computer could factor large numbers exponentially faster than a traditional computer, which is important for cryptography and cybersecurity.\r\n\r\nQuantum computing is still a developing field, and the technology is not yet widely available. However, researchers are actively working on building more powerful quantum computers, and there is a lot of excitement about the potential applications of this technology in fields like drug discovery, financial modeling, and more.", i == 2);
            }

            _cs.Conversations
                .Connect()
                .Transform(x => new ConversationViewModel(x))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _conversations)
                .Subscribe();
        }
    }
}
