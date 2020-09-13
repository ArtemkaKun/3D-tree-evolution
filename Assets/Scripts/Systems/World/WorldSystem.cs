using System;
using System.Collections.Generic;
using Systems;
using Components.Tree;
using Components.TreeCell;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = System.Random;
using RaycastHit = Unity.Physics.RaycastHit;

//[UpdateAfter(typeof(TreeGarbageCollector))]
public class WorldSystem : ComponentSystem
{
    private const float RayLength = 3f;

    private CollisionFilter _groundCollisionFilter;

    protected override void OnCreate()
    {
        base.OnCreate();
        
        _groundCollisionFilter = new CollisionFilter
        {
            BelongsTo = 1u,
            CollidesWith = 1u,
            GroupIndex = 0
        };
    }

    protected override void OnUpdate()
    {
        if (!global::World.SimulationStatus.IsSimulationRun) return;
        if (global::World.SimulationStatus.WorldAge >= 10000) return;
        
        ProceedTrees();

        var entitiesToQuery = EntityManager.CreateEntityQuery(typeof(DeleteTag));
        EntityManager.DestroyEntity(entitiesToQuery);
        
        global::World.IncreaseWorldAge();
    }

    private void ProceedTrees()
    {
        Entities.WithNone<DeleteTag>().ForEach((Entity tree, ref TreeTag _, ref TreeComponent treeData) =>
        {
            ++treeData.TreeAge;

            if (treeData.TreeAge >= global::World.SimulationConstants.MaxTreeAge)
            {
                DestroyTree(ref tree);
                return;
            }

            TreeEnergyCalc(ref tree, ref treeData);

            if (treeData.TreeEnergy <= 0)
            {
                DestroyTree(ref tree);
                return;
            }

            ProceedTreeCells(ref tree);
        });
    }

    private void DestroyTree(ref Entity tree)
    {
        var cellsToDestroy = new List<Entity>();
        
        foreach (var cell in EntityManager.GetBuffer<TreeCellsComponent>(tree))
        {
            // if (EntityManager.GetComponentData<TreeCellComponent>(cell.Value).IsSeed)
            // {
            //     if (new Random().Next(0, 100) <= 5)
            //     {
            //         var newGene = Mutate(ref tree);
            //         
            //         IncreaseGeneration();
            //     }
            //     //TODO Grow new tree
            //     continue;
            // }
            
            cellsToDestroy.Add(cell.Value);
        }

        foreach (var cell in cellsToDestroy)
        {
            EntityManager.AddComponentData(cell, new DeleteTag());
        }
        
        EntityManager.AddComponentData(tree, new DeleteTag());
    }

    private GrowGenes Mutate(ref Entity tree)
    {
        var newGene = new NativeArray<int>(1, Allocator.TempJob);
        var geneToMutate = new NativeArray<int>(1, Allocator.TempJob);
        var directionToMutate = new NativeArray<int>(1, Allocator.TempJob);

        var time = DateTime.Now;

        var job = new MutateJob
        {
            Seed = (uint) new Random().Next(time.Hour + time.Minute + time.Second + tree.Index),
            GenesCount = global::World.SimulationConstants.TreeGenesCount,
            NewGene = newGene,
            GeneToMutate = geneToMutate,
            DirectionToMutate = directionToMutate
        };

        job.Schedule().Complete();

        var treeGene = EntityManager.GetBuffer<TreeGenesComponent>(tree)[geneToMutate[0]].Value;

        switch (directionToMutate[0])
        {
            case 0:
                treeGene.Up = newGene[0];
                break;

            case 1:
                treeGene.Down = newGene[0];
                break;

            case 2:
                treeGene.Forward = newGene[0];
                break;

            case 3:
                treeGene.Back = newGene[0];
                break;

            case 4:
                treeGene.Left = newGene[0];
                break;

            case 5:
                treeGene.Right = newGene[0];
                break;
        }

        //Test buffer assing
        // EntityManager.GetBuffer<TreeGenesComponent>(tree).Insert(geneToMutate[0], new TreeGenesComponent{Value = treeGenesComponent});
        // var a = EntityManager.GetBuffer<TreeGenesComponent>(tree)[geneToMutate[0]].Value;

        geneToMutate.Dispose();
        directionToMutate.Dispose();
        newGene.Dispose();

        return treeGene;
    }

    [BurstCompile]
    private struct MutateJob : IJob
    {
        [ReadOnly] public int GenesCount;
        [ReadOnly] public uint Seed;
        public NativeArray<int> NewGene;
        public NativeArray<int> GeneToMutate;
        public NativeArray<int> DirectionToMutate;

