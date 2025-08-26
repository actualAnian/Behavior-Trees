using BehaviorTrees;

namespace BehaviorTreeWrapper.AbstractDecoratorsListeners
{
    public abstract class BannerlordConstantEventListener : ConstantEventListener
    {
        SubscriptionPossibilities _subscribesTo;
        public BannerlordConstantEventListener(SubscriptionPossibilities subscribesTo)
        {
            _subscribesTo = subscribesTo;
        }
        public override abstract void Notify(object[] data);
        public override void CreateListener()
        {
            Listener = new BannerlordBTListener(_subscribesTo, Tree, this);
            Listener.Subscribe();
        }
    }
}
