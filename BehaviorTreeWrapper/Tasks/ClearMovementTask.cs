﻿using System.Threading.Tasks;
using SandBox;
using System.Threading;
using BehaviorTrees.Nodes;
using BehaviorTrees;
using BehaviorTreeWrapper.BlackBoardClasses;

namespace BehaviorTreeWrapper.Tasks
{
    public class ClearMovementTask : BTTask, IBTMovable
    {
        BTBlackboardValue<AgentNavigator> _navigator;
        public BTBlackboardValue<AgentNavigator> Navigator { get => _navigator; set => _navigator = value; }
        public ClearMovementTask() : base() { }

        public override async Task<bool> Execute(CancellationToken cancellationToken)
        {
            Navigator.GetValue().ClearTarget();
            return true;
        }
    }
}
