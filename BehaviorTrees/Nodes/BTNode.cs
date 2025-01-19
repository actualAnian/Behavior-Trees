using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BehaviorTrees.Nodes
{
    public abstract class BTNode
    {
        protected BehaviorTree BaseTree { get; set; }
        public virtual AbstractDecorator? Decorator
        {
            get
            {
                return null;
            }
        }
        protected BTNode() { }
        public virtual async Task<bool> Execute(CancellationToken cancellationToken) { return true; }
        public Task<bool>? AddDecoratorsListeners(CancellationToken cancellationToken)
        {
            return Decorator?.AddListener(cancellationToken);
        }
    }
}
