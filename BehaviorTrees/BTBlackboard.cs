namespace BehaviorTrees
{
    /// <summary>
    /// Only objects of type BTBlackBoardValue will work.
    /// </summary>
    public interface IBTBlackboard { }
    public class BTBlackboardValue<Type>
    {
        private Type value;
        public BTBlackboardValue(Type value)
        {
            this.value = value;
        }
        public Type GetValue()
        {
            return value;
        }
        public void SetValue(Type value)
        {
            this.value = value;
        }
    }

    /*
     * new blackboard type:
     * simply a class
     * the tree should have a list of blackboard objects
     * tasks, decorators take blackboard objects as parameters in constructors
     */
    public interface NewIBTBlackboard
    {
    }
}
