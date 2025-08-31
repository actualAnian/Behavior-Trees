using System;
using System.Collections.Generic;
using System.Linq;

namespace BehaviorTrees.Nodes
{
    public class Selector : BTControlNode
    {
        public Selector(BehaviorTree tree, string name, List<BTNode>? children = null, AbstractDecorator? decorator = null, int weight = 100) : base(tree, name, decorator, children, weight) { }
        private BTNode? lastChild;
        protected List<BTNode> childrenWithTasks = new();
        private bool Prepare()
        {
            alreadyExecutedNodes = 0;
            currentlyExecutableChildren = new();
            childrenWithTasks = new();
            foreach (BTNode child in allChildren)
            {
                if (child.Decorator == null) currentlyExecutableChildren.Add(child);
                else
                {
                    if (child.Decorator.Evaluate()) currentlyExecutableChildren.Add(child);
                    else if (child.Decorator is BTEventDecorator) childrenWithTasks.Add(child);
                    else alreadyExecutedNodes += 1;
                }
            }
            IsWaitingASingleTime = false;
            if (currentlyExecutableChildren.Count == 0 && childrenWithTasks.Count == 0)
            {
                Status = BTStatus.FinishedWithFalse;
                return false;
            }
            Status = BTStatus.Running;
            return true;
        }
        public override BTNode HandleExecute()
        {            
            switch (Status)
            {
                case BTStatus.NotExecuted:
                    bool shouldContinue = Prepare();
                    if (shouldContinue) return this;
                    else return Parent;

                case BTStatus.WaitingForEvent:
                    return this;

                case BTStatus.ReceivedEvent:
                    if (BaseTree.NodeReceivingEvent!.Decorator!.Evaluate())
                    {
                        Status = BTStatus.Running;
                        RemoveDecorators();
                        childrenWithTasks.Remove(BaseTree.NodeReceivingEvent);
                        currentlyExecutableChildren.Add(BaseTree.NodeReceivingEvent);
                        lastChild = BaseTree.NodeReceivingEvent;
                        return BaseTree.NodeReceivingEvent;
                    }
                    else
                    {
                        BaseTree.NodeReceivingEvent = null;
                        Status = BTStatus.WaitingForEvent;
                        return this;
                    }
                case BTStatus.Running:
                    if (alreadyExecutedNodes >= allChildren.Count)
                    {
                        Status = BTStatus.FinishedWithFalse;
                        ResetChildren();
                        return Parent;
                    }
                    if (currentlyExecutableChildren.Count == 0)
                    {
                        foreach (BTNode node in childrenWithTasks)
                        {
                            if (node.Decorator!.Evaluate())
                            {
                                lastChild = node;
                                currentlyExecutableChildren.Add(node);
                                Status = BTStatus.Running;
                                return node;
                            }
                            else //node.Decorator must be BTEventDecorator
                            {
                                node.AddDecoratorsListeners();
                                Status = BTStatus.WaitingForEvent;
                            }
                        }
                        return this;
                    }
                    else
                    {
                        if (lastChild != null)
                        {
                            return HandleChild();
                        }
                        else
                        {
                            lastChild = currentlyExecutableChildren.First();
                            hasRunChild = true;
                            return lastChild;
                        }
                    }
                default: // fallback, should never happen
                    BTRegister.Logger?.LogMessage($"Error, the Selector {Name} is in a {Status} state, this should never happen!");
                    ResetChildren();
                    return Parent;
            }
        }

        private BTNode HandleChild()
        {
            switch (lastChild!.Status)
            {
                case BTStatus.FinishedWithTrue:
                    IsWaitingASingleTime = false;
                    Status = BTStatus.FinishedWithTrue;
                    ResetChildren();
                    lastChild = null;
                    return Parent;

                case BTStatus.FinishedWithFalse:
                    IsWaitingASingleTime = false;
                    alreadyExecutedNodes++;
                    currentlyExecutableChildren.Remove(lastChild);
                    lastChild = null;
                    return this;

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

                default:
                    Status = BTStatus.Running;
                    return this;
            }

        }

        internal void RemoveDecorators()
        {
            childrenWithTasks.ForEach(child => { if (child.Decorator is not null and BTEventDecorator decorator) { RemoveDecorator(decorator); } });
        }
    }
}
