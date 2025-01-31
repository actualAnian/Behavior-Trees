using BehaviorTrees;
using System.Threading;

namespace BehaviorTreeWrapper.AbstractDecoratorsListeners
{
    public class BannerlordBTTickListener : BTListener
    {
        // Subscribes to: SubscriptionPossibilities.OnMissionTick
        public double SecondsTillEvent { get; }
        internal BannerlordBTTickListener(double secondsTillEvent, BehaviorTree tree, INotifiable notifies) : base(tree, notifies)
        {
            SecondsTillEvent = secondsTillEvent;
        }
        public override void Subscribe(CancellationToken cancellationToken)
        {
            base.Subscribe(cancellationToken);
            BehaviorTreeBannerlordWrapper.Instance.Subscribe(this);
        }
        public override void UnSubscribe()
        {
            BehaviorTreeBannerlordWrapper.Instance.UnSubscribe(this);
        }
    }
}
