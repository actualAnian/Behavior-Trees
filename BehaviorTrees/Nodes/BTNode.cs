﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BehaviorTrees.Nodes
{
    public abstract class BTNode
    {
        protected BehaviorTree BaseTree { get; set; }
        public int weight;
        public virtual AbstractDecorator? Decorator
        {
            get
            {
                return null;
            }
        }
        protected BTNode(int weight = 100) { this.weight = weight; }
        public virtual async Task<bool> Execute(CancellationToken cancellationToken) { return true; }
        public Task<bool> AddDecoratorsListeners(CancellationToken cancellationToken)
        {
            return ((BTEventDecorator)Decorator).AddListener(cancellationToken);
        }
    }
}
