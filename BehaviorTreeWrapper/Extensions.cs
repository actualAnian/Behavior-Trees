using System.Collections.Generic;
using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem.AgentOrigins;

namespace BehaviorTreeWrapper
{
    public static class Extensions
    {
        static bool treeAdded = false;
        static int NO = 0;
        public static void AddBehaviorTree(this Agent agent)
        {
            if (agent.Origin == null || agent.Origin is PartyAgentOrigin) return;
            if (BehaviorTreeBannerlordWrapper.Instance.CurrentMissionLogic.trees.TryGetValue(agent.Origin.Seed, out var descriptors))
            {
                throw new Exception("BehaviorTreeWrapper.Extensions.AddBehaviorTree error, key already exists");
            }
            NO++;
            BehaviorTreeBannerlordWrapper.Instance.CurrentMissionLogic.trees[agent.Origin.Seed] = MovementTree.BuildBasicTree(agent);
//            BehaviorTreeMissionLogic.Instance.trees[agent] = new BasicTree(agent);
        }

        public static BannerlordTree? GetBehaviorTree(this Agent agent)
        {
            if (agent.Origin == null) return null;
            if (!BehaviorTreeBannerlordWrapper.Instance.CurrentMissionLogic.trees.ContainsKey(agent.Origin.Seed)) return null;
            return BehaviorTreeBannerlordWrapper.Instance.CurrentMissionLogic.trees[agent.Origin.Seed];
        }
    }
}
