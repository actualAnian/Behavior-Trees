using BehaviorTrees;
using BehaviorTreeWrapper.AbstractDecoratorsListeners;
using BehaviorTreeWrapper.Trees;
using System;
using System.Linq;
using System.Threading;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BehaviorTreeWrapper
{
    public class BehaviorTreeBannerlordWrapper : IDisposable
    {
        private BehaviorTreeMissionLogic? _missionLogic;
        public BehaviorTreeMissionLogic CurrentMissionLogic
        {
            get => _missionLogic;
            set
            {
                _missionLogic = value;
                _disposed = false;
            }
        }
        static BehaviorTreeBannerlordWrapper? _instance;
        private bool _disposed = false;
        public static BehaviorTreeBannerlordWrapper Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = new BehaviorTreeBannerlordWrapper();
                RegisterDefaultTrees();
                return _instance;
            }
        }
        public void Subscribe(BannerlordBTListener listener)
        {
            CurrentMissionLogic?.Subscribe(listener);
        }
        public void UnSubscribe(BannerlordBTListener listener)
        {
            CurrentMissionLogic?.UnSubscribe(listener);
        }
        public void Subscribe(BannerlordBTTickListener listener)
        {
            CurrentMissionLogic?.Subscribe(listener);
        }
        public void UnSubscribe(BannerlordBTTickListener listener)
        {
            CurrentMissionLogic?.UnSubscribe(listener);
        }
        private static void RegisterDefaultTrees()
        {
            BTRegister.RegisterClass("PerformAnAttackTree", objects => PerformAnAttackTree.BuildTree(objects));
        }
        internal BehaviorTree? AddBehaviorTree(string treeName, object[] args)
        {
            Agent agent = (Agent)args[0];
            if (CurrentMissionLogic.trees.TryGetValue(agent, out var descriptors))
            {
                InformationManager.DisplayMessage(new($"Error adding a tree to {agent.Name} as it already exists"));
            }
            args.Prepend(agent);
            try
            {
                BehaviorTree? tree = BTRegister.Build(treeName, args);
                if (tree == null)
                {
                    InformationManager.DisplayMessage(new($"Could not find a tree with name {treeName}"));
                    return null;
                }
                else
                {
                    CurrentMissionLogic.trees[agent] = tree;
                    return tree;
                }
            }
            catch (Exception ex)
            {
                InformationManager.DisplayMessage(new($"Error building a tree {treeName} for {agent.Name}. Message {ex.Message}"));
                return null;
            }
        }
        public void DisposeTree(Agent agent)
        {
            if (!_disposed && _missionLogic != null)
            {
                _missionLogic.trees[agent].Dispose();
                _missionLogic.trees.Remove(agent);
            }
        }
        public void Dispose()
        {
            if (!_disposed)
            {
                foreach (BehaviorTree tree in CurrentMissionLogic.trees.Values)
                {
                    tree.Dispose();
                }
                CurrentMissionLogic = null;
                _disposed = true;
            }
        }
    }
}