using System;
using Components.Tree;
using Components.TreeCell;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Random = System.Random;

namespace Systems.Tree
{
    public static class TreeController
    {
        public static void InitializeNewTree()
        {
            var tree = SpawnTreeEntity();

            GenerateTreeGenome(ref tree);
            
            SpawnTreeRoot(ref tree);
        }

        private static Entity SpawnTreeEntity()
        {
            var randomPos = GetRandomPosition();

            var entityManager = Unity.Entities.World.DefaultGameObjectInjectionWorld.EntityManager;

            var newTree = entityManager.Instantiate(global::World.SimulationResources.TreePrefab);
            
            entityManager.AddComponentData(newTree, new TreeTag());
            
            entityManager.SetComponentData(newTree, new Translation
            {
                Value = randomPos
            });
            
            entityManager.AddBuffer<TreeGenesComponent>(newTree);
            entityManager.AddBuffer<TreeCellsComponent>(newTree);
            
            return newTree;
        }

        private static Vector3 GetRandomPosition()
        {
            const int groundBorderOffset = 20;
            
            var randomPos = UnityEngine.Random.insideUnitSphere * (global::World.GroundDimensions - groundBorderOffset) / 2;
            randomPos = new Vector3(Mathf.Round(randomPos.x), 0, Mathf.Round(randomPos.z));
            
            return randomPos;
        }

        private static void GenerateTreeGenome(ref Entity tree)
        {
            var time = DateTime.Now;
            var random = new Random(time.Hour + time.Minute + time.Second + time.Millisecond + tree.Index);

            var maxRandomEdge = global::World.SimulationConstants.TreeGenesCount * 2;
            
            var genes = Unity.Entities.World.DefaultGameObjectInjectionWorld.EntityManager.GetBuffer<TreeGenesComponent>(tree);
            
            for (var gene = 0; gene < global::World.SimulationConstants.TreeGenesCount; ++gene)
            {
                var randGene = new GrowGenes(
                    random.Next(maxRandomEdge), random.Next(maxRandomEdge), random.Next(maxRandomEdge),
                    random.Next(maxRandomEdge), random.Next(maxRandomEdge), random.Next(maxRandomEdge)
                );

                genes.Add(randGene);
            }
        }
        
        private static void SpawnTreeRoot(ref Entity tree)
        {
            var entityManager = Unity.Entities.World.DefaultGameObjectInjectionWorld.EntityManager;
            
            var root = entityManager.Instantiate(global::World.SimulationResources.TreeCellPrefab);
            
            entityManager.AddComponentData(root, new TreeCellTag());
            
            entityManager.GetBuffer<TreeCellsComponent>(tree).Add(root);
            
            entityManager.SetComponentData(root, new Translation
            {
                Value = entityManager.GetComponentData<Translation>(tree).Value
            });

            entityManager.SetComponentData(root, new TreeCellComponent
            {
                ParentTree = tree,
                IsSeed = true,
                Energy = global::World.SimulationConstants.StartSeedEnergy,
                Genes = entityManager.GetBuffer<TreeGenesComponent>(tree)[0]
            });
        }
    }
}