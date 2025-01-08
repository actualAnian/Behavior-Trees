using BehaviorTrees;
using System.Collections.Generic;
using TaleWorlds.Library;
using static TaleWorlds.MountAndBlade.Agent;

namespace BehaviorTreeWrapper
{
    public class BannerlordBTListener : BTListener
    {
        public SubscriptionPossibilities SubscribesTo { get; set; }
        public BannerlordBTListener(SubscriptionPossibilities subscribesTo, BehaviorTree tree, INotifiable notifies) : base(tree, notifies)
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
    public abstract class BannerlordDecorator<TTree> : BTDecorator<TTree> where TTree : BehaviorTree
    {
        protected BannerlordDecorator(TTree tree, SubscriptionPossibilities subscribesTo) : base(tree)
        {
            listener = new BannerlordBTListener(subscribesTo, tree, this);
        }
    }
    public class AlarmedDecorator : BannerlordDecorator<BannerlordTree>
    {
        private bool alreadyAlarmed = false;
        public AlarmedDecorator(BannerlordTree tree, SubscriptionPossibilities SubscribesTo) : base(tree, SubscribesTo) { }
        public override bool Evaluate()
        {
            if ((Tree.Agent.AIStateFlags & AIStateFlag.Alarmed) == AIStateFlag.Alarmed && !alreadyAlarmed)
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

    public class HitDecorator : BannerlordDecorator<BannerlordTree>
    {
        private bool hasBeenHit = false;
        public HitDecorator(MovementTree tree, SubscriptionPossibilities SubscribesTo) : base(tree, SubscribesTo) {}
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
    public class InPositionDecorator : BannerlordDecorator<MovementTree>
    {
        private Vec3 position;
        public InPositionDecorator(MovementTree tree, Vec3 position, SubscriptionPossibilities SubscribesTo) : base(tree, SubscribesTo)
        {
            this.position = position;
        }
        public override bool Evaluate()
        {
            return Tree.Agent.Position.Distance(position) < 2;
        }
        public override void Notify(List<object> data) {}
    }
}
