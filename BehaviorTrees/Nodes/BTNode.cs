using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BehaviorTrees.Nodes
{
    public abstract class BTNode
    {
        public BTNode Parent;
        protected BehaviorTree BaseTree { get; set; }
        public int weight;
        public virtual AbstractDecorator? Decorator
        {
            get
            {
                return null;
            }
        }
        public BTStatus Status { get; set; } = BTStatus.FinishedWithTrue;
        protected BTNode(int weight = 100) { this.weight = weight; }
        public virtual BTNode Execute() { return this; }
        public Task<bool> AddDecoratorsListeners(CancellationToken cancellationToken)
        {
            return ((BTEventDecorator)Decorator).AddListener(cancellationToken);
        }
    }
}
