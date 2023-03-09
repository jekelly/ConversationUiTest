namespace ConversationUiTest.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Context
    {
    }

    public abstract class QuickActionProvider
    {
        public string Title { get; }

        public string Icon { get; }

        public IObservable<bool> Enabled { get; }

        protected abstract bool IsEnabledForContext(string context);

        public QuickActionProvider(string title, string icon, Predicate<string> isAvailable)
        {
            this.Title = title;
            this.Icon = icon;
        }
    }
    //this.QuickActions.Add(new QuickActionViewModel("fix", "🛠", this));
    //this.QuickActions.Add(new QuickActionViewModel("repair", "🤕", this));
    //this.QuickActions.Add(new QuickActionViewModel("explain","👩‍🏫",   this));
}

/*
 * datamodel
 * 
 * conversations (list of conversations)
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
 *                  
 * computed:
 *  current conversation: most recent conversation that is not closed in the conversation service.
 *      
 *      
 * fullcontext: prompt + context 
 * 
 * observable: active conversation
 * observable of fullcontext for active conversation
 *  */
