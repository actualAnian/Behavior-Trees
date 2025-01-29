using System.Threading.Tasks;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using SandBox;
using TaleWorlds.Engine;
using System.Threading;
using BehaviorTrees.Nodes;
using BehaviorTrees;
using BehaviorTreeWrapper.BlackBoardClasses;

namespace BehaviorTreeWrapper.Tasks
{
    public class MoveToPlaceTask : BTTask, IBTMovable, IBTBannerlordBase
    {
        BTBlackboardValue<AgentNavigator> _navigator;
        public BTBlackboardValue<AgentNavigator> Navigator { get => _navigator; set => _navigator = value; }
        BTBlackboardValue<Agent> _agent;
        public BTBlackboardValue<Agent> Agent { get => _agent; set => _agent = value; }
        Vec3 moveTo;
        public MoveToPlaceTask(Vec3 moveTo) : base() { this.moveTo = moveTo; }

        public override async Task<bool> Execute(CancellationToken cancellationToken)
        {
            WorldPosition position = new WorldPosition(Mission.Current.Scene, moveTo);
            Navigator.GetValue().SetTargetFrame(position, Agent.GetValue().Frame.rotation.f.AsVec2.RotationInRadians);
            return true;
        }
    }
}
