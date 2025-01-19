using BehaviorTrees;
using System.Collections.Generic;
using System.Threading;

namespace BehaviorTreeWrapper.Decorators
{
    public class BannerlordBTListener : BTListener
    {
        public SubscriptionPossibilities SubscribesTo { get; set; }
        public BannerlordBTListener(SubscriptionPossibilities subscribesTo, BehaviorTree tree, INotifiable notifies) : base(tree, notifies)
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
        public override void Notify(List<object> data)
        {
            Notifies.Notify(data);
            Signal(true);
        }
        public void NotifyWithCancel()
        {
            Signal(false);
        }
    }
}
