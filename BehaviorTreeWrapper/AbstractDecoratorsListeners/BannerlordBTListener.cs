using BehaviorTrees;
using System.Collections.Generic;
using System.Threading;

namespace BehaviorTreeWrapper.AbstractDecoratorsListeners
{
    public class BannerlordBTListener : BTListener
    {
        public SubscriptionPossibilities SubscribesTo { get; set; }
        internal BannerlordBTListener(SubscriptionPossibilities subscribesTo, BehaviorTree tree, BTEventDecorator notifies) : base(tree, notifies)
        {
            SubscribesTo = subscribesTo;
        }
        public override void Subscribe()
        {
            base.Subscribe();
            BehaviorTreeBannerlordWrapper.Instance.Subscribe(this);
        }
        public override void UnSubscribe()
        {
            BehaviorTreeBannerlordWrapper.Instance.UnSubscribe(this);
        }
    }
}
