namespace ConversationUiTest.Models
{
    using DynamicData;
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public record Conversation
    {
        public Guid Id { get; } = Guid.NewGuid();

        public string Title { get; }

        public ConversationContext Context { get; } = new ConversationContext();

        // TODO: how mutable do we want the data model to be?
        // public IImmutableList<Exchange> Exchanges { get; }?
        private readonly ISourceCache<Exchange, Guid> _exchanges = new SourceCache<Exchange, Guid>(e => e.Id);

        public IObservableCache<Exchange, Guid> Exchanges => _exchanges.AsObservableCache();

        public Conversation(string title)
        {
            this.Title = title;
        }

        public void Ask(string prompt, string response, bool split = false)
        {
            Exchange e = new Exchange(prompt, response, split);
            _exchanges.AddOrUpdate(e);
        }

        /*
 *  conversation
 *      title
 *      context (property bag)
 *      list of exchanges
 *          exchange
 *            prompt
 *              text (string)
 *              list of actions
 *                  action name
 *                  
 *            response
 *              list of actions
        */
    }
}
