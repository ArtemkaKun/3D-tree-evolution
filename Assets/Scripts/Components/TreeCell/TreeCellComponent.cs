using Components.Tree;
using Unity.Entities;

namespace Components.TreeCell
{
    public struct TreeCellComponent : IComponentData
    {
        public bool isSeed;
        public int energy;
        public GrowGenes genes;
    }
}