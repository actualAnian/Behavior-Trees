using BehaviorTreeWrapper.AbstractDecoratorsListeners;
using BehaviorTreeWrapper.BlackBoardClasses;
using TaleWorlds.Library;

namespace BehaviorTreeWrapper.Decorators
{
    public class InPositionDecorator : BannerlordTickTimedDecorator
    {
        private Vec3 _position;
        readonly BTBlackboardBannerlordBase _bbBase;
        public InPositionDecorator(Vec3 position, BTBlackboardBannerlordBase bbBase) : base(1)
        {
            _position = position;
            _bbBase = bbBase;
        }
        public override bool Evaluate()
        {
            return _bbBase.Agent.Position.Distance(_position) < 2;
        }
        public override void Notify(object[] data) { }
    }
}
