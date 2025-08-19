using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorTrees
{
    public static class BTRegister
    {
        public static ILogger? Logger { get; private set; }
        
        /// <summary>
        /// add a logger to the BTRegister, this logger will be used to log messages from the behavior tree.
        /// </summary>
        /// <param name="logger"></param>
        public static void AddLogger(ILogger logger)
        {
            Logger = logger;
        }

        private static readonly Dictionary<string, Func<object[], BehaviorTree?>> RegisteredBuilders = new();
        public static void RegisterClass(string className, Func<object[], BehaviorTree?> factory)
        {
            if (!RegisteredBuilders.ContainsKey(className))
            {
                RegisteredBuilders[className] = factory;
            }
        }
        /// <summary>
        /// Build the tree, the class needs to be registered through RegisterClass method first.
        /// </summary>
        /// <param name="className"></param>
        /// <param name="args">The arguments the tree needs to be built.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static BehaviorTree? Build(string className, params object[] args)
        {
            if (RegisteredBuilders.TryGetValue(className, out var builder))
            {
                return builder(args);
            }

            throw new ArgumentException($"Class '{className}' is not registered.");
        }
    }
}
