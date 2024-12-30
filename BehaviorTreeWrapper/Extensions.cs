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
            if (BehaviorTreeBannerlordWrapper.Instance.CurrentMission.trees.TryGetValue(agent.Origin.Seed, out var descriptors))
            {
                throw new Exception("BehaviorTreeWrapper.Extensions.AddBehaviorTree error, key already exists");
            }
            NO++;
            BehaviorTreeBannerlordWrapper.Instance.CurrentMission.trees[agent.Origin.Seed] = BasicTree.BuildBasicTree(agent);
//            BehaviorTreeMissionLogic.Instance.trees[agent] = new BasicTree(agent);
        }

        public static BasicTree? GetBehaviorTree(this Agent agent)
        {
            if (agent.Origin == null) return null;
            if (!BehaviorTreeBannerlordWrapper.Instance.CurrentMission.trees.ContainsKey(agent.Origin.Seed)) return null;
            return BehaviorTreeBannerlordWrapper.Instance.CurrentMission.trees[agent.Origin.Seed];
        }
    }
}
