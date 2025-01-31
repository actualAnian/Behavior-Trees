# Behavior Trees
The goal of this library is to allow users to create their own adapters for specific projects, or to seamlessly create new trees for existing adapters.

# Using the library

## Creating an adapter
TODO

## Using the Bannerlord Behavior Tree adapter

### Creating a new Blackboard
A Behavior Tree may implement multiple blackboards. Every Task/Decorator knows which blackboards it has access to, making it modular - many trees may use the same task as long as they implement all the necessary blackboards.

With that said, each tree will probably contain each own specific Blackboard, with all the necessary information. All blackboards have to be c# interfaces, inheriting after IBTBlackboard.
Blackboards can only contain objects of type BTBlackboardValue<Type>.

### Creating a new Tree class

Most trees will probably use the IBTBannerlordBase blackboard, which gives access to the Actor object, making it necessary to use most predefined tasks/decorators.

Each tree class should contain a constructor, where all the BTBlackboardValue<Type> objects are created (they are synced with task objects during the building of a tree)

Above that, a tree should override the method:
```c#
public static new BehaviorTree? BuildTree(object[] objects).
```
Inside this method, the tree object should be built, starting with the method BehaviorTree.StartBuildingTree, which returns the BehaviorTreeBuilder object.

IMPORTANT, objects[0] WILL ALWAYS BE A REFERENCE TO AN Agent object.
```c#
        public static new BehaviorTree? BuildTree(object[] objects)
        {
            if (objects[0] is not Agent agent) return null;

            Vec3 position = new(373, 295, 20);
            ExampleTree? tree = StartBuildingTree(new ExampleTree(agent))
                .AddSelector("main")
                    .AddSequence("spotted", new AlarmedDecorator(SubscriptionPossibilities.OnSelfAlarmedStateChanged))
                        .AddTask(new PrintQuickMessageTask("I am spotted"))
                        .Up()
                    .AddSequence("hit", new HitDecorator(SubscriptionPossibilities.OnSelfIsHit))
                        .AddTask(new MoveToPlaceTask(position))
                        .AddTask(new PrintQuickMessageTask("I am hit"))
                        .Up()
                    .AddSequence("moved", new InPositionDecorator(position))
                        .AddTask(new ClearMovementTask())
                        .Up()
                .Finish();
            return tree;
        }
```
### Building a Tree
The BehaviorTreeBuilder provides the following options
1. AddSelector
2. AddSequence
3. AddRandomSelector
4. AddSubTree - a previously created tree, has to be registered first
5. AddTask
6. Up - return to the parent node <br>
The BehaviorTreeBuilder copies all the blackboards values to the respective tasks/ decorators (using reflection), so for example if both a tree and PlayAnimationTask implement IBTBannerlordBase, the BTBlackboardValue<Agent> reference inside will be copied from the tree to PlayAnimationTask.

### Syncing the BTBlackboardValues
To sync the values, tasks and decorators need to have fields to set the value to. As such the blackboard interfaces should be implemented as follows: <br>
```c#
public BTBlackboardValue<Agent> Agent { get => agent; set => agent = value; } //the property from IBTBannerlordBase blackboard (interface) <br>
BTBlackboardValue<Agent> agent; // the field Agent sets and gets the value from<br>
```
### Creating a new Task
Task classes have to inherit after BTTask, and override the Execute method as follows:
```c#
public class PlayAnimationTask : BTTask, IBTBannerlordBase
{
    string actionId;
    public PlayAnimationTask(string actionId)
    {
        this.actionId = actionId;
    }
    BTBlackboardValue<Agent> agent;
    public BTBlackboardValue<Agent> Agent { get => agent; set => agent = value; }

    public override async Task<bool> Execute(CancellationToken cancellationToken)
    {
        Agent.GetValue().SetActionChannel(0, ActionIndexCache.Create(actionId), true);
        return true;
    }
}
```
### Creating a decorator
there are 3 types of decorators
#### BannerlordNoWaitDecorator
will simply return information, if the Evaluation returns true or false
```c#
    public class HealthBelowPercentageDecorator : BannerlordNoWaitDecorator, IBTBannerlordBase
    {
        BTBlackboardValue<Agent> agent;
        private int healthPercentageThreshold;
        public HealthBelowPercentageDecorator(int healthPercentageThreshold)
        {
            healthPercentageThreshold = this.healthPercentageThreshold;
        }

        public BTBlackboardValue<Agent> Agent { get => agent; set => agent = value; }

        public override bool Evaluate()
        {
            int minHealth = (int)Agent.GetValue().HealthLimit * healthPercentageThreshold;
            return Agent.GetValue().Health < minHealth;
        }
    }
```
#### BannerlordEventDecorator
If the evaluation returns false, waits till the game calls a specific event, and checks again if the evaluation now returns true. ```SubscriptionPossibilities``` enum tells the decorator which event to wait for.
Since events can possibly return additional information, the ```Notify``` method can be used to check for specific conditions. To check which event returns what data, see ```BehaviorTreeWrapper.BehaviorTreeMissionLogic```.
```c#
    public class HitDecorator : BannerlordEventDecorator
    {
        private bool hasBeenHit = false;
        public HitDecorator(SubscriptionPossibilities SubscribesTo) : base(SubscribesTo) { }
        public override bool Evaluate()
        {
            if (!hasBeenHit) return false;
            hasBeenHit = false;
            return true;
        }
        public override void Notify(object[] data)
        {
            hasBeenHit = true;
        }
    }
```
#### BannerlordTickTimedDecorator
A special event decorator, that waits a specified time before returning true.
```c#
    public class WaitNSecondsTickDecorator : BannerlordTickTimedDecorator
    {
        bool hasBeenNotified = false;
        public WaitNSecondsTickDecorator(double timeToWait ) : base(timeToWait) { }
        public override bool Evaluate()
        {
            if (hasBeenNotified)
            {
                hasBeenNotified = false;
                return true;
            }
            else return false;
        }
        public override void Notify(object[] data) { hasBeenNotified = true; }
    }
```
### Adding the created Tree.
To add a tree to the agent, you first need to register it with BTRegister.RegisterClass

The RegisterClass takes a function that builds the tree as an argument. its a good practice to give it the Tree.BuildTree method.
```c#
BehaviorTrees.BTRegister.RegisterClass("ExampleTree", objects => ExampleTree.BuildTree(objects));
```
After the tree has been added, you can add it to an agent as follows:
```c#
agent.AddComponent(new BehaviorTreeAgentComponent(agent, "ExampleTree"));
```
The base library trees are registered before any user-created trees are built and are always available.