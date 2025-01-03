using BehaviorTree;
using System.Collections.Generic;
using static TaleWorlds.MountAndBlade.Agent;

namespace BehaviorTreeWrapper
{
    public class BannerlordBTListener : BTListener
    {
        public SubscriptionPossibilities SubscribesTo { get; set; }
        public BannerlordBTListener(SubscriptionPossibilities subscribesTo, BehaviorTree.BehaviorTree tree, INotifiable notifies) : base(tree, notifies)
        {
            SubscribesTo = subscribesTo;
            Subscribe();
        }
        public override void Subscribe()
        {
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
    //public class BannerlordBTStandaloneListener : BannerlordBTListener 
    //{

    //    public BannerlordBTStandaloneListener(SubscriptionPossibilities subscribesTo) : base(subscribesTo) { }

    //}
    //public class BannerlordBTDecoratorListener : BannerlordBTListener, BTDecoratorListener
    //{
    //    public BTDecorator Decorator { get; set; }

    //    public BannerlordBTDecoratorListener(BannerlordDecorator bannerlordDecorator, SubscriptionPossibilities subscribesTo) : base(subscribesTo)
    //    {
    //        Decorator = bannerlordDecorator;
    //    }
    //    public void Notify()
    //    {
    //        Decorator.Evaluate();
    //    }
    //}
    public abstract class BannerlordDecorator : BTDecorator
    {
        protected BannerlordDecorator(BehaviorTree.BehaviorTree tree, SubscriptionPossibilities subscribesTo) : base(tree)
        {
            listener = new BannerlordBTListener(subscribesTo, tree, this);
        }
        public override void Update()
        {
        }
    }
    public class AlarmedDecorator : BannerlordDecorator
    {
        BasicTree tree;
        private bool alreadyAlarmed = false;
        public AlarmedDecorator(BasicTree tree, SubscriptionPossibilities SubscribesTo) : base(tree, SubscribesTo)
        {
            this.tree = tree;
        }
        public override bool Evaluate()
        {
            if ((tree.Agent.AIStateFlags & AIStateFlag.Alarmed) == AIStateFlag.Alarmed && !alreadyAlarmed)
            {
                alreadyAlarmed = true;
                return true;
            }
            return false;
        }
        public override void Notify(List<object> data)
        {
            base.Notify(data);
        }
    }

    public class HitDecorator : BannerlordDecorator
    {
        BasicTree tree;
        private bool hasBeenHit = false;
        public HitDecorator(BasicTree tree, SubscriptionPossibilities SubscribesTo) : base(tree, SubscribesTo)
        {
            this.tree = tree;
        }
        public override bool Evaluate()
        {
            if (!hasBeenHit) return false;
            hasBeenHit = false;
            return true;
        }
        public override void Notify(List<object> data)
        {
            hasBeenHit = true;
        }
    }
}
