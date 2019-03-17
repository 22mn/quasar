using System;
using System.Linq;
using System.Collections.Generic;
using Dynamo.Core;
using Dynamo.Extensions;
using Dynamo.Graph;
using Dynamo.Graph.Notes;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Connectors;
using Dynamo.ViewModels;
using Dynamo.Graph.Workspaces;
using Dynamo.Models;

namespace QuasarExtension
{
    /*
    public class QuasarNodeInGraph : NotificationObject, IDisposable
    {
        private string currentNodes;
        private ReadyParams readyParams;

        public string CurrentNodes
        {
            get
            {
                currentNodes = GetNodes();
                return currentNodes;
            }
        }

        public string GetNodes()
        {
            string fileName = readyParams.CurrentWorkspaceModel.Name.ToString();
            int count = 1;
            string outputFormat = String.Format(" Nodes in {0}.dyn \n\n", fileName);
            foreach (NodeModel node in readyParams.CurrentWorkspaceModel.Nodes)
            {
                string name = node.Name;
                string package = node.Category.Split('.')[0].ToString();
                outputFormat += String.Format("{0}. {1} ({2}) | {3}\n", count, name, package, node.State);
                count += 1;
            }
            return outputFormat;
        }
        private void NodesCount_Changed(NodeModel node)
        {
            RaisePropertyChanged("CurrentNodes");
        }

        private void WiresCount_Changed(ConnectorModel wire)
        {
            RaisePropertyChanged("CurrentNodes");
        }

        public QuasarNodeInGraph(ReadyParams ready)
        {
            readyParams = ready;
            ready.CurrentWorkspaceModel.NodeAdded += NodesCount_Changed;
            ready.CurrentWorkspaceModel.NodeRemoved += NodesCount_Changed;
            ready.CurrentWorkspaceModel.ConnectorAdded += WiresCount_Changed;
            ready.CurrentWorkspaceModel.ConnectorDeleted += WiresCount_Changed;
        }

        public void Dispose()
        {
            readyParams.CurrentWorkspaceModel.NodeAdded -= NodesCount_Changed;
            readyParams.CurrentWorkspaceModel.NodeRemoved -= NodesCount_Changed;
            readyParams.CurrentWorkspaceModel.ConnectorAdded -= WiresCount_Changed;
            readyParams.CurrentWorkspaceModel.ConnectorDeleted -= WiresCount_Changed;
        }
    }
    */
    public class QuasarAbout : NotificationObject, IDisposable
    {

        private readonly ReadyParams readyParams;
        public QuasarAbout(ReadyParams r)
        {
            readyParams = r;
        }

        public void Dispose() { }
    }
  
    public class QuasarFreeze : NotificationObject, IDisposable
    {
        private readonly ReadyParams readyParams;

        public ReadyParams Ready
        {
            get
            {
                return readyParams;
            }
        }


        public QuasarFreeze(ReadyParams r)
        {
            readyParams = r;
            var nodes = r.CurrentWorkspaceModel.CurrentSelection;
            foreach(NodeModel node in nodes)
            {
                if (node.IsSelected && !node.IsFrozen)
                {
                    node.IsFrozen = true;
                }
            }
        }
        public void Dispose() { }
        
    }
    public class QuasarUnfreeze: NotificationObject, IDisposable
    {
        private readonly ReadyParams readyParams;
        public ReadyParams Ready
        {
            get
            {
                return readyParams;
            }
        }

        public QuasarUnfreeze(ReadyParams r)
        {
            readyParams = r;
            var nodes = r.CurrentWorkspaceModel.CurrentSelection;
            foreach (NodeModel node in nodes)
            {
                if (node.IsSelected && node.IsFrozen)
                {
                    node.IsFrozen = false;
                    
                }
            }
        }
        public void Dispose() { }
    }

    /*
    public class QuasarFunctionTest : NotificationObject, IDisposable
    {

        private readonly ReadyParams readyParams;
        public string output;

        public string Output
        {
            get { return output; }
        }

        // dynViewModel Function Test
        public QuasarFunctionTest(ReadyParams r, DynamoViewModel dynamoViewModel)
        {
            readyParams = r;
            WorkspaceModel workspace = readyParams.CurrentWorkspaceModel as WorkspaceModel;
            output += workspace.FileName.ToString();
            output += "/n";
            output += dynamoViewModel.Model.Version.ToString();

        }

        public void Dispose() { }
    }
    */
    public class QuasarLabel : NotificationObject, IDisposable
    {

        public QuasarLabel(ReadyParams r, DynamoViewModel dynamoViewModel)
        {
            // Create notes for current selection nodes
            var nodes = dynamoViewModel.Model.CurrentWorkspace.CurrentSelection;
            List<Guid> guids = new List<Guid>();
            foreach (NodeModel node in nodes)
            {
                if (node.IsSelected)  // node.IsCustomFunction can use to filter
                {
                    Guid newG = Guid.NewGuid();
                    string description = "- " + node.Description.Split('\n')[0];
                    var note = new DynamoModel.CreateNoteCommand(newG, description, node.CenterX-100, node.CenterY-75, false);

                    // Execute create command
                    dynamoViewModel.ExecuteCommand(note);
                    
                    // record note guid
                    guids.Add(newG);

                    // deselect node
                    node.Deselect();
                }
            };

            /*
            dynamoViewModel.Model.ClearCurrentWorkspace();
            var notes = dynamoViewModel.Model.CurrentWorkspace.Notes;
            foreach (var note in notes)
            {
                if (guids.Contains(note.GUID))
                {
                    dynamoViewModel.AddToSelectionCommand.Execute(note);
                }
            }
            */
        }
        public void Dispose() { }
    }

    
}
