using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;
using System.Windows;
using System;
using System.Windows.Input;

namespace ConversationUiTest
{
    internal class QuickActionViewModel : ReactiveObject
    {
        private ObservableAsPropertyHelper<Visibility> _visibility;
        public Visibility Visibility => _visibility.Value;

        public string Name { get; }

        public ICommand Command { get; }

        // todo: Context instead of string
        public QuickActionViewModel(string match, string name, WindowViewModel parent, Action callback, Predicate<string> canCall)
        {
            _visibility = parent.WhenAnyValue(vm => vm.NewQuery)
                .Select(query => query?.Contains(match) ?? false ? Visibility.Visible : Visibility.Collapsed)
                .ToProperty(this, vm => vm.Visibility, Visibility.Collapsed);

            this.Name = name;

            this.Command = ReactiveCommand.Create(callback, parent.WhenAnyValue(vm => vm.NewQuery).Select(query => string.IsNullOrEmpty(query) ? false : canCall(query)));
        }
    }
}

