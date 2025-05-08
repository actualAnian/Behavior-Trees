//using BehaviorTrees;
//using BehaviorTrees.Nodes;
//using BehaviorTreeWrapper.AbstractDecoratorsListeners;
//using BehaviorTreeWrapper.BlackBoardClasses;
//using BehaviorTreeWrapper.Decorators;
//using BehaviorTreeWrapper.Tasks;
//using SandBox;
//using System.Threading;
//using System.Threading.Tasks;
//using TaleWorlds.Library;
//using TaleWorlds.MountAndBlade;

//namespace BehaviorTreeWrapper.Tests
//{
//    public interface IBTEnemyAttackTree : IBTBlackboard
//    {
//        public BTBlackboardValue<bool> ShouldBeAttacking { get; set; }
//    }
//    public class ShouldBeAttackingDecorator : BannerlordEventDecorator, IBTEnemyAttackTree
//    {
//        BTBlackboardValue<bool> shouldBeAttacking;

//        public ShouldBeAttackingDecorator(SubscriptionPossibilities subscribesTo) : base(subscribesTo)
//        {
//        }

//        public BTBlackboardValue<bool> ShouldBeAttacking { get => shouldBeAttacking; set => shouldBeAttacking = value; }

//        public override bool Evaluate()
//        {
//            return ShouldBeAttacking.GetValue();
//        }

//        public override void Notify(object[] data) { }
//    }
//    public class SetShouldBeAttackingTask : BTTask, IBTEnemyAttackTree
//    {
//        BTBlackboardValue<bool> shouldBeAttacking;
//        public BTBlackboardValue<bool> ShouldBeAttacking { get => shouldBeAttacking; set => shouldBeAttacking = value; }

//        public override BTNode Execute()
//        {
//            shouldBeAttacking.SetValue(true);
//            return Parent;
//        }
//    }

//    public class RatTestTree : BehaviorTree, IBTBannerlordBase, IBTMovable, IBTEnemyAttackTree
//    {
//        public BTBlackboardValue<AgentNavigator> Navigator { get; set; }
//        public BTBlackboardValue<Agent> Agent { get; set; }
//        public BTBlackboardValue<bool> ShouldBeAttacking { get; set; }

//        public RatTestTree(Agent agent) : base(2000)
//        {
//            Navigator = new BTBlackboardValue<AgentNavigator>(agent.GetComponent<CampaignAgentComponent>().CreateAgentNavigator());
//            Agent = new BTBlackboardValue<Agent>(agent);
//            ShouldBeAttacking = new(false);
//        }
//        public static new BehaviorTree? BuildTree(object[] objects)
//        {
//            if (objects[0] is not Agent agent) return null;

//            string[] plagueRatAttack = new string[]
//            {
//                "act_rat_attack_1",
//                "act_rat_attack_2",
//                "act_rat_attack_3",
//                "act_rat_attack_4",
//                "act_rat_attack_6",
//            };
//            RatTestTree? tree = StartBuildingTree(new RatTestTree(agent))
//                //.AddSelector("main")
//                .AddRandomSelector("choose_attack", new ShouldBeAttackingDecorator(SubscriptionPossibilities.OnAgentDeleted))
//                .AddSubTree("PerformAnAttackTree", 100, agent, plagueRatAttack[0], 2000)
//                    //.AddSequence("attack_1")
//                    //.AddTask(new PlayAnimationTask(plagueRatAttack[0]))
//                    //.AddTask(new SleepTask(2000))
//                    //.Up()
//                    .AddSequence("attack_2")
//                        .AddTask(new PlayAnimationTask(plagueRatAttack[1]))
//                        .AddTask(new SleepTask(2000))
//                        .Up()
//                    .AddSequence("attack_3")
//                        .AddTask(new PlayAnimationTask(plagueRatAttack[2]))
//                        .AddSequence("false", new AlwaysFalseDecorator())
//                            .AddTask(new SleepTask(20))
//                            .Up()
//                        .AddTask(new SleepTask(2000))
//                        .Up()
//                    .AddSequence("attack_4")
//                        .AddTask(new PlayAnimationTask(plagueRatAttack[3]))
//                        .AddTask(new SleepTask(2000))
//                        .Up()
//                    .AddSequence("attack_6")
//                        .AddTask(new PlayAnimationTask(plagueRatAttack[4]))
//                        .AddTask(new SleepTask(2000))
//                        .Up()
//                    .Up()
//                .AddSequence("attacking", new WaitNSecondsTickDecorator(10))
//                    .AddTask(new SetShouldBeAttackingTask())
//                    .AddTask(new PrintBottomLeftMessageTask("Attacking!"))
//                    .Up()
//                .Finish();
//            return tree;
//        }
//    }
//}
