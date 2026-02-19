using BehaviorTrees;
using TaleWorlds.MountAndBlade;

namespace BehaviorTreeWrapper.BlackBoardClasses
{
    public class BTBlackboardBannerlordBase : NewIBTBlackboard
    {
        public Agent Agent { get; set; }

        public BTBlackboardBannerlordBase(Agent agent)
        {
            Agent = agent;
        }
    }

}
