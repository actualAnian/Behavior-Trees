using BehaviorTrees;
using TaleWorlds.MountAndBlade;

namespace BehaviorTreeWrapper.BlackBoardClasses
{
    public interface IBTBannerlordBase : IBTBlackboard
    {
        public BTBlackboardValue<Agent> Agent { get; set; }
    }

}
