using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BehaviorTrees.Nodes
{
    public abstract class BTNode
    {
        public BTNode Parent; // set up while building the tree
        protected BehaviorTree BaseTree { get; set; }
        public int weight;
        public virtual AbstractDecorator? Decorator
        {
            get
            {
                return null;
            }
        }
        public BTStatus Status { get; set; } = BTStatus.NotExecuted;
        protected BTNode(int weight = 100) {this.weight = weight; }
        public abstract BTNode HandleExecute();// { return this; }
        public virtual bool ShouldExitTree() { return false; }

        public void AddDecoratorsListeners()
        {
            ((BTEventDecorator)Decorator).AddListener();
        }
    }
}
