using System;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;

namespace ConversationUiTest
{
    public class Exchange
    {
        public string Prompt { get; }
        public IObservable<string> Response { get; }
        public Guid Id { get; }
        public string Title { get; }
        public Exchange(string prompt, string response, bool split)
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
}

