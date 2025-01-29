using BehaviorTrees;
using SandBox;

namespace BehaviorTreeWrapper.BlackBoardClasses
{
    public interface IBTMovable : IBTBlackboard
    {
        public BTBlackboardValue<AgentNavigator> Navigator { get; set; }
    }

}
