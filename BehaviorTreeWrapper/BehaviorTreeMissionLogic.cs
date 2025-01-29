using TaleWorlds.MountAndBlade;
using System.Collections.Generic;
using TaleWorlds.Core;
using System.Linq;
using BehaviorTrees;
using BehaviorTreeWrapper.Decorators;
using BehaviorTreeWrapper.Trees;
using TaleWorlds.Library;

namespace BehaviorTreeWrapper
{
    public enum SubscriptionPossibilities
    {
        //use BannerlordTickTimedDecorator for OnMissionTick

        OnAgentDeleted,
        OnSelfAlarmedStateChanged,
        OnAgentFleeing,
        OnSelfFleeing,
        OnSelfMount,
        OnAgentMount,
        OnSelfDismount,
        OnAgentDismount,
        OnAgentPanicked,
        OnSelfRemoved,
        OnSelfKilledEnemy,
        OnAgentRemoved,
        OnAgentShootMissile,
        OnSelfGainedFocus,
        OnSelfLostFocus,
        OnSelfIsHit,
        OnSelfHitsEnemy,
        OnSelfUsedObject,
        OnSelfStoppedUsingObject,
        OnObjectDisabled
    }

    public class BehaviorTreeMissionLogic : MissionBehavior
    {
        static BehaviorTreeMissionLogic _instance;
        Dictionary<SubscriptionPossibilities, List<BannerlordBTListener>> actions = new();
        public Dictionary<Agent, BehaviorTree> trees = new();
        private readonly List<(BannerlordBTTickListener listener, double elapsedTime)> tickListeners = new();

        float mainTime;
        float giveTreesTime;
        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;
        public void Subscribe(BannerlordBTListener listener)
        {
            if (!actions.TryGetValue(listener.SubscribesTo, out var descriptors))
            {
                descriptors = new List<BannerlordBTListener>();
                actions[listener.SubscribesTo] = descriptors;
            }
            descriptors.Add(listener);
        }
        public void UnSubscribe(BannerlordBTListener listener)
        {
            listener.NotifyWithCancel();
            actions[listener.SubscribesTo].Remove(listener);
        }
        public void Subscribe(BannerlordBTTickListener listener)
        {
            tickListeners.Add((listener, 0));
        }
        public void UnSubscribe(BannerlordBTTickListener listener)
        {
            for (int i = 0; i < tickListeners.Count; i++)
            {
                if (tickListeners[i].listener == listener)
                {
                    tickListeners.RemoveAt(i);
                    break;
                }
            }
            listener.NotifyWithCancel();
        }
        public List<BannerlordBTListener> GetAllListeners()
        {
            List<BannerlordBTListener> allListeners = new();
            foreach (List<BannerlordBTListener> listenerList in actions.Values)
                allListeners.AddRange(listenerList);
            return allListeners;
        }
        public BehaviorTreeMissionLogic()
        {
            Globals.IsMissionInitialized = false;
            BehaviorTreeBannerlordWrapper.Instance.CurrentMissionLogic = this;
        }
        public override void AfterStart()
        {
            giveTreesTime = 0;
        }

        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);
            if (Agent.Main == null)
                return;

            //if (giveTreesTime > 1 && !Globals.IsMissionInitialized)
            //{
            //    BTRegister.RegisterClass("ExampleTree", objects => ExampleTree.BuildTree(objects));
            //    BTRegister.RegisterClass("EnemyAttackTree", objects => RatTestTree.BuildTree(objects));
            //    foreach (var agent in Mission.Agents)
            //    {
            //        agent.AddComponent(new BehaviorTreeAgentComponent(agent, "EnemyAttackTree"));
            //        //if (agent == Agent.Main) continue;
            //        //agent.AddBehaviorTree();
            //    }
            //    Globals.IsMissionInitialized = true;
            //}
            //else
            //{
            //    giveTreesTime += dt;
            //}

            object[] toSend = { dt };
            for (int i = tickListeners.Count - 1; i >= 0; i--)
            {
                var (listener, elapsedTime) = tickListeners[i];
                elapsedTime += dt;

                if (elapsedTime >= listener.SecondsTillEvent)
                {
                    listener.Notify(toSend);
                }
                else
                    tickListeners[i] = (listener, elapsedTime);
            }
        }
        private List<BannerlordBTListener> FindCalledListeners(Agent agent, SubscriptionPossibilities action)
        {
            List<BannerlordBTListener> toReturn = new();
            actions.TryGetValue(action, out var listeners);
            if (listeners == null) return toReturn;
            foreach (var listener in listeners)
                if (agent.GetBehaviorTree() == listener.Tree)
                    toReturn.Add(listener);
            return toReturn;
        }
        public override void OnAgentDismount(Agent agent)
        {
            List<BannerlordBTListener> listeners = FindCalledListeners(agent, SubscriptionPossibilities.OnSelfDismount);
            if (listeners != null)
            {
                object[] toSend = { };
                listeners.ForEach(listener => { listener.Notify(toSend); });
            }

            listeners = FindCalledListeners(agent, SubscriptionPossibilities.OnAgentDismount);
            if (listeners != null)
            {
                object[] toSend = { agent };
                listeners.ForEach(listener => { listener.Notify(toSend); });

            }
        }
        public override void OnAgentFleeing(Agent affectedAgent)
        {
            List<BannerlordBTListener> listeners = FindCalledListeners(affectedAgent, SubscriptionPossibilities.OnSelfFleeing);
            if (listeners != null)
            {
                object[] toSend = { affectedAgent };
                listeners.ForEach(listener => { listener.Notify(toSend); });
            }
            actions.TryGetValue(SubscriptionPossibilities.OnAgentFleeing, out listeners);
            if (listeners != null)
            {
                List<BannerlordBTListener> listToIterate = listeners.ToList();
                object[] toSend = { affectedAgent };
                listeners.ForEach(listener => { listener.Notify(toSend); });
            }
        }
        public override void OnAgentDeleted(Agent affectedAgent)
        {
            List<BannerlordBTListener> listeners = FindCalledListeners(affectedAgent, SubscriptionPossibilities.OnSelfAlarmedStateChanged);
            if (listeners == null) return;
            List<BannerlordBTListener> listToIterate = listeners.ToList();
            object[] toSend = { };
            listToIterate.ForEach(listener => { listener.Notify(toSend); });
        }
        public override void OnAgentMount(Agent agent)
        {
            List<BannerlordBTListener> listeners = FindCalledListeners(agent, SubscriptionPossibilities.OnSelfMount);
            if (listeners != null)
            {
                object[] toSend = { };
                listeners.ForEach(listener => { listener.Notify(toSend); });
            }

            listeners = FindCalledListeners(agent, SubscriptionPossibilities.OnAgentMount);
            if (listeners != null)
            {
                object[] toSend = { agent };
                listeners.ForEach(listener => { listener.Notify(toSend); });
            }
        }
        public override void OnAgentPanicked(Agent affectedAgent)
        {
            actions.TryGetValue(SubscriptionPossibilities.OnAgentPanicked, out var listeners);
            if (listeners == null) return;
            List<BannerlordBTListener> listToIterate = listeners.ToList();
            object[] toSend = { affectedAgent };
            listToIterate.ForEach(listener => { listener.Notify(toSend); });
        }
        public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
        {
            List<BannerlordBTListener> listeners = FindCalledListeners(affectedAgent, SubscriptionPossibilities.OnSelfRemoved);
            if (listeners == null)
            {
                List<BannerlordBTListener> listToIterate = listeners.ToList();
                object[] toSend = { affectorAgent, agentState, blow };
                listToIterate.ForEach(listener => { listener.Notify(toSend); });
            }
            listeners = FindCalledListeners(affectorAgent, SubscriptionPossibilities.OnSelfKilledEnemy);
            if (listeners == null)
            {
                List<BannerlordBTListener> listToIterate = listeners.ToList();
                object[] toSend = { affectedAgent, agentState, blow };
                listToIterate.ForEach(listener => { listener.Notify(toSend); });
            }
            actions.TryGetValue(SubscriptionPossibilities.OnAgentRemoved, out listeners);
            if (listeners == null)
            {
                List<BannerlordBTListener> listToIterate = listeners.ToList();
                object[] toSend = { affectedAgent, affectorAgent, agentState, blow };
                listToIterate.ForEach(listener => { listener.Notify(toSend); });
            }
        }
        public override void OnAgentShootMissile(Agent shooterAgent, EquipmentIndex weaponIndex, Vec3 position, Vec3 velocity, Mat3 orientation, bool hasRigidBody, int forcedMissileIndex)
        {
            actions.TryGetValue(SubscriptionPossibilities.OnAgentShootMissile, out var listeners);
            if (listeners == null) return;
            List<BannerlordBTListener> listToIterate = listeners.ToList();
            object[] toSend = { shooterAgent, weaponIndex, position, velocity, orientation, hasRigidBody, forcedMissileIndex };
            listToIterate.ForEach(listener => { listener.Notify(toSend); });
        }
        public override void OnFocusGained(Agent agent, IFocusable focusableObject, bool isInteractable)
        {
            List<BannerlordBTListener> listeners = FindCalledListeners(agent, SubscriptionPossibilities.OnSelfGainedFocus);
            if (listeners == null) return;
            List<BannerlordBTListener> listToIterate = listeners.ToList();
            object[] toSend = { focusableObject, isInteractable };
            listToIterate.ForEach(listener => { listener.Notify(toSend); });
        }
        public override void OnFocusLost(Agent agent, IFocusable focusableObject)
        {
            List<BannerlordBTListener> listeners = FindCalledListeners(agent, SubscriptionPossibilities.OnSelfLostFocus);
            if (listeners == null) return;
            List<BannerlordBTListener> listToIterate = listeners.ToList();
            object[] toSend = { focusableObject };
            listToIterate.ForEach(listener => { listener.Notify(toSend); });
        }
        public override void OnAgentAlarmedStateChanged(Agent agent, Agent.AIStateFlag flag)
        {
            List<BannerlordBTListener> listeners = FindCalledListeners(agent, SubscriptionPossibilities.OnSelfAlarmedStateChanged);
            if (listeners == null) return;
            object[] toSend = { flag };
            listeners.ForEach(listener => { listener.Notify(toSend); });
        }
        public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon affectorWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
        {
            List<BannerlordBTListener> listeners = FindCalledListeners(affectedAgent, SubscriptionPossibilities.OnSelfIsHit);
            if (listeners != null)
            {
                object[] toSend = { affectorAgent, affectorWeapon, blow, attackCollisionData };
                listeners.ForEach(listener => { listener.Notify(toSend); });
            }

            listeners = FindCalledListeners(affectorAgent, SubscriptionPossibilities.OnSelfHitsEnemy);
            if (listeners != null)
            {
                object[] toSend = { affectedAgent, affectorWeapon, blow, attackCollisionData };
                listeners.ForEach(listener => { listener.Notify(toSend); });
            }
        }
        public override void OnObjectUsed(Agent userAgent, UsableMissionObject usedObject)
        {
            List<BannerlordBTListener> listeners = FindCalledListeners(userAgent, SubscriptionPossibilities.OnSelfUsedObject);
            if (listeners == null) return;
            object[] toSend = { usedObject };
            listeners.ForEach(listener => { listener.Notify(toSend); });
        }
        protected override void OnObjectDisabled(DestructableComponent destructionComponent)
        {
            actions.TryGetValue(SubscriptionPossibilities.OnObjectDisabled, out var listeners);
            if (listeners == null) return;
            object[] toSend = { destructionComponent};
            listeners.ForEach(listener => { listener.Notify(toSend); });
        }
        public override void OnObjectStoppedBeingUsed(Agent userAgent, UsableMissionObject usedObject)
        {
            List<BannerlordBTListener> listeners = FindCalledListeners(userAgent, SubscriptionPossibilities.OnSelfStoppedUsingObject);
            if (listeners == null) return;
            object[] toSend = { usedObject };
            listeners.ForEach(listener => { listener.Notify(toSend); });
        }
        public override void OnEndMissionInternal()
        {
            BehaviorTreeBannerlordWrapper.Instance.Dispose();
        }
    }
}
