using Components.Tree;
using Unity.Entities;

namespace Systems
{
    // [UpdateBefore(typeof(WorldSystem))]
    // public class TreeGarbageCollector : ComponentSystem
    // {
    //     protected override void OnUpdate()
    //     {
    //         var entitiesToQuery = EntityManager.CreateEntityQuery(typeof(DeleteTag));
    //         EntityManager.DestroyEntity(entitiesToQuery);
    //     }
    // }
}