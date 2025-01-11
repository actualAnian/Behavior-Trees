using TaleWorlds.MountAndBlade;
using System.Collections.Generic;
using TaleWorlds.Core;
using System.Linq;
using System;

namespace BehaviorTreeWrapper
{
    public enum SubscriptionPossibilities
    {
        OnMissionTick,
        OnAgentAlarmedStateChanged,
        OnAgentHit
    } // more to be added

    public class BehaviorTreeMissionLogic : MissionBehavior
    {
        Dictionary<SubscriptionPossibilities, Dictionary<string, List<Type>>> DataSets = new()
        {
            [SubscriptionPossibilities.OnAgentAlarmedStateChanged] = new() { ["test"] = new() { typeof(string) } }
        };

        static BehaviorTreeMissionLogic _instance;
        Dictionary<SubscriptionPossibilities, List<BannerlordBTListener>> actions = new();
        public Dictionary<int, BannerlordTree> trees = new();
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

            if (giveTreesTime > 1 && !Globals.IsMissionInitialized)
            {
                foreach (var agent in Mission.Agents)
                {
                    if (agent == Agent.Main) continue; 
                    agent.AddBehaviorTree();
                }
                Globals.IsMissionInitialized = true;
            }
            else
            {
                giveTreesTime += dt;
            }
            if (mainTime < 2) { mainTime += dt; return; }
            actions.TryGetValue(SubscriptionPossibilities.OnMissionTick, out var listeners);
            if (listeners == null) return;
            List<BannerlordBTListener> listToIterate = listeners.ToList();

            List<object> toSend = new() { dt };
            listToIterate.ForEach(listener => { listener.Notify(toSend); });
            mainTime = 0;
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

        public override void OnAgentAlarmedStateChanged(Agent agent, Agent.AIStateFlag flag)
        {
            List<BannerlordBTListener> listeners = FindCalledListeners(agent, SubscriptionPossibilities.OnAgentAlarmedStateChanged);
            if (listeners == null) return;

            List<object> toSend = new() { flag };
            listeners.ForEach(listener => { listener.Notify(toSend); });
        }
        public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon affectorWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
        {
            List<BannerlordBTListener> listeners = FindCalledListeners(affectedAgent, SubscriptionPossibilities.OnAgentHit);
            if (listeners == null) return;

            List<object> toSend = new() { affectorAgent, affectorWeapon, blow, attackCollisionData };
            listeners.ForEach(listener => { listener.Notify(toSend); });
        }
        public override void OnEndMissionInternal()
        {
            BehaviorTreeBannerlordWrapper.Instance.Dispose();
        }
    }
}
