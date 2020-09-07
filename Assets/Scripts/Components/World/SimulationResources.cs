using Unity.Entities;
using UnityEngine;

namespace Components.World
{
    public struct SimulationResources
    {
        public Entity TreePrefab { get; set; }
        public Entity TreeCellPrefab { get; set; }
        public Material TreeMaterial { get; set; }
        public Material SeedMaterial { get; set; }

        public SimulationResources(Entity treePrefab, Entity treeCellPrefab, Material treeMaterial, Material seedMaterial)
        {
            TreePrefab = treePrefab;
            TreeCellPrefab = treeCellPrefab;
            TreeMaterial = treeMaterial;
            SeedMaterial = seedMaterial;
        }
    }
}