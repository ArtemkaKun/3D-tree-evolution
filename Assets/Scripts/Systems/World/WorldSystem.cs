using System;
using Components.Tree;
using Components.TreeCell;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Random = System.Random;

[AlwaysUpdateSystem]
public class WorldSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        if (!global::World.SimulationStatus.IsSimulationRun) return;
        if (global::World.SimulationStatus.WorldAge >= 10000) return;
        
        ProceedTrees();

        global::World.IncreaseWorldAge();
    }

    private void ProceedTrees()
    {
        Entities.ForEach((Entity tree, ref TreeTag _, ref TreeComponent treeData) =>
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
            
            //     if (buffer_tree.Count > 0)
            //     {
            //         await Task.WhenAll(buffer_tree.Select(x => x.GetComponent<CellController>().CellMainLoop())
            //             .ToArray());
            //     }
            // }
        });
    }

    private void DestroyTree(ref Entity tree)
    {
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
        
            EntityManager.AddComponentData(cell.Value, new DeleteTag());
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
                EntityManager.GetComponentData<TreeCellComponent>(cell.Value)
                    .Energy; //TODO Set actual energy calculation!
        }
    }
}