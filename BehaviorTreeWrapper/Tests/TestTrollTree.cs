//using System.Threading.Tasks;
//using Alliance.Common.Extensions.AdvancedCombat.AgentBehaviors;
//using Alliance.Common.Extensions.AdvancedCombat.AgentComponents;
//using Alliance.Common.Extensions.AdvancedCombat.BTBlackBoards;
//using Alliance.Common.Extensions.AdvancedCombat.BTTasks;
//using Alliance.Common.Extensions.AdvancedCombat.Models;
//using BehaviorTrees;
//using BehaviorTrees.Nodes;
//using BehaviorTreeWrapper.BlackBoardClasses;
//using BehaviorTreeWrapper.Decorators;
//using global::Alliance.Common.Utilities;
//using System.Threading;
//using TaleWorlds.Engine;
//using TaleWorlds.MountAndBlade;
//using static Alliance.Common.Utilities.Logger;
//using Alliance.Common.Core.Utils;
//using Alliance.Common.Extensions.FormationEnforcer.Component;
//using System.Collections.Generic;
//namespace BehaviorTreeWrapper.Tests
//{

//    namespace Alliance.Common.Extensions.AdvancedCombat.BTBehaviorTrees
//    {
//        public enum BTState
//        {
//            Idle,
//            LookForTarget,
//            Chase,
//            MeleeFight,
//            RangedFight,
//            Flee
//        }
//        public interface IBTStateBlackboard : IBTBlackboard
//        {
//            public BTBlackboardValue<BTState> State { get; set; }
//        }
//        public class StateDecorator : AbstractDecorator, IBTStateBlackboard
//        {
//            private readonly BTState targetState;

//            public StateDecorator(BTState targetState) : base()
//            {
//                this.targetState = targetState;
//            }

//            BTBlackboardValue<BTState> state;
//            public BTBlackboardValue<BTState> State { get => state; set => state = value; }

//            public override bool Evaluate()
//            {
//                return targetState == State.GetValue();
//            }
//        }

//        public class SetStateTask : BTTask, IBTStateBlackboard
//        {
//            private readonly BTState targetState;

//            BTBlackboardValue<BTState> state;
//            public BTBlackboardValue<BTState> State { get => state; set => state = value; }

//            public SetStateTask(BTState targetState) : base()
//            {
//                this.targetState = targetState;
//            }

//            public override async Task<bool> Execute(CancellationToken cancellationToken)
//            {
//                await Task.CompletedTask;
//                State.SetValue(targetState);
//                return true;
//            }
//        }

//        public class IsTargetCloseTask : BTTask, IBTCombatBlackboard
//        {
//            private readonly float maxRange;

//            public BTBlackboardValue<Agent> Target { get; set; }
//            public BTBlackboardValue<Agent> Agent { get; set; }

//            public IsTargetCloseTask(float maxRange) : base()
//            {
//                this.maxRange = maxRange;
//            }

//            public override async Task<bool> Execute(CancellationToken cancellationToken)
//            {
//                Agent self = Agent.GetValue();
//                Agent targetAgent = Target.GetValue();

//                if (self == null || targetAgent == null || targetAgent.Health <= 0 || targetAgent.IsFadingOut()
//                    || self.Position.Distance(targetAgent.Position) > maxRange)
//                {
//                    Target.SetValue(null);
//                    return false;
//                }
//                return true;
//            }
//        }

//        public class TestTrollTree : BehaviorTree, IBTCombatBlackboard, IBTStateBlackboard
//        {
//            //public BTBlackboardValue<AL_AgentNavigator> Navigator { get; set; }
//            public BTBlackboardValue<Agent> Agent { get; set; }
//            public BTBlackboardValue<Agent> Target { get; set; }
//            public BTBlackboardValue<BTState> State { get; set; }

//            public TestTrollTree(Agent agent) : base()
//            {
//                //Navigator = new BTBlackboardValue<AL_AgentNavigator>(agent.GetComponent<DefaultAgentComponent>().CreateAgentNavigator());
//                Agent = new BTBlackboardValue<Agent>(agent);
//                Target = new BTBlackboardValue<Agent>(null);
//                State = new BTBlackboardValue<BTState>(BTState.Idle);
//            }

