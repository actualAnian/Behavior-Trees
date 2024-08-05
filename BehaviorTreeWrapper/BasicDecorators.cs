using BehaviorTree;
using static TaleWorlds.MountAndBlade.Agent;

namespace BehaviorTreeWrapper
{
    public class BannerlordBTListener : BTListener
    {
        public SubscriptionPossibilities SubscribesTo { get; set; }
        public INotifiable NotifiedObject { get; set; }
        public BannerlordBTListener(SubscriptionPossibilities subscribesTo, INotifiable notifiable) : base(notifiable)
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

        public void Notify()
        {
            throw new System.NotImplementedException();
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
            listener = new BannerlordBTListener(subscribesTo, this);
            //listener = new BannerlordBTDecoratorListener(this, subscribesTo);
        }
        public override void Update()
        {
        }
    }
    public class TrueDecorators : BannerlordDecorator
    {
        BasicTree tree;
        public TrueDecorators(BasicTree tree, SubscriptionPossibilities SubscribesTo) : base(tree, SubscribesTo)
        {
            this.tree = tree;
        }
        public override bool Evaluate()
        {
            return true;
        }
        public bool Evaluate(AIStateFlag flags)
        {
            //return flags.HasFlag
            tree.IsSuspicious = true;
            return true;
        }

        public override void Notify()
        {
            throw new System.NotImplementedException();
        }
    }








    //public class HealthBelow50PercentDecorators : IDecorator
    //{
    //    public string Name { get; set; }

    //    public bool Evaluate(Agent agent)
    //    {
    //        return (agent.HealthLimit / 2 > agent.Health);
    //    }

    //    public bool Evaluate()
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public bool notify()
    //    {
    //        return true;
    //    }

    //    public void update()
    //    {
    //        BehaviorTreeBannerlordWrapper.Instance.subscribe(this);
    //    }
    //}
}
