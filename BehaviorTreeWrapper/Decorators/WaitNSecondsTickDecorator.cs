using BehaviorTreeWrapper.AbstractDecoratorsListeners;

namespace BehaviorTreeWrapper.Decorators
{
    public class WaitNSecondsTickDecorator : BannerlordTickTimedDecorator
    {
        bool hasBeenNotified = false;
        public WaitNSecondsTickDecorator(double timeToWait ) : base(timeToWait) { }
        public override bool Evaluate()
        {
            if (hasBeenNotified)
            {
                hasBeenNotified = false;
                return true;
            }
            else return false;
        }
        public override void Notify(object[] data) { hasBeenNotified = true; }
    }
}