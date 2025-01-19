using BehaviorTrees;
using SandBox;

namespace BehaviorTreeWrapper.BlackBoardClasses
{
    public interface IMovable : IBTBlackboard
    {
        public BTBlackboardValue<AgentNavigator> Navigator { get; set; }
    }

}
