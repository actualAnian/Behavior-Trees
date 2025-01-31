using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BehaviorTrees.Nodes
{
    // Ignores decorators that await an event
    internal class RandomSelector : BTControlNode
    {
        private readonly Random random = new();
        public RandomSelector(BehaviorTree tree, AbstractDecorator? decorator = null, List<BTNode>? children = null, int weight = 100) : base(tree, decorator, children, weight)
        {
        }
        protected override async Task<bool> ExecuteImplementation(CancellationToken cancellationToken)
        {
            int totalWeight = 0;
            List<BTNode> validChildren = new();
            foreach (BTNode child in allChildren)
            {
                if (child.Decorator?.Evaluate() == false) continue;
                totalWeight += child.weight;
                validChildren.Add(child);
            }
            int randomNumber = random.Next(totalWeight);
            foreach (BTNode child in validChildren)
            {
                randomNumber -= child.weight;
                if (randomNumber < 0)
                {
                    return await child.Execute(cancellationToken);
                }
            }
            return false; // fallback (shouldn't reach here)
        }
    }
}
