using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Engine;
using BehaviorTrees.Nodes;
using BehaviorTrees;
using BehaviorTreeWrapper.BlackBoardClasses;

namespace BehaviorTreeWrapper.Tasks
{
    public class MoveToPlaceTask : BTTask
    {
        readonly BTBlackboardBannerlordBase _bbBase;
        readonly BTBlackboardMovable _bbMovable;
        Vec3 _moveTo;
        public MoveToPlaceTask(Vec3 moveTo, BTBlackboardBannerlordBase bbBase, BTBlackboardMovable bbMovable) : base() 
        {
            _moveTo = moveTo;
            _bbMovable = bbMovable;
            _bbBase = bbBase;
        }

        public override BTTaskStatus Execute()
        {
            WorldPosition position = new WorldPosition(Mission.Current.Scene, _moveTo);
            _bbMovable.Navigator.SetTargetFrame(position, _bbBase.Agent.Frame.rotation.f.AsVec2.RotationInRadians);
            return BTTaskStatus.FinishedWithTrue;
        }
    }
}
