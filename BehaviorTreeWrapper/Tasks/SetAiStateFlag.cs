using BehaviorTrees.Nodes;
using BehaviorTrees;
using BehaviorTreeWrapper.BlackBoardClasses;
using SandBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using static TaleWorlds.MountAndBlade.Agent;

namespace BehaviorTreeWrapper.Tasks
{
    public class SetAiStateFlag : BTTask, IBTBannerlordBase
    {
        BTBlackboardValue<Agent> _agent;
        public BTBlackboardValue<Agent> Agent { get => _agent; set => _agent = value; }
        AIStateFlag flag;
        public SetAiStateFlag(AIStateFlag flag) : base() { this.flag = flag; }

        public override BTTaskStatus Execute()
        {
            Agent.GetValue().AIStateFlags = flag;
            return BTTaskStatus.FinishedWithTrue;
        }
    }
}
