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
}
