using BehaviorTrees.Nodes;
using BehaviorTrees;
using BehaviorTreeWrapper.BlackBoardClasses;

namespace BehaviorTreeWrapper.Tasks
{
    public class ClearMovementTask : BTTask
    {
        readonly BTBlackboardMovable _bbMovable;
        public ClearMovementTask(BTBlackboardMovable bbMovable) : base() { _bbMovable = bbMovable; }

        public override BTTaskStatus Execute()
        {
            _bbMovable.Navigator.ClearTarget();
            return BTTaskStatus.FinishedWithTrue;
        }
    }
}
