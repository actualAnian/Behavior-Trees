using BehaviorTrees;
using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BehaviorTreeWrapper
{
    public class BehaviorTreeAgentComponent : AgentComponent
    {
        BehaviorTree? Tree { get; set; }
        float timeSinceLastEvaluation;
        public BehaviorTreeAgentComponent(Agent agent, string treeName, params object[] args) : base(agent)
        {
            object[] newArgs = new object[args.Length + 1];
            newArgs[0] = agent;
            Array.Copy(args, 0, newArgs, 1, args.Length);

            args = newArgs;
            Tree = BehaviorTreeBannerlordWrapper.Instance.AddBehaviorTree(treeName, args);
            Random random = new();
            timeSinceLastEvaluation = (float)(random.NextDouble() * (Tree.rootEvaluationDelay / 1000f)); // randomize the first tick to avoid all agents ticking at the same time
        }
        public override void OnAgentRemoved()
        {
            BehaviorTreeBannerlordWrapper.Instance.DisposeTree(Agent);
        }
        public override void OnTickAsAI(float dt)
        {
            if (Tree == null) return;
            timeSinceLastEvaluation += dt;
            if (Tree.rootEvaluationDelay / 1000 < timeSinceLastEvaluation || Tree.ShouldRunNextTick)
            {
                Tree.RunTree();
                timeSinceLastEvaluation = 0f;
            }
        }
    }
}