using BehaviorTrees;
using BehaviorTrees.Nodes;
using BehaviorTreeWrapper.BlackBoardClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BehaviorTreeWrapper.Tasks
{
    public class PlayAnimationTask : BTTask, IBTBannerlordBase
    {
        string actionId;
        public PlayAnimationTask(string actionId)
        {
            this.actionId = actionId;
        }
        BTBlackboardValue<Agent> agent;
        public BTBlackboardValue<Agent> Agent { get => agent; set => agent = value; }

        public override async Task<bool> Execute(CancellationToken cancellationToken)
        {
            Agent.GetValue().SetActionChannel(0, ActionIndexCache.Create(actionId), true);
            return true;
        }
    }
}
