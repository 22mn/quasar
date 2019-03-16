using System;
using System.Windows;
using System.Windows.Controls;
using Dynamo.Wpf.Extensions;
using Dynamo.Graph.Nodes;

namespace QuasarExtension
{
    public class QuasarExtension : IViewExtension
    {
        private MenuItem Quasar;
        private MenuItem NodesInGraph;
        private MenuItem About;
        private MenuItem Freeze;
        private MenuItem Unfreeze;
        

        public void Dispose() { }

        public void Startup(ViewStartupParams startup)
        {
            
        }

        public void Loaded(ViewLoadedParams loaded)
        {
            // Quasar Main Menu
            Quasar = new MenuItem { Header = "Quasar" };
            Quasar.ToolTip = new ToolTip { Content = "Quasar Package v2.0.102" };

            // NodesInGraph MenuItem
            NodesInGraph = new MenuItem { Header = "Nodes In Graph" };
            NodesInGraph.ToolTip = new ToolTip { Content = "Each node info in this graph" };

            // About MenuItem
            About = new MenuItem { Header = "About" };
            About.ToolTip = new ToolTip { Content = "About Quasar Extension" };

            // Freeze MenuItem
            Freeze = new MenuItem { Header = "Freeze Selection" };
            Freeze.ToolTip = new ToolTip { Content = "Freeze current selected nodes" };

            // Unfreeze MenuItem
            Unfreeze = new MenuItem { Header = "Unfreeze Selection" };
            Unfreeze.ToolTip = new ToolTip { Content = "Unfreeze current selected nodes" };


            /* **EVENT REGION START** */

            // ~ NodesInGraph Click Event ~  
            NodesInGraph.Click += (sender, args) =>
            {
                var viewModel = new QuasarWindowViewModel(loaded);
                var window = new QuasarWindow
                {
                    MainGrid = { DataContext = viewModel },
                    Owner = loaded.DynamoWindow
                };
                window.Left = window.Owner.Left + 300;
                window.Top = window.Owner.Top + 250;
                window.Show();
            };

            // ~ About Click Event ~ 
            About.Click += (sender, args) =>
            {
                var viewModel = new QuasarAbout(loaded);
                var window = new QuasarAboutWindow
                {
                    MainGrid = { DataContext = viewModel },
                    Owner = loaded.DynamoWindow
                };
                window.Left = window.Owner.Left + 200;
                window.Top = window.Owner.Top + 100;
                window.Show();
            };
            
            // ~ Freeze Click Event ~
            Freeze.Click += (sender, args) =>
            {
                var viewModel = new QuasarFreeze(loaded); 

            };

            // ~ Unfreeze Click Event ~
            Unfreeze.Click += (sender, args) =>
            {
                var viewModel = new QuasarUnfreeze(loaded);
            };



            // Add MenuItems 
            Quasar.Items.Add(Freeze);
            Quasar.Items.Add(Unfreeze);
            Quasar.Items.Add(NodesInGraph);
            Quasar.Items.Add(new Separator());
            Quasar.Items.Add(About);
            loaded.dynamoMenu.Items.Add(Quasar);
        }
        public void Shutdown() { }

        public string UniqueId
        {
            get
            {
                return Guid.NewGuid().ToString();
            }
        }
        public string Name
        {
            get
            {
                return "Quasar Extension";
            }
        }

    }
}
