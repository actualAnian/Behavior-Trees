using BehaviorTrees;

namespace BehaviorTreeWrapper.AbstractDecoratorsListeners
{
    public abstract class BannerlordEventDecorator : BTEventDecorator
    {
        private readonly SubscriptionPossibilities subscribesTo;
        protected BannerlordEventDecorator(SubscriptionPossibilities subscribesTo) : base()
        {
            this.subscribesTo = subscribesTo;
        }
        public override sealed void CreateListener()
        {
            listener = new BannerlordBTListener(subscribesTo, Tree, this);
        }
    }
}
