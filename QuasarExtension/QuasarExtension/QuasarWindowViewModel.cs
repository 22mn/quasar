using System;
using System.Linq;
using Dynamo.Core;
using Dynamo.Extensions;
using Dynamo.Graph.Nodes;


namespace QuasarExtension
{
    public class QuasarWindowViewModel : NotificationObject, IDisposable
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
            outputFormat += String.Format("** {0} [{1}] **\n\n", "Name", "Package/Category");
            foreach (NodeModel node in readyParams.CurrentWorkspaceModel.Nodes.OrderBy(o => o.Category).ToList())
            {
                string name = node.Name;
                string package = node.Category.Split('.')[0].ToString();
                outputFormat += String.Format("{0}. {1} [{2}] \n", count, name, package);
                count += 1;
            }
            return outputFormat;
        }
        private void NodesCount_Changed(NodeModel node)
        {
            RaisePropertyChanged("CurrentNodes");
        }

        public QuasarWindowViewModel(ReadyParams ready)
        {
            readyParams = ready;
            ready.CurrentWorkspaceModel.NodeAdded += NodesCount_Changed;
            ready.CurrentWorkspaceModel.NodeRemoved += NodesCount_Changed;
        }

        public void Dispose()
        {
            readyParams.CurrentWorkspaceModel.NodeAdded -= NodesCount_Changed;
            readyParams.CurrentWorkspaceModel.NodeRemoved -= NodesCount_Changed;
        }
    }
    public class QuasarFreezeViewModel : NotificationObject , IDisposable
    {
        private ReadyParams readyParams;

        public void SelectionFreeze()
        {
            // selection list includes nodes that are not frozen yet.
            var selection = readyParams.CurrentWorkspaceModel.CurrentSelection.Where(o => !o.IsFrozen).ToList();
            
            foreach(NodeModel node in selection)
            {
                node.IsFrozen = true;
            }
            
        }
        public QuasarFreezeViewModel(ReadyParams ready)
        {
            readyParams = ready;
        }

        public void Dispose()
        {

        }
    }
}
