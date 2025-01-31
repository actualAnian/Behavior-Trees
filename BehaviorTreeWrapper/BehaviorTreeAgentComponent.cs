using BehaviorTrees;
using System;
using TaleWorlds.MountAndBlade;

namespace BehaviorTreeWrapper
{
    public class BehaviorTreeAgentComponent : AgentComponent
    {
        BehaviorTree? Tree { get; set; }
        public BehaviorTreeAgentComponent(Agent agent, string treeName, params object[] args) : base(agent)
        {
            object[] newArgs = new object[args.Length + 1];
            newArgs[0] = agent;
            Array.Copy(args, 0, newArgs, 1, args.Length);

            args = newArgs;
            Tree = BehaviorTreeBannerlordWrapper.Instance.AddBehaviorTree(treeName, args);
            Tree?.StartTree();
        }
        public void RemoveTree()
        {
            //@TODO
        }
        public override void OnAgentRemoved()
        {
            BehaviorTreeBannerlordWrapper.Instance.DisposeTree(Agent);
        }
    }
}