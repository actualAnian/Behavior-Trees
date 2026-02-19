using BehaviorTrees;
using BehaviorTrees.Nodes;
using BehaviorTreeWrapper.BlackBoardClasses;
using TaleWorlds.MountAndBlade;

namespace BehaviorTreeWrapper.Tasks
{
    public class PlayAnimationTask : BTTask
    {
        readonly string _actionId;
        readonly BTBlackboardBannerlordBase _bbBase;
        public PlayAnimationTask(string actionId, BTBlackboardBannerlordBase bbBase)
        {
            _actionId = actionId;
            _bbBase = bbBase;
        }
        public override BTTaskStatus Execute()
        {
            _bbBase.Agent.SetActionChannel(0, ActionIndexCache.Create(_actionId), true);
            return BTTaskStatus.FinishedWithTrue;
        }
    }
}
