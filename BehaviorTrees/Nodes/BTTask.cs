using System.Threading;
using System.Threading.Tasks;

namespace BehaviorTrees.Nodes
{
    public abstract class BTTask : BTNode
    {
        BTListener? isExecutedListener;
        protected BTTask(BTListener? listener = null) : base()
        {
            isExecutedListener = listener;
        }

        public override async Task<bool> Execute(CancellationToken cancellationToken)
        {
            return true;
        }
    }
}
