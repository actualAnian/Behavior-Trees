using TaleWorlds.MountAndBlade;
using BehaviorTrees;

namespace BehaviorTreeWrapper
{
    public static class Extensions
    {
        public static BehaviorTree? GetBehaviorTree(this Agent agent)
        {
            if (!BehaviorTreeBannerlordWrapper.Instance.CurrentMissionLogic.trees.ContainsKey(agent)) return null;
            return BehaviorTreeBannerlordWrapper.Instance.CurrentMissionLogic.trees[agent];
        }
    }
}