        public void Execute()
        {
            var random = new Unity.Mathematics.Random(Seed);

            var randomEdge = GenesCount * 2;

            GeneToMutate[0] = random.NextInt(GenesCount);
            DirectionToMutate[0] = random.NextInt(6);
            NewGene[0] = random.NextInt(randomEdge);
        }
    }

    private static void IncreaseGeneration()
    {
        var simulationStatus = global::World.SimulationStatus;
        simulationStatus.TreeGeneration += 1;
        global::World.SimulationStatus = simulationStatus;
    }


    private void TreeEnergyCalc(ref Entity tree, ref TreeComponent treeData)
    {
        treeData.TreeEnergy = 0;

        foreach (var cell in EntityManager.GetBuffer<TreeCellsComponent>(tree))
        {
            treeData.TreeEnergy +=
                EntityManager.GetComponentData<TreeCellComponent>(cell.Value).Energy;
        }
    }

    private void ProceedTreeCells(ref Entity tree)
    {
        foreach (var cell in EntityManager.GetBuffer<TreeCellsComponent>(tree))
        {
            var cellData = EntityManager.GetComponentData<TreeCellComponent>(cell.Value);
            var translation = EntityManager.GetComponentData<Translation>(cell.Value);
            
            CellCalcEnergy(ref cellData, ref translation);

            if (cellData.Energy < 18 || !cellData.IsSeed) return;

            GrowNewCells(ref cellData, translation.Value);
        }
    }

    private void CellCalcEnergy(ref TreeCellComponent cellData, ref Translation translation)
    {
        cellData.Energy -= global::World.SimulationConstants.CellEnergyUsage;

        var sunLvl = GetSunLvl(translation.Value);

        if (sunLvl == 0) return;

        cellData.Energy += sunLvl * Mathf.RoundToInt(translation.Value.y + 6);
    }

    private int GetSunLvl(Vector3 position)
    {
        var ray = CastRay(new Vector3(0, RayLength, 0), position);

        var physicWorld = Unity.Entities.World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
        var collisionWorld = physicWorld.PhysicsWorld.CollisionWorld;

        var hittedEntities = new NativeList<RaycastHit>(Allocator.Temp);
        collisionWorld.CastRay(ray, ref hittedEntities);

        var hittedEntitiesCount = hittedEntities.Length;
        hittedEntities.Dispose();

        if (hittedEntitiesCount > 2) return 0;

        return 3 - hittedEntitiesCount;
    }

    private RaycastInput CastRay(Vector3 direction, Vector3 position)
    {
        return new RaycastInput
        {
            Start = position,
            End = position + direction,
            Filter = _groundCollisionFilter
        };
    }

    private void GrowNewCells(ref TreeCellComponent cellData, Vector3 position)
    {
        var growPairs = new (int GeneValue, Vector3 GrowDirection)[]
        {
            (cellData.Genes.Up, Vector3.up),
            (cellData.Genes.Down, Vector3.down),
            (cellData.Genes.Forward, Vector3.forward),
            (cellData.Genes.Back, Vector3.back),
            (cellData.Genes.Left, Vector3.left),
            (cellData.Genes.Right, Vector3.right)
        };

        foreach (var (geneValue, growDirection) in growPairs)
        {
            if (cellData.Energy < 18) return;
            if (geneValue >= global::World.SimulationConstants.TreeGenesCount) return;
            if (IsSpaceOccupied(growDirection, position)) return;

            cellData.Energy -= 18;

            GrowCell(ref cellData, growDirection + position, geneValue);
        }
    }

    private bool IsSpaceOccupied(Vector3 direction, Vector3 position)
    {
        var ray = CastRay(direction, position);

        var physicWorld = Unity.Entities.World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
        var collisionWorld = physicWorld.PhysicsWorld.CollisionWorld;

        return collisionWorld.CastRay(ray, out _);
    }

    private void GrowCell(ref TreeCellComponent cellData, Vector3 newCellPosition, int geneValue)
    {
        if (newCellPosition.y < 0) return;

        var entityManager = Unity.Entities.World.DefaultGameObjectInjectionWorld.EntityManager;

        var newCell = entityManager.Instantiate(global::World.SimulationResources.TreeCellPrefab);

        entityManager.AddComponentData(newCell, new TreeCellTag());

        entityManager.GetBuffer<TreeCellsComponent>(cellData.ParentTree).Add(newCell);

        entityManager.SetComponentData(newCell, new Translation
        {
            Value = newCellPosition
        });

        entityManager.SetComponentData(newCell, new TreeCellComponent
        {
            ParentTree = cellData.ParentTree,
            IsSeed = true,
            Genes = entityManager.GetBuffer<TreeGenesComponent>(cellData.ParentTree)[geneValue]
        });

        cellData.IsSeed = false;
        //gameObject.GetComponent<Renderer>().material = _woodMaterial;
    }
}