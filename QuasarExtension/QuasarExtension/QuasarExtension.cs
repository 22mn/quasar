using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Dynamo.Wpf.Extensions;
using Dynamo.Graph.Nodes;
using Dynamo.ViewModels;

namespace QuasarExtension
{
    public class QuasarExtension : IViewExtension
    {
        private MenuItem Quasar;
        private MenuItem About;
        private MenuItem Freeze;
        private MenuItem Unfreeze;
        private MenuItem Label;
        private MenuItem SampleFileMenu;
        //private MenuItem NodesInGraph;
        //private MenuItem FunctionTest;


        public void Dispose() { }

        public void Startup(ViewStartupParams startup)
        {
            
        }

        public void Loaded(ViewLoadedParams loaded)
        {

            DynamoViewModel dynamoViewModel = loaded.DynamoWindow.DataContext as DynamoViewModel;

            // Quasar Main Menu
            Quasar = new MenuItem { Header = "Quasar" };
            Quasar.ToolTip = new ToolTip { Content = "Quasar Package v2.0.102" };
            
            /*
            // NodesInGraph MenuItem
            NodesInGraph = new MenuItem { Header = "Nodes In Graph" };
            NodesInGraph.ToolTip = new ToolTip { Content = "Nodes info in this graph" };
            */

            // About MenuItem
            About = new MenuItem { Header = "About" };
            About.ToolTip = new ToolTip { Content = "About Quasar Extension" };

            // Freeze MenuItem
            Freeze = new MenuItem { Header = "Freeze Selection" };
            Freeze.ToolTip = new ToolTip { Content = "Freeze current selected nodes" };

            // Unfreeze MenuItem
            Unfreeze = new MenuItem { Header = "Unfreeze Selection" };
            Unfreeze.ToolTip = new ToolTip { Content = "Unfreeze current selected nodes" };
            
            /*
            // FunctionTest MenuItem
            FunctionTest = new MenuItem { Header = "Function Test" };
            FunctionTest.ToolTip = new ToolTip { Content = "Testing Dynamo API Methods" };
            */


            // Samples MenuItems
            Label = new MenuItem { Header = "Label Selection" };
            Label.ToolTip = new ToolTip { Content = " Add description label to selected nodes" };

            SampleFileMenu = new MenuItem { Header = "Quasar Samples" };
            SampleFileMenu.ToolTip = new ToolTip { Content = " Sample files for node usage" };

            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var path = Path.Combine(appdata, "Dynamo\\Dynamo Revit\\2.0\\packages\\Quasar\\extra\\samples");
            var SubMenuItem = Directory.GetDirectories(path);

            foreach (string sub in SubMenuItem)
            {
                MenuItem subMenu = new MenuItem { Header = sub.Split('\\').Last()};
                //subMenu.ToolTip = new ToolTip { Content = sub.Split('\\').Last()};
                //SampleFileMenu.Items.Add(subMenu);
                MenuItem subMenuItems = SampleFiles(sub, subMenu, dynamoViewModel);
                SampleFileMenu.Items.Add(subMenuItems);
            }




            /* **EVENT REGION START** */

            /*
            // ~ NodesInGraph Click Event ~  
            NodesInGraph.Click += (sender, args) =>
            {
                var viewModel = new QuasarNodeInGraph(loaded);
                var window = new QuasarWindow
                {
                    MainGrid = { DataContext = viewModel },
                    Owner = loaded.DynamoWindow
                };
                window.Left = window.Owner.Left + 300;
                window.Top = window.Owner.Top + 250;
                window.Show();
            };
            */


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

            // ~ Label MenuItem
            Label.Click += (sender, args) =>
             {
                 var viewModel = new QuasarLabel(loaded, dynamoViewModel);
                 
             };

            // ~ Sample MenuItem

            SampleFileMenu.Click += (sender, args) =>
            {
                var viewModel = new QuasarSampleFile(loaded, dynamoViewModel);
            };


            // Add MenuItems
            
            //Quasar.Items.Add(FunctionTest);

            Quasar.Items.Add(Label);
            //Quasar.Items.Add(new Separator());
            Quasar.Items.Add(Freeze);
            Quasar.Items.Add(Unfreeze);
            // Quasar.Items.Add(NodesInGraph);
            Quasar.Items.Add(new Separator());
            Quasar.Items.Add(SampleFileMenu);
            Quasar.Items.Add(new Separator());
            Quasar.Items.Add(About);
            loaded.dynamoMenu.Items.Add(Quasar);
        }


        public MenuItem SampleFiles(string path, MenuItem submenu,DynamoViewModel dynamoViewModel)
        {
            string[] files = Directory.GetFiles(path,"*.dyn");
            foreach(var file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                MenuItem menu = new MenuItem { Header = file.Split('\\').Last() };
                menu.Click += (sender, args) =>
                  {
                      if (File.Exists(file))
                      {
                          dynamoViewModel.CloseHomeWorkspaceCommand.Execute(null);
                          dynamoViewModel.OpenCommand.Execute(file);
                      }
                      else { MessageBox.Show("File "+ file.Split('\\').Last() + " not found.", "Error Message!"); };
                  };
                submenu.Items.Add(menu);

            }
            return submenu;

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
