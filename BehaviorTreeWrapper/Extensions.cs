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
        public static void AddBehaviorTree(this Agent agent)
        {
            if (agent.Origin == null || agent.Origin is PartyAgentOrigin) return;
            if (BehaviorTreeBannerlordWrapper.Instance.CurrentMissionLogic.trees.TryGetValue(agent.Origin.Seed, out var descriptors))
            {
                throw new Exception("BehaviorTreeWrapper.Extensions.AddBehaviorTree error, key already exists");
            }
            BehaviorTree? tree = BTRegister.Build("ExampleTree", agent);
            if (tree != null)
            {
                BehaviorTreeBannerlordWrapper.Instance.CurrentMissionLogic.trees[agent.Origin.Seed] = tree;
                tree.StartTree();
            }
        }
        public static BehaviorTree? GetBehaviorTree(this Agent agent)
        {
            if (agent.Origin == null) return null;
            if (!BehaviorTreeBannerlordWrapper.Instance.CurrentMissionLogic.trees.ContainsKey(agent.Origin.Seed)) return null;
            return BehaviorTreeBannerlordWrapper.Instance.CurrentMissionLogic.trees[agent.Origin.Seed];
        }
    }
}
