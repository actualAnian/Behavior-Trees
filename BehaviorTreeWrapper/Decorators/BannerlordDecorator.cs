using BehaviorTrees;

namespace BehaviorTreeWrapper.Decorators
{
    public abstract class BannerlordDecorator : BTDecorator
    {
        private readonly SubscriptionPossibilities subscribesTo;
        protected BannerlordDecorator(SubscriptionPossibilities subscribesTo, OnDecoratorFalse onFalse) : base(onFalse)
        {
            this.subscribesTo = subscribesTo;
        }
        public override sealed void CreateListener()
        {
            listener = new BannerlordBTListener(subscribesTo, Tree, this);
        }
    }
}
