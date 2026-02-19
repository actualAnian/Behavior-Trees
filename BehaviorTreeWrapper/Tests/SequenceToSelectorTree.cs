using BehaviorTrees;
using SandBox;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using BehaviorTreeWrapper.Decorators;
using BehaviorTreeWrapper.Tasks;
using BehaviorTreeWrapper.BlackBoardClasses;

namespace BehaviorTreeWrapper.Tests
{
    public class SequenceToSelectorTree : BehaviorTree
    {
        readonly BTBlackboardBannerlordBase _bbBase;
        readonly BTBlackboardMovable _bbMovable;

        public SequenceToSelectorTree(BTBlackboardBannerlordBase bbBase, BTBlackboardMovable bbMovable) : base(2000)
        {
            _bbBase = bbBase;
            _bbMovable = bbMovable;
        }
        public static new BehaviorTree? BuildTree(object[] objects)
        {
            if (objects[0] is not Agent agent) return null;

            var bbBase = new BTBlackboardBannerlordBase(agent);
            var bbMovable = new BTBlackboardMovable(agent);
            Vec3 position = new(373, 295, 20);
            SequenceToSelectorTree? tree = StartBuildingTree(new SequenceToSelectorTree(bbBase, bbMovable))
                .AddSequence("main")
                    .AddSelector("main", new HealthBelowPercentageDecorator(99, bbBase))
                        .AddSequence("spotted", new AlarmedDecorator(SubscriptionPossibilities.OnSelfAlarmedStateChanged, bbBase))
                            .AddTask(new PrintQuickMessageTask("I am spotted"))
                            .AddTask(new SleepTask(new System.TimeSpan(0, 0, 4)))
                            .AddTask(new PrintQuickMessageTask("the sleep finished!"))
                            .Up()
                        .AddSequence("hit", new HitDecorator(SubscriptionPossibilities.OnSelfIsHit))
                            .AddTask(new MoveToPlaceTask(position, bbBase, bbMovable))
                            .AddTask(new PrintQuickMessageTask("I am hit"))
                            .Up()
                        .AddSequence("moved", new InPositionDecorator(position, bbBase))
                            .AddTask(new ClearMovementTask(bbMovable))
                            .Up()
                        .Up()
                    .AddTask(new PrintQuickMessageTask("End of the tree!"))
                    .Finish();
            return tree;
        }
    }
}