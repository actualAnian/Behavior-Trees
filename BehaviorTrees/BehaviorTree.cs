using System.Collections.Generic;

namespace BehaviorTree
{
    public abstract class BehaviorTree : INotifiable
    {
        public List<BTListener> currentListeners;
        public ControlNode CurrentControlNode { get; set; }
        public ControlNode RootNode { get; set; }

        public BehaviorTree NotifiedTree { get { return this; } }

        public BehaviorTree()
        {
        }
        public BehaviorTree BuildTree(ControlNode rootNode, BTListener listener)
        {
            CurrentControlNode = RootNode = rootNode;
            //this.listener = listener;
            //listener.Subscribe();
            ExecuteNode();
            return this;
        }
        public BehaviorTree BuildTree(ControlNode rootNode)
        {
            CurrentControlNode = RootNode = rootNode;
            ExecuteNode();
            return this;
        }
        public async void ExecuteNode()
        {
            while (true)
            {
                await CurrentControlNode.Execute();
                //bool result = await listener.NotifyAsync();
                int a = 5;
            }
        }
        public void NotifyOnPropertyChange()
        {
            CurrentControlNode.Reevaluate();
        }
        public abstract void Notify();
    }
}
