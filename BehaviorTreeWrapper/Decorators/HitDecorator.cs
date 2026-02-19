using BehaviorTreeWrapper.AbstractDecoratorsListeners;

namespace BehaviorTreeWrapper.Decorators
{
    public class HitDecorator : BannerlordEventDecorator
    {
        private bool hasBeenHit = false;
        public HitDecorator(SubscriptionPossibilities SubscribesTo) : base(SubscribesTo) { }
        public override bool Evaluate()
        {
            if (!hasBeenHit) return false;
            hasBeenHit = false;
            return true;
        }
        public override void Notify(object[] data)
        {
            hasBeenHit = true;
        }
    }
}
