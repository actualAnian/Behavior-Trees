using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorTrees
{
    public static class BTRegister
    {
        private static readonly Dictionary<string, Func<object[], BehaviorTree?>> RegisteredBuilders = new();
        public static void RegisterClass(string className, Func<object[], BehaviorTree?> factory)
        {
            if (!RegisteredBuilders.ContainsKey(className))
            {
                RegisteredBuilders[className] = factory;
            }
        }
        public static BehaviorTree Build(string className, params object[] args)
        {
            if (RegisteredBuilders.TryGetValue(className, out var builder))
            {
                return builder(args);
            }

            throw new ArgumentException($"Class '{className}' is not registered.");
        }
    }
}
