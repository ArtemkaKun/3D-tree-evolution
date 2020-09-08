using Unity.Entities;

public class WorldSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        if (global::World.SimulationStatus.WorldAge >= 10000) return;
        if (!global::World.SimulationStatus.IsSimulationRun) return;
        
        global::World.IncreaseWorldAge();
    }
}
