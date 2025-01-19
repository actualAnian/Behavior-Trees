﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BehaviorTrees.Nodes
{
    public class Sequence : BTControlNode
    {
        public Sequence(BehaviorTree tree, List<BTNode>? children = null, AbstractDecorator? decorator = null) : base(tree, decorator, children) { }
        protected override async Task<bool> ExecuteImplementation(CancellationToken cancellationToken)
        {
            bool shouldContinue = true;
            foreach (BTNode child in allChildren)
            {
                if (child.Decorator == null)
                {
                    shouldContinue = await child.Execute(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    if (!shouldContinue) return false;
                }
                else
                {
                    bool evaluate = child.Decorator.Evaluate();
                    if (!evaluate && child.Decorator.OnDecoratorFalse == OnDecoratorFalse.ReturnFalse) return false;
                    while (!evaluate && child.Decorator.OnDecoratorFalse == OnDecoratorFalse.AwaitEvent)
                    {
                        await child.AddDecoratorsListeners(cancellationToken);
                        evaluate = child.Decorator.Evaluate();
                    }

                    shouldContinue = await child.Execute(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    if (!shouldContinue) return false;
                }
            }
            return true;
        }
    }
}
