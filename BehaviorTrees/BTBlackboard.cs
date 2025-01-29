namespace BehaviorTrees
{
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
}
