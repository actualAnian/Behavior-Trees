using System.Collections.Generic;

namespace BehaviorTrees.Nodes
{
    public class Sequence : BTControlNode
    {
        public Sequence(BehaviorTree tree, string name, List<BTNode>? children = null, AbstractDecorator? decorator = null, int weight = 100) : base(tree, name, decorator, children, weight) { }

        private void Prepare()
        {
            alreadyExecutedNodes = 0;
            IsWaitingASingleTime = false;
        }
        public override BTNode HandleExecute()
        {
            switch (Status)
            {
                case BTStatus.NotExecuted:
                    Prepare();
                    Status = BTStatus.Running;
                    return this;

                case BTStatus.WaitingForEvent:
                    return this;

                case BTStatus.ReceivedEvent:
                    if (BaseTree.NodeReceivingEvent!.Decorator!.Evaluate())
                    {
                        Status = BTStatus.Running;
                        RemoveDecorator((BaseTree.NodeReceivingEvent!.Decorator! as BTEventDecorator)!);
                        return BaseTree.NodeReceivingEvent;
                    }
                    else
                    {
                        BaseTree.NodeReceivingEvent = null;
                        Status = BTStatus.WaitingForEvent;
                        return this;
                    }
                case BTStatus.Running:
                    return HandleChild(allChildren[alreadyExecutedNodes]);

                default: // fallback, should never happen
                    BTRegister.Logger?.LogMessage($"Error, the Sequence {Name} is in a {Status} state, this should never happen!");
                    ResetChildren();
                    return Parent;
            }
        }
        private BTNode? HandleDecorators(BTNode currentChild)
        {
            if (currentChild.Decorator is { } decorator && !decorator.Evaluate())
            {
                if (decorator is BTEventDecorator)
                {
                    AddDecoratorsListeners();
                    Status = BTStatus.WaitingForEvent;
                    return this;
                }
                else
                {
                    Status = BTStatus.FinishedWithFalse;
                    ResetChildren();
                    return Parent;
                }
            }
            return null;
        }
        private BTNode HandleChild(BTNode currentChild)
        {
            switch (currentChild.Status)
            {
                case BTStatus.NotExecuted:
                    BTNode? nextNode = HandleDecorators(currentChild);
                    if (nextNode == null)
                    {
                        hasRunChild = true;
                        Status = BTStatus.Running;
                        return currentChild;
                    }
                    else return nextNode;

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
                        return currentChild;
                    }

                case BTStatus.FinishedWithTrue:
                    IsWaitingASingleTime = false;
                    alreadyExecutedNodes++;
                    if (alreadyExecutedNodes >= allChildren.Count)
                    {
                        Status = BTStatus.FinishedWithTrue;
                        ResetChildren();
                        return Parent;
                    }
                    return this;

                case BTStatus.FinishedWithFalse:
                    IsWaitingASingleTime = false;
                    Status = BTStatus.FinishedWithFalse;
                    ResetChildren();
                    return Parent;

                case BTStatus.WaitingForEvent:
                    if (currentChild.Decorator!.Evaluate())
                    {
                        Status = BTStatus.Running;
                        return currentChild;
                    }
                    else
                        return this;
                default:
                    ResetChildren();
                    return Parent;
            }
        }
    }
}
