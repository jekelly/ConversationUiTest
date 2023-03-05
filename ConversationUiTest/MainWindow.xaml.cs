using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reactive;
using DynamicData.Binding;
using System.ComponentModel;
using DynamicData;

namespace ConversationUiTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IDisposable d;

        public MainWindow()
        {
            InitializeComponent();
            var generator = this.Conversations.ItemContainerGenerator;

            generator.StatusChanged += (o, e) =>
            {
                var container = this.Conversations.ItemContainerGenerator.ContainerFromIndex(this.Conversations.Items.Count - 1);
                (container as FrameworkElement)?.BringIntoView();
            };

            //this.Conversations.ItemContainerGenerator.ItemsChanged += (o, e) =>
            //{
            //    //var item = this.Conversations.Items[this.Conversations.Items.Count - 1];
            //    //var container = this.Conversations.ItemContainerGenerator.ContainerFromItem(item);
            //    var container = this.Conversations.ItemContainerGenerator.ContainerFromIndex(this.Conversations.Items.Count - 2);
            //    (container as FrameworkElement)?.BringIntoView();
            //};
        }
    }
}
