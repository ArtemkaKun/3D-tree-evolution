using Components.World;
using Unity.Entities;
using UnityEngine;

namespace Systems.World
{
    public class WorldStartDataHandler : MonoBehaviour
    {
        [SerializeField] private GameObject treePrefab;
        [SerializeField] private GameObject treeCellPrefab;
        [SerializeField] private Material treeMaterial;
        [SerializeField] private Material seedMaterial;

        private BlobAssetStore _blobAssetStore;

        public void InitializeResources()
        {
            _blobAssetStore = new BlobAssetStore();
            var (treeEntity, treeCellEntity) = ConvertPrefabsIntoEntities();
            global::World.SimulationResources = new SimulationResources(treeEntity, treeCellEntity, treeMaterial, seedMaterial);
        }
        
        private (Entity treePrefab, Entity treeCellPrefab) ConvertPrefabsIntoEntities()
        {
            return (
                GameObjectConversionUtility.ConvertGameObjectHierarchy(treePrefab,
                    GameObjectConversionSettings.FromWorld(Unity.Entities.World.DefaultGameObjectInjectionWorld,
                        _blobAssetStore)),
                GameObjectConversionUtility.ConvertGameObjectHierarchy(treeCellPrefab,
                    GameObjectConversionSettings.FromWorld(Unity.Entities.World.DefaultGameObjectInjectionWorld,
                        _blobAssetStore)));
        }

        private void OnDestroy()
        {
            _blobAssetStore.Dispose();
        }
    }
}