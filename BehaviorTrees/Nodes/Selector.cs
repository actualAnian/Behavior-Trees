using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BehaviorTrees.Nodes
{
    public class Selector : BTControlNode
    {
        public Selector(BehaviorTree tree, List<BTNode>? children = null, AbstractDecorator? decorator = null, int weight = 100) : base(tree, decorator, children, weight) { }
        protected override async Task<bool> ExecuteImplementation(CancellationToken cancellationToken)
        {
            foreach (BTNode child in allChildren)
            {
                if (child.Decorator == null) currentlyExecutableChildren.Add(child);
                else
                {
                    if (child.Decorator.Evaluate()) currentlyExecutableChildren.Add(child);
                    else if (child.Decorator.OnDecoratorFalse == OnDecoratorFalse.AwaitEvent) childrenWithTasks.Add(child);
                }
            }
            bool shouldStop = false;
            foreach (var child in currentlyExecutableChildren)
            {
                shouldStop = await child.Execute(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                if (shouldStop) return true;
                alreadyExecutedNodes += 1;
            }
            if (childrenWithTasks.Count == 0) return false;
            AddTasks(cancellationToken);
            while (!ExecutedAll())
            {
                currentlyExecutableChildren = new();
                if (childrenWithTasks.Count > 0)
                {
                    List<AbstractDecorator> decoratorsList = new(decorators);
                    List<Task<bool>> tasksList = new(tasks)
                    {
                        GetCancellationTask(cancellationToken)
                    };
                    Task<bool> completedTask = await Task.WhenAny(tasksList);
                    cancellationToken.ThrowIfCancellationRequested();
                    int completedIndex = tasks.IndexOf(completedTask);
                    if (!completedTask.Result) return false;
                    AbstractDecorator decoratorCalled = decoratorsList[completedIndex];
                    BTNode child = decoratorCalled.NodeBeingDecoracted;
                    if (decoratorCalled.Evaluate())
                    {
                        shouldStop = await child.Execute(cancellationToken);
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                    else
                    {
                        lock (tasks)
                        {
                            tasks.RemoveAt(completedIndex);
                            RemoveDecorator(decoratorCalled);
                            decorators.RemoveAt(completedIndex);
                            tasks.Add(child.AddDecoratorsListeners(cancellationToken));
                            decorators.Add(child.Decorator);
                        }
                    }
                    if (shouldStop) return true;
                }
            }
            return false;
        }
    }
}
