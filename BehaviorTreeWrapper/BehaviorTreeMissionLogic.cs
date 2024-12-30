﻿using TaleWorlds.MountAndBlade;
using System.Collections.Generic;
using TaleWorlds.Core;
using System.Linq;

namespace BehaviorTreeWrapper
{
    public enum SubscriptionPossibilities
    {
        OnMissionTick,
        OnAgentAlarmedStateChanged,
        OnAgentHit
    }
    public class BehaviorTreeMissionLogic : MissionBehavior
    {

        static BehaviorTreeMissionLogic _instance;
        Dictionary<SubscriptionPossibilities, List<BannerlordBTListener>> actions = new();
        public Dictionary<int, BasicTree> trees = new();
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
            BehaviorTreeBannerlordWrapper.Instance.CurrentMission = this;
        }
        //public override void OnAgentCreated(Agent agent)
        //{
        //    base.OnAgentCreated(agent);
        //    agent.AddBehaviorTree();
        //}
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
                    agent.AddBehaviorTree();
                }
                Globals.IsMissionInitialized = true;
            }
            else
            {
                giveTreesTime += dt;
            }
            //actions.TryGetValue(SubscriptionPossibilities.OnAgentAlarmedStateChanged, out var listeners);
            //if (listeners == null) return;
            //if (mainTime < 2) { mainTime += dt; return; }
            //foreach (var listener in listeners)
            //{
            //    listener.Signal();
            //}
            //mainTime = 0;
        }

        public override void OnAgentAlarmedStateChanged(Agent agent, Agent.AIStateFlag flag)
        {
            actions.TryGetValue(SubscriptionPossibilities.OnAgentAlarmedStateChanged, out var listeners);
            if (listeners == null) return;
            foreach (var listener in listeners)
            {
                if (agent.GetBehaviorTree() == listener.NotifiedObject.NotifiedTree)
                {
                    listener.Notify();
                    break;
                }
            }
            base.OnAgentAlarmedStateChanged(agent, flag);
        }
        public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon affectorWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
        {
            actions.TryGetValue(SubscriptionPossibilities.OnAgentHit, out var listeners);
            if (listeners == null) return;
            foreach (var listener in listeners)
            {
                if (affectedAgent.GetBehaviorTree() == listener.NotifiedObject.NotifiedTree)
                {
                    listener.Notify();
                    break;
                }
            }
        }
        public override void OnEndMissionInternal()
        {
            BehaviorTreeBannerlordWrapper.Instance.Dispose();
        }
    }
}
