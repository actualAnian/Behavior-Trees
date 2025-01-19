using BehaviorTrees;
using TaleWorlds.MountAndBlade;

namespace BehaviorTreeWrapper.BlackBoardClasses
{
    public interface IBannerlordBase : IBTBlackboard
    {
        public BTBlackboardValue<Agent> Agent { get; set; }
    }

}
