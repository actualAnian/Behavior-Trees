using BehaviorTrees;
using SandBox;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using BehaviorTreeWrapper.Decorators;
using BehaviorTreeWrapper.Tasks;
using BehaviorTreeWrapper.BlackBoardClasses;

namespace BehaviorTreeWrapper.Tests
{
    public class ExampleTree : BehaviorTree
    {
        readonly BTBlackboardBannerlordBase _bbBase;
        readonly BTBlackboardMovable _bbMovable;
        public ExampleTree(BTBlackboardBannerlordBase bbBase, BTBlackboardMovable bbMovable) : base(2000)
        {
            _bbBase = bbBase;
            _bbMovable = bbMovable;
        }
        public static new BehaviorTree? BuildTree(object[] objects)
        {
            if (objects[0] is not Agent agent) return null;
            
            Vec3 position = new(373, 295, 20);
            BTBlackboardBannerlordBase bbBase = new(agent);
            BTBlackboardMovable bbMovable = new(agent);

            ExampleTree? tree = StartBuildingTree(new ExampleTree(bbBase, bbMovable))
                .AddSelector("main")
                    .AddSequence("spotted", new AlarmedDecorator(SubscriptionPossibilities.OnSelfAlarmedStateChanged, bbBase))
                        .AddTask(new PrintQuickMessageTask("I am spotted"))
                        .Up()
                    .AddSequence("hit", new HitDecorator(SubscriptionPossibilities.OnSelfIsHit))
                        .AddTask(new MoveToPlaceTask(position, bbBase, bbMovable))
                        .AddTask(new PrintQuickMessageTask("I am hit"))
                        .Up()
                    .AddSequence("moved", new InPositionDecorator(position, bbBase))
                        .AddTask(new ClearMovementTask(bbMovable))
                        .Up()
                    .AddSequence("moved", new WaitNSecondsTickDecorator(5))
                        .AddTask(new PrintQuickMessageTask("5 seconds passed"))
                        .Up()
                .Finish();
            return tree;
        }
    }
}