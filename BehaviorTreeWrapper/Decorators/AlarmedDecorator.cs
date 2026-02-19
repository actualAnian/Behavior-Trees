using BehaviorTrees;
using BehaviorTreeWrapper.AbstractDecoratorsListeners;
using BehaviorTreeWrapper.BlackBoardClasses;
using TaleWorlds.MountAndBlade;
using static TaleWorlds.MountAndBlade.Agent;

namespace BehaviorTreeWrapper.Decorators
{
    public class AlarmedDecorator : BannerlordEventDecorator
    {
        readonly BTBlackboardBannerlordBase _bbBase;

        private bool alreadyAlarmed = false;
        public AlarmedDecorator(SubscriptionPossibilities SubscribesTo, BTBlackboardBannerlordBase bbBase) : base(SubscribesTo) { _bbBase = bbBase; }

        public override bool Evaluate()
        {
            if ((_bbBase.Agent.AIStateFlags & AIStateFlag.Alarmed) == AIStateFlag.Alarmed && !alreadyAlarmed)
            {
                alreadyAlarmed = true;
                return true;
            }
            return false;
        }
        public override void Notify(object[] data) { }
    }
}
