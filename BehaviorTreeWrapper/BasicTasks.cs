using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BehaviorTreeWrapper
{
    public class PrintTask : BehaviorTree.BTTask
    {
        public PrintTask(BehaviorTree.BehaviorTree tree) : base(tree) { }

        public override async Task<bool> Execute()
        {
            MBInformationManager.AddQuickInformation(new TextObject("I spotted you!", null), 0, null, "");
            return true;
        }

        public override void RemoveDecorators() { }

        public override void UpdateTreeProperty() { }
    }
    public class PrintTask2 : BehaviorTree.BTTask
    {
        public PrintTask2(BehaviorTree.BehaviorTree tree) : base(tree) { }

        public override async Task<bool> Execute()
        {
            MBInformationManager.AddQuickInformation(new TextObject("I am hit!", null), 0, null, "");
            return true;
        }

        public override void RemoveDecorators() { }

        public override void UpdateTreeProperty() { }
    }
}
