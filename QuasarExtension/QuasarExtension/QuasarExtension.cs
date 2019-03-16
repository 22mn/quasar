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
            Quasar = new MenuItem { Header = "Quasar" };
            NodesInGraph = new MenuItem { Header = "Nodes In Graph" };
            About = new MenuItem { Header = "About" };
            Freeze = new MenuItem { Header = "Freeze Selection" };
            Unfreeze = new MenuItem { Header = "Unfreeze Selection" };

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
            
            Freeze.Click += (sender, args) =>
            {
                var viewModel = new QuasarFreeze(loaded); 

            };

            Unfreeze.Click += (sender, args) =>
            {
                var viewModel = new QuasarUnfreeze(loaded);
            };


            Quasar.Items.Add(About);
            Quasar.Items.Add(Freeze);
            Quasar.Items.Add(Unfreeze);
            Quasar.Items.Add(NodesInGraph);
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
