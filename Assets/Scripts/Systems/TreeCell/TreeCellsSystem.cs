// using Components.Tree;
// using Components.TreeCell;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Physics;
// using Unity.Physics.Systems;
// using Unity.Transforms;
// using UnityEngine;
// using RaycastHit = Unity.Physics.RaycastHit;
//
// namespace Systems.TreeCell
// {
//     [UpdateAfter(typeof(TreeGarbageCollector))]
//     [UpdateAfter(typeof(WorldSystem))]
//     public class TreeCellsSystem : ComponentSystem
//     {
//         private const float RayLength = 3f;
//
//         private CollisionFilter _groundCollisionFilter;
//
//         protected override void OnStartRunning()
//         {
//             base.OnStartRunning();
//             
//             _groundCollisionFilter = new CollisionFilter
//             {
//                 BelongsTo = 1u,
//                 CollidesWith = 1u,
//                 GroupIndex = 0
//             };
//         }
//
//         protected override void OnUpdate()
//         {
//             ProceedTreeCells();
//
//             global::World.IncreaseWorldAge();
//         }
//
//         private void ProceedTreeCells()
//         {
//             Entities.ForEach((Entity cell, ref TreeCellTag _, ref TreeCellComponent cellData,
//                 ref Translation translation) =>
//             {
//                 CellCalcEnergy(ref cellData, ref translation);
//
//                 if (cellData.Energy < 18 || !cellData.IsSeed) return;
//
//                 GrowNewCells(ref cellData, translation.Value);
//             });
//         }
//         
//         private void CellCalcEnergy(ref TreeCellComponent cellData, ref Translation translation)
//         {
//             cellData.Energy -= global::World.SimulationConstants.CellEnergyUsage;
//
//             var sunLvl = GetSunLvl(translation.Value);
//
//             if (sunLvl == 0) return;
//
//             cellData.Energy += sunLvl * Mathf.RoundToInt(translation.Value.y + 6);
//         }
//
//         private int GetSunLvl(Vector3 position)
//         {
//             var ray = CastRay(new Vector3(0, RayLength, 0), position);
//
//             var physicWorld = Unity.Entities.World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
//             var collisionWorld = physicWorld.PhysicsWorld.CollisionWorld;
//             
//             var hittedEntities = new NativeList<RaycastHit>(Allocator.Temp);
//             collisionWorld.CastRay(ray, ref hittedEntities);
//
//             var hittedEntitiesCount = hittedEntities.Length;
//             hittedEntities.Dispose();
//             
//             if (hittedEntitiesCount > 2) return 0;
//
//             return 3 - hittedEntitiesCount;
//         }
//
//         private RaycastInput CastRay(Vector3 direction, Vector3 position)
//         {
//             return new RaycastInput
//             {
//                 Start = position,
//                 End = position + direction,
//                 Filter = _groundCollisionFilter
//             };
//         }
//         
//         private void GrowNewCells(ref TreeCellComponent cellData, Vector3 position)
//         {
//             var growPairs = new (int GeneValue, Vector3 GrowDirection)[]
//             {
//                 (cellData.Genes.Up, Vector3.up),
//                 (cellData.Genes.Down, Vector3.down),
//                 (cellData.Genes.Forward, Vector3.forward),
//                 (cellData.Genes.Back, Vector3.back),
//                 (cellData.Genes.Left, Vector3.left),
//                 (cellData.Genes.Right, Vector3.right)
//             };
//
//             foreach (var (geneValue, growDirection) in growPairs)
//             {
//                 if (cellData.Energy < 18) return;
//                 if (geneValue >= global::World.SimulationConstants.TreeGenesCount) return;
//                 if (IsSpaceOccupied(growDirection, position)) return;
//                 
//                 cellData.Energy -= 18;
//
//                 GrowCell(ref cellData,growDirection + position, geneValue);
//             }
//         }
//
//         private bool IsSpaceOccupied(Vector3 direction, Vector3 position)
//         {
//             var ray = CastRay(direction, position);
//             
//             var physicWorld = Unity.Entities.World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
//             var collisionWorld = physicWorld.PhysicsWorld.CollisionWorld;
//             
//             return collisionWorld.CastRay(ray, out _);
//         }
//
//         private void GrowCell(ref TreeCellComponent cellData, Vector3 newCellPosition, int geneValue)
//         {
//             if (newCellPosition.y < 0) return;
//
//             var entityManager = Unity.Entities.World.DefaultGameObjectInjectionWorld.EntityManager;
//             
//             var newCell = entityManager.Instantiate(global::World.SimulationResources.TreeCellPrefab);
//
//             entityManager.AddComponentData(newCell, new TreeCellTag());
//             
//             entityManager.GetBuffer<TreeCellsComponent>(cellData.ParentTree).Add(newCell);
//             
//             entityManager.SetComponentData(newCell, new Translation
//             {
//                 Value = newCellPosition
//             });
//
//             entityManager.SetComponentData(newCell, new TreeCellComponent
//             {
//                 ParentTree = cellData.ParentTree,
//                 IsSeed = true,
//                 Genes = entityManager.GetBuffer<TreeGenesComponent>(cellData.ParentTree)[geneValue]
//             });
//
//             cellData.IsSeed = false;
//             //gameObject.GetComponent<Renderer>().material = _woodMaterial;
//         }
//     }
// }