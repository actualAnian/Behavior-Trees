using BehaviorTrees;
using TaleWorlds.MountAndBlade;

namespace BehaviorTreeWrapper
{
    public class BehaviorTreeAgentComponent : AgentComponent
    {
        BehaviorTree? Tree { get; set; }
        public BehaviorTreeAgentComponent(Agent agent, string treeName) : base(agent)
        {
            Tree = Agent.AddBehaviorTree(treeName);
            Tree?.StartTree();
        }
        public void RemoveTree()
        {
            //@TODO
        }
    }
}
