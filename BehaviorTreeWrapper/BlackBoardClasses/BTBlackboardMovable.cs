using BehaviorTrees;
using SandBox;
using TaleWorlds.MountAndBlade;

namespace BehaviorTreeWrapper.BlackBoardClasses
{
    public class BTBlackboardMovable : NewIBTBlackboard
    {
        public AgentNavigator Navigator { get; set; }

        public BTBlackboardMovable(Agent agent)
        {
            Navigator = agent.GetComponent<CampaignAgentComponent>().CreateAgentNavigator();
        }
    }
}
