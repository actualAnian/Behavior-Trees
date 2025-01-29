using System.Collections.Generic;
using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem.AgentOrigins;
using BehaviorTrees;
using BehaviorTreeWrapper.Trees;

namespace BehaviorTreeWrapper
{
    public static class Extensions
    {
        public static BehaviorTree? AddBehaviorTree(this Agent agent, string treeName)
        {
            //if (agent.Origin == null || agent.Origin is PartyAgentOrigin) return null;
            if (BehaviorTreeBannerlordWrapper.Instance.CurrentMissionLogic.trees.TryGetValue(agent, out var descriptors))
            {
                throw new Exception("BehaviorTreeWrapper.Extensions.AddBehaviorTree error, key already exists");
            }
            BehaviorTree? tree = BTRegister.Build(treeName, agent);
            if (tree != null)
            {
                BehaviorTreeBannerlordWrapper.Instance.CurrentMissionLogic.trees[agent] = tree;
            }
            return tree;
        }
        public static BehaviorTree? GetBehaviorTree(this Agent agent)
        {
            if (agent.Origin == null) return null;
            if (!BehaviorTreeBannerlordWrapper.Instance.CurrentMissionLogic.trees.ContainsKey(agent)) return null;
            return BehaviorTreeBannerlordWrapper.Instance.CurrentMissionLogic.trees[agent];
        }
    }
}
