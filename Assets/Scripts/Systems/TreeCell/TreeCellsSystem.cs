using Components.TreeCell;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace Systems.TreeCell
{
    [UpdateAfter(typeof(WorldController))]
    [UpdateAfter(typeof(TreeGarbageCollector))]
    public class TreeCellsSystem : ComponentSystem
    {
        private const float RayLength = 3f;

        private CollisionFilter _groundCollisionFilter;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            
            _groundCollisionFilter = new CollisionFilter
            {
                BelongsTo = 1u,
                CollidesWith = 1u,
                GroupIndex = 0
            };
        }

        protected override void OnUpdate()
        {
            ProceedTreeCells();

            global::World.IncreaseWorldAge();
        }

        private void ProceedTreeCells()
        {
            Entities.ForEach((Entity cell, ref TreeCellTag _, ref TreeCellComponent cellData,
                ref Translation translation) =>
            {
                CellCalcEnergy(ref cellData, ref translation);

                if (cellData.Energy < 18 || !cellData.IsSeed) return;

                cellData.Energy -= 18;

                //GrowNewCell();
            });
        }
        
        private void CellCalcEnergy(ref TreeCellComponent cellData, ref Translation translation)
        {
            cellData.Energy -= global::World.SimulationConstants.CellEnergyUsage;

            var sunLvl = GetSunLvl(ref translation);

            if (sunLvl == 0) return;

            cellData.Energy += sunLvl * Mathf.RoundToInt(translation.Value.y + 6);
        }

        private int GetSunLvl(ref Translation translation)
        {
            var physicWorld = Unity.Entities.World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
            var collisionWorld = physicWorld.PhysicsWorld.CollisionWorld;

            var ray = new RaycastInput
            {
                Start = translation.Value,
                End = translation.Value + new float3(0, RayLength, 0),
                Filter = _groundCollisionFilter
            };

            var hittedEntities = new NativeList<RaycastHit>(Allocator.Temp);
            collisionWorld.CastRay(ray, ref hittedEntities);

            var hittedEntitiesCount = hittedEntities.Length;
            hittedEntities.Dispose();
            
            if (hittedEntitiesCount > 2) return 0;

            return 3 - hittedEntitiesCount;
        }
    }
}