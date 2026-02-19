using BehaviorTrees.Nodes;
using BehaviorTrees;
using BehaviorTreeWrapper.BlackBoardClasses;
using static TaleWorlds.MountAndBlade.Agent;

namespace BehaviorTreeWrapper.Tasks
{
    public class SetAiStateFlag : BTTask
    {
        readonly BTBlackboardBannerlordBase _bbBase;
        readonly AIStateFlag _flag;
        public SetAiStateFlag(AIStateFlag flag, BTBlackboardBannerlordBase bbBase) : base() 
        {
            _flag = flag; 
            _bbBase = bbBase;
        }
        public override BTTaskStatus Execute()
        {
            _bbBase.Agent.AIStateFlags = _flag;
            return BTTaskStatus.FinishedWithTrue;
        }
    }
}
