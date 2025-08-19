using System;
using System.Collections.Generic;

namespace BehaviorTrees.Nodes
{
    // Ignores decorators that await an event
    internal class RandomSelector : BTControlNode
    {
        private readonly Random random = new();
        private BTNode? lastChild;
        public RandomSelector(BehaviorTree tree, string name, AbstractDecorator? decorator = null, List<BTNode>? children = null, int weight = 100) : base(tree, name, decorator, children, weight)
        {
        }
        public override BTNode HandleExecute()
        {
            if (lastChild != null)
            {
                switch (lastChild.Status)
                {
                    case BTStatus.FinishedWithTrue:
                        lastChild = null;
                        IsWaitingASingleTime = false;
                        Status = BTStatus.FinishedWithTrue;
                        return Parent;
                    case BTStatus.FinishedWithFalse:
                        lastChild = null;
                        IsWaitingASingleTime = false;
                        Status = BTStatus.FinishedWithFalse;
                        return Parent;
                    case BTStatus.Running:
                        Status = BTStatus.Running;
                        if (hasRunChild)
                        {
                            hasRunChild = false;
                            IsWaitingASingleTime = true;
                            return this;
                        }
                        else
                        {
                            hasRunChild = true;
                            return lastChild;
                        }
                    default: // fallback
                        Status = BTStatus.Running;
                        return this;
                }

            }
            else
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
                        hasRunChild = true;
                        lastChild = child;
                        return child;
                    }
                }
                return Parent; // fallback (shouldn't reach here)
            }

        }
    }
}
