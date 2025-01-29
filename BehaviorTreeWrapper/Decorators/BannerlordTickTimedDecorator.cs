using BehaviorTrees;

namespace BehaviorTreeWrapper.Decorators
{
    public abstract class BannerlordTickTimedDecorator : BTDecorator
    {
        //Subscribes to SubscriptionPossibilities.OnMissionTick;
        private readonly double secondsTillEvent;
        protected BannerlordTickTimedDecorator(double secondsTillEvent = 0.66667) : base(OnDecoratorFalse.AwaitEvent)
        {
            this.secondsTillEvent = secondsTillEvent;
        }
        public override sealed void CreateListener()
        {
            listener = new BannerlordBTTickListener(secondsTillEvent, Tree, this);
        }
    }
}
