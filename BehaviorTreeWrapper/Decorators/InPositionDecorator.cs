using BehaviorTrees;
using BehaviorTreeWrapper.AbstractDecoratorsListeners;
using BehaviorTreeWrapper.BlackBoardClasses;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BehaviorTreeWrapper.Decorators
{
    public class InPositionDecorator : BannerlordTickTimedDecorator, IBTBannerlordBase
    {
        private Vec3 position;
        public InPositionDecorator(Vec3 position) : base(1)
        {
            this.position = position;
        }
        BTBlackboardValue<Agent> _agent;
        public BTBlackboardValue<Agent> Agent { get => _agent; set => _agent = value; }

        public override bool Evaluate()
        {
            return Agent.GetValue().Position.Distance(position) < 2;
        }
        public override void Notify(object[] data) { }
    }
}
