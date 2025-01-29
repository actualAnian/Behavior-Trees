using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BehaviorTrees.Nodes
{
    internal class RandomSelector : BTControlNode
    {
        private bool calculatedWeight = false;
        private int totalWeight;
        private int TotalWeight { 
            get
            {
                if (!calculatedWeight) SumWeights();
                return totalWeight; 
            }
        }
        private Random random = new();
        public RandomSelector(BehaviorTree tree, AbstractDecorator? decorator = null, List<BTNode>? children = null, int weight = 100) : base(tree, decorator, children, weight)
        {
        }
        private void SumWeights()
        {
            calculatedWeight = true;
            totalWeight = 0;
            allChildren.ForEach(child => { totalWeight += child.weight; });
        }
        protected override async Task<bool> ExecuteImplementation(CancellationToken cancellationToken)
        {
            int randomNumber = random.Next(TotalWeight);
            foreach (BTNode child in allChildren)
            {
                randomNumber -= child.weight;
                if (randomNumber < 0)
                {
                    return await child.Execute(cancellationToken);
                }
            }
            return false; // Fallback (shouldn't reach here)
        }
    }
}
