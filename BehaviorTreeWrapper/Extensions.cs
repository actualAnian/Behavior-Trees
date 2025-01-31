using TaleWorlds.MountAndBlade;
using BehaviorTrees;
using TaleWorlds.Library;
using System.Linq;

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
