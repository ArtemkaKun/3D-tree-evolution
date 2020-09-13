using Components.Tree;
using Unity.Entities;

namespace Components.TreeCell
{
    [GenerateAuthoringComponent]
    public struct TreeCellComponent : IComponentData
    {
        public Entity ParentTree;
        public bool IsSeed;
        public int Energy;
        public GrowGenes Genes;
    }
}