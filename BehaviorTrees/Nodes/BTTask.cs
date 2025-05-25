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
        public abstract BTTaskStatus Execute();
        public override BTNode HandleExecute()
        {
            if (Decorator != null && !Decorator.Evaluate())
            {
                if (Decorator is BTEventDecorator)
                {
                    Status = BTStatus.WaitingForEvent;
                }
                else
                {
                    Status = BTStatus.FinishedWithFalse;
                }
                Status = BTStatus.Running;
                return this;
            }
            switch (Execute())
            {
                case BTTaskStatus.Running:
                    Status = BTStatus.Running;
                    break;
                case BTTaskStatus.FinishedWithFalse:
                    Status = BTStatus.FinishedWithFalse;
                    break;
                case BTTaskStatus.FinishedWithTrue:
                    Status = BTStatus.FinishedWithTrue;
                    break;
            }
            return Parent;
        }
    }
}
