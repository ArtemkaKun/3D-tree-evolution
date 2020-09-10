using Unity.Entities;

namespace Components.Tree
{
    public struct TreeCellsComponent : IBufferElementData
    {
        public static implicit operator Entity (TreeCellsComponent e)
        {
            return e.Value;
        }

        public static implicit operator TreeCellsComponent (Entity e)
        {
            return new TreeCellsComponent { Value = e };
        }
        
        public Entity Value;
    }
}