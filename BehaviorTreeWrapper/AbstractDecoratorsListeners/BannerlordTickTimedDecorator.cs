using BehaviorTrees;

namespace BehaviorTreeWrapper.AbstractDecoratorsListeners
{
    public abstract class BannerlordTickTimedDecorator : BTEventDecorator
    {
        //Subscribes to SubscriptionPossibilities.OnMissionTick;
        private readonly double secondsTillEvent;
        protected BannerlordTickTimedDecorator(double secondsTillEvent = 0.66667) : base()
        {
            this.secondsTillEvent = secondsTillEvent;
        }
        public override sealed void CreateListener()
        {
            Listener = new BannerlordBTTickListener(secondsTillEvent, Tree, this);
        }
    }
}
