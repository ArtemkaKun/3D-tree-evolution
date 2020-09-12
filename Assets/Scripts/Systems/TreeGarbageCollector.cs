using Systems.TreeCell;
using Components.Tree;
using Unity.Entities;

namespace Systems
{
    [UpdateAfter(typeof(WorldSystem))]
    [UpdateBefore(typeof(TreeCellsSystem))]
    public class TreeGarbageCollector : ComponentSystem
    {
        protected override void OnUpdate()
        {
            var entitiesToQuery = EntityManager.CreateEntityQuery(typeof(DeleteTag));
            EntityManager.DestroyEntity(entitiesToQuery);
        }
    }
}