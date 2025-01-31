using BehaviorTrees;
using BehaviorTreeWrapper.AbstractDecoratorsListeners;
using BehaviorTreeWrapper.BlackBoardClasses;
using System.Collections.Generic;
using TaleWorlds.MountAndBlade;
using static TaleWorlds.MountAndBlade.Agent;

namespace BehaviorTreeWrapper.Decorators
{
    public class AlarmedDecorator : BannerlordEventDecorator, IBTBannerlordBase
    {
        BTBlackboardValue<Agent> _agent;
        public BTBlackboardValue<Agent> Agent { get => _agent; set => _agent = value; }

        private bool alreadyAlarmed = false;
        public AlarmedDecorator(SubscriptionPossibilities SubscribesTo) : base(SubscribesTo) { }

        public override bool Evaluate()
        {
            if ((Agent.GetValue().AIStateFlags & AIStateFlag.Alarmed) == AIStateFlag.Alarmed && !alreadyAlarmed)
            {
                alreadyAlarmed = true;
                return true;
            }
            return false;
        }
        public override void Notify(object[] data) { }
    }
}
