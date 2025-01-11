using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using SandBox.Conversation.MissionLogics;
using SandBox.Objects.AnimationPoints;
using SandBox.Objects.AreaMarkers;
using SandBox.Objects.Usables;
using SandBox;
using TaleWorlds.Engine;
using BehaviorTrees;
using System.Threading;

namespace BehaviorTreeWrapper
{
    public class PrintTask : BTTask<BannerlordTree>
    {
        public PrintTask() { }

        public override async Task<bool> Execute(CancellationToken cancellationToken)
        {
            MBInformationManager.AddQuickInformation(new TextObject("I spotted you!", null), 0, null, "");
            return true;
        }
    }
    public class PrintTask2 : BTTask<BannerlordTree>
    {
        public PrintTask2() : base() { }

        public override async Task<bool> Execute(CancellationToken cancellationToken)
        {
            MBInformationManager.AddQuickInformation(new TextObject("I am hit!", null), 0, null, "");
            return true;
        }
    }
    public class PrintTask3 : BTTask<BannerlordTree>
    {
        public PrintTask3() : base() { }

        public override async Task<bool> Execute(CancellationToken cancellationToken)
        {
            MBInformationManager.AddQuickInformation(new TextObject("Second message!", null), 0, null, "");
            return true;
        }
    }
    public class MoveToPlaceTask : BTTask<MovementTree>
    {
        Vec3 moveTo;
        public MoveToPlaceTask(Vec3 moveTo) : base() { this.moveTo = moveTo; }

        public override async Task<bool> Execute(CancellationToken cancellationToken)
        {
            WorldPosition position = new WorldPosition(Mission.Current.Scene, moveTo);
            Tree.Navigator.SetTargetFrame(position, Tree.Agent.Frame.rotation.f.AsVec2.RotationInRadians);
            return true;
        }
    }
    public class ClearMovementTask : BTTask<MovementTree>
    {
        public ClearMovementTask() : base() { }

        public override async Task<bool> Execute(CancellationToken cancellationToken)
        {
            Tree.Navigator.ClearTarget();
            return true;
        }
    }
}