//            public static new BehaviorTree? BuildTree(object[] objects)
//            {
//                if (objects[0] is not Agent agent) return null;

//                BehaviorTree? tree = StartBuildingTree(new TestTrollTree(agent))
//                    .AddSelector("main")
//                        .AddSequence("Idle", new StateDecorator(BTState.Idle))
//                            .AddSequence("UnderAttack", new HitDecorator(SubscriptionPossibilities.OnSelfIsHit))
//                                .AddTask(new LogTask("I am hit", LogLevel.Debug))
//                                .AddTask(new SetStateTask(BTState.LookForTarget))
//                                .Up()
//                            .AddSequence("Spotted", new AlarmedDecorator(SubscriptionPossibilities.OnSelfAlarmedStateChanged))
//                                .AddTask(new LogTask("I am spotted", LogLevel.Debug))
//                                .AddTask(new SetStateTask(BTState.LookForTarget))
//                                .Up()
//                            .AddSequence("IdleSeq", new StateDecorator(BTState.Idle))
//                                .AddTask(new LogTask("I am idle", LogLevel.Debug))
//                                .AddTask(new AnimationTask(TrollConstants.IdleAnimations))
//                                .Up()
//                            .Up()
//                        .AddSequence("LookForTarget", new StateDecorator(BTState.LookForTarget))
//                            .AddSequence("LookForTargetSeq")
//                                .AddTask(new LogTask("I am looking for target", LogLevel.Debug))
//                                .AddTask(new MyLookForTargetTask(20))
//                                .AddTask(new LogTask("I found a target", LogLevel.Debug))
//                                .AddTask(new SetStateTask(BTState.Chase))
//                                .Up()
//                            .Up()
//                        .AddSequence("Chase", new StateDecorator(BTState.Chase))
//                            .AddSelector("ChaseOrNot")
//                                .AddSequence("TryChase")
//                                    .AddTask(new IsTargetCloseTask(20))
//                                    .AddTask(new AnimationTask(TrollConstants.RageAnimations))
//                                    .AddTask(new LogTask("I am chasing", LogLevel.Debug))
//                                    .AddTask(new AttackTask())
//                                    .Up()
//                                .AddSequence("CancelChase")
//                                    .AddTask(new LogTask("I lost my target", LogLevel.Debug))
//                                    .AddTask(new AnimationTask(TrollConstants.SearchAnimations))
//                                    .AddTask(new SetStateTask(BTState.LookForTarget))
//                                    .Up()
//                                .Up()
//                            .Up()
//                        .Up()
//                    .Finish();
//                return tree;
//            }
//        }
//        public class MyLookForTargetTask : BTTask, IBTCombatBlackboard, IBTBannerlordBase, IBTBlackboard
//        {
//            private readonly float range;

//            private BTBlackboardValue<Agent> agent;

//            private BTBlackboardValue<Agent> target;

//            public BTBlackboardValue<Agent> Agent
//            {
//                get
//                {
//                    return agent;
//                }
//                set
//                {
//                    agent = value;
//                }
//            }

//            public BTBlackboardValue<Agent> Target
//            {
//                get
//                {
//                    return target;
//                }
//                set
//                {
//                    target = value;
//                }
//            }

//            public MyLookForTargetTask(float range)
//                : base((BTListener)null, 100)
//            {
//                this.range = range;
//            }

//            public override async Task<bool> Execute(CancellationToken cancellationToken)
//            {
//                Target.SetValue(GetBestTarget(range));
//                return Target.GetValue() != null;
//            }
//            public static List<Agent> GetNearAliveAgentsInRange(float range, Agent target)
//            {
//                List<Agent> allAgents = Mission.Current.AllAgents;
//                List<Agent> agentsInRange = new List<Agent>();
//                foreach (Agent agent in allAgents)
//                {
//                    bool flag = !agent.IsActive();
//                    if (!flag)
//                    {
//                        bool flag2 = agent == target.MountAgent || agent == target;
//                        if (!flag2)
//                        {
//                            float distance = agent.Position.Distance(target.Position);
//                            bool isMount = agent.IsMount;
//                            if (isMount)
//                            {
//                                distance -= 0.5f;
//                            }
//                            bool flag3 = distance < range;
//                            if (flag3)
//                            {
//                                agentsInRange.Add(agent);
//                            }
//                        }
//                    }
//                }
//                return agentsInRange;
//            }

//            private Agent GetBestTarget(float range)
//            {
//                List<Agent> nearAliveAgentsInRange = GetNearAliveAgentsInRange(range, agent.GetValue());
//                Agent result = null;
//                float num = float.MinValue;
//                foreach (Agent item in nearAliveAgentsInRange)
//                {
//                    if (item != agent.GetValue() && item.IsActive() && (item.Team == null || item.Team.IsEnemyOf(agent.GetValue().Team)))
//                    {
//                        float num2 = 0f;
//                        FormationComponent formationComponent = item.MissionPeer?.GetComponent<FormationComponent>();
//                        if (formationComponent != null && formationComponent.State == FormationState.Rambo)
//                        {
//                            num2 += 40f;
//                        }

//                        if (item.IsMount && item.RiderAgent == null)
//                        {
//                            num2 += 80f;
//                        }

//                        if (item.Health / item.HealthLimit < 0.3f)
//                        {
//                            num2 += 20f;
//                        }

//                        float length = (item.Position - agent.GetValue().Position).Length;
//                        num2 -= length;
//                        if (num2 > num)
//                        {
//                            result = item;
//                            num = num2;
//                        }
//                    }
//                }

//                return result;
//            }
//        }


//        public class AttackTask : BTTask, IBTCombatBlackboard, IBTBannerlordBase, IBTBlackboard
//        {
//            private BTBlackboardValue<Agent> agent;

//            private BTBlackboardValue<Agent> target;

//            public BTBlackboardValue<Agent> Agent
//            {
//                get
//                {
//                    return agent;
//                }
//                set
//                {
//                    agent = value;
//                }
//            }

//            public BTBlackboardValue<Agent> Target
//            {
//                get
//                {
//                    return target;
//                }
//                set
//                {
//                    target = value;
//                }
//            }

//            public AttackTask()
//                : base((BTListener)null, 100)
//            {
//            }

//            public override async Task<bool> Execute(CancellationToken cancellationToken)
//            {
//                Logger.Log("Enter Attack Task!!!", Logger.LogLevel.Error);
//                Agent self = Agent.GetValue();
//                Agent targetAgent = Target.GetValue();
//                if (self == null)
//                {
//                    Logger.Log("ERROR, Agent is null in AttackTask", Logger.LogLevel.Error);
//                    return false;
//                }
//                if (!Target.GetValue().Position.IsValid)
//                {
//                    int a = 5;
//                }
//                if (targetAgent == null || targetAgent.Health <= 0f || targetAgent.IsFadingOut())
//                {
//                    Logger.Log("ERROR, Target invalid in AttackTask - " + targetAgent?.Name, Logger.LogLevel.Error);
//                    Target.SetValue((Agent)null);
//                    self?.DisableScriptedCombatMovement();
//                    self?.DisableScriptedMovement();
//                    return false;
//                }
//                //WorldPosition p = new(0, new TaleWorlds.Library.Vec3(0, 0, 0);
//                WorldPosition pos = self.GetWorldPosition();
//                self.SetScriptedTargetEntityAndPosition(targetAgent.AgentVisuals.GetEntity(), pos, TaleWorlds.MountAndBlade.Agent.AISpecialCombatModeFlags.IgnoreAmmoLimitForRangeCalculation);
//                if (self.HasRangedWeapon())
//                {
//                    self.SetScriptedPosition(ref pos, addHumanLikeDelay: false, TaleWorlds.MountAndBlade.Agent.AIScriptedFrameFlags.RangerCanMoveForClearTarget);
//                }
//                Logger.Log("Leave Attack Task!!!", Logger.LogLevel.Error);

//                return true;
//            }
//        }
//    }
//}
