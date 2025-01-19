using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using System.Threading;
using BehaviorTrees.Nodes;

namespace BehaviorTreeWrapper.Tasks
{
    public class PrintTask : BTTask
    {
        public PrintTask() { }
        public override async Task<bool> Execute(CancellationToken cancellationToken)
        {
            MBInformationManager.AddQuickInformation(new TextObject("I spotted you!", null), 0, null, "");
            return true;
        }
    }
    public class PrintTask2 : BTTask
    {
        public PrintTask2() : base() { }
        public override async Task<bool> Execute(CancellationToken cancellationToken)
        {
            MBInformationManager.AddQuickInformation(new TextObject("I am hit!", null), 0, null, "");
            return true;
        }
    }
}
