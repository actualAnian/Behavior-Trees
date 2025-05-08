using System.Threading;
using System.Threading.Tasks;

namespace BehaviorTrees.Nodes
{
    public abstract class BTTask : BTNode
    {
        BTListener? isExecutedListener;
        protected BTTask(BTListener? listener = null, int weight = 100) : base(weight)
        {
            isExecutedListener = listener;
        }
        public override BTNode Execute()
        {
            return Parent;
        }
    }
}
