using Components.Tree;
using Unity.Entities;

namespace Components.TreeCell
{
    [GenerateAuthoringComponent]
    public struct TreeCellComponent : IComponentData
    {
        public bool IsSeed;
        public int Energy;
        public GrowGenes Genes;
    }
}