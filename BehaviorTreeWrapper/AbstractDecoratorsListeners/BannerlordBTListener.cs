using BehaviorTrees;
using System.Collections.Generic;
using System.Threading;

namespace BehaviorTreeWrapper.AbstractDecoratorsListeners
{
    public class BannerlordBTListener : BTListener
    {
        public SubscriptionPossibilities SubscribesTo { get; set; }
        internal BannerlordBTListener(SubscriptionPossibilities subscribesTo, BehaviorTree tree, INotifiable notifies) : base(tree, notifies)
        {
            SubscribesTo = subscribesTo;
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
