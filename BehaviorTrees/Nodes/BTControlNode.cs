using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BehaviorTrees.Nodes
{
    public abstract class BTControlNode : BTNode
    {
        protected int alreadyExecutedNodes = 0;
        protected List<BTNode> allChildren;
        protected List<BTNode> currentlyExecutableChildren = new();
        protected bool hasRunChild = false;
        private readonly AbstractDecorator? _decorator;
        public string Name { get; internal set; }
        public override AbstractDecorator? Decorator => _decorator;
        protected bool IsWaitingASingleTime { get; set; } = false;

        protected BTControlNode(BehaviorTree tree, string name, AbstractDecorator? decorator = null, List<BTNode>? children = null, int weight = 100) : base(weight)
        {
            BaseTree = tree;
            Name = name;
            _decorator = decorator;
            allChildren = children ?? new();
        }
        public override bool ShouldExitTree()
        {
            if (Status == BTStatus.WaitingForEvent) return true;
            if (IsWaitingASingleTime == true)
            {
                IsWaitingASingleTime = false;
                return true;
            }
            return false;
        }

        internal void ResetChildren()
        {
            allChildren.ForEach(c => c.Status = BTStatus.NotExecuted);
        }
        internal void AddChild(BTNode child)
        {
            allChildren.Add(child);
        }
        public BTControlNode BuildNode(BTNode nextNode)
        {
            allChildren.Add(nextNode);
            return this;
        }
        protected void RemoveDecorator(BTEventDecorator decorator)
        {
            decorator.Remove();
        }
        protected bool ExecutedAll()
        {
            return alreadyExecutedNodes >= allChildren.Count;
        }
    }
}
