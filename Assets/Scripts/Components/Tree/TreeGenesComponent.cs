using Unity.Entities;

namespace Components.Tree
{
    public struct TreeGenesComponent : IBufferElementData
    {
        public static implicit operator GrowGenes(TreeGenesComponent e)
        {
            return e.Value;
        }

        public static implicit operator TreeGenesComponent(GrowGenes e)
        {
            return new TreeGenesComponent { Value = e };
        }

        public GrowGenes Value;
    }
}