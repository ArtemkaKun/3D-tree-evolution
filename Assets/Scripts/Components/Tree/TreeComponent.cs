using Unity.Entities;

namespace Components.Tree
{
    [GenerateAuthoringComponent]
    public struct TreeComponent : IComponentData
    {
        public int TreeAge;
        public int TreeEnergy;
    }
}