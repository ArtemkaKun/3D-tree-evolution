using System;
using Systems.World;
using Components.World;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class World : MonoBehaviour
{
    public static int GroundDimensions { get; set; }
    public static SimulationStatus SimulationStatus { get; set; }
    public static SimulationConstants SimulationConstants { get; set; }
    public static SimulationResources SimulationResources { get; set; }
    
    public static Action OnWorldAgeChange;
    public static Action OnTreeGenerationChange;
    public static Action OnForestSizeChange;
    
    [SerializeField] private int worldYearInSeconds;

    public void StartSimulation()
    {
        transform.localScale = new Vector3(GroundDimensions, 1, GroundDimensions);

        SpawnStartForest();
        
        OnWorldAgeChange?.Invoke();
        
        MainLoop();
    }

    private void SpawnStartForest()
    {
        for (var i = 0; i < SimulationConstants.StartForestSize; i++)
        {
            TreeSpawner();
        }
        
        OnTreeGenerationChange?.Invoke();
        OnForestSizeChange?.Invoke();
    }

    private static void TreeSpawner()
    {
        var randomPos = Random.insideUnitSphere * GroundDimensions / 2;
        randomPos = new Vector3(Mathf.Round(randomPos.x), 0, Mathf.Round(randomPos.z));

        var entityManager = Unity.Entities.World.DefaultGameObjectInjectionWorld.EntityManager;

        var newTree = entityManager.Instantiate(SimulationResources.TreePrefab);

        entityManager.SetComponentData(newTree, new Translation
        {
            Value = randomPos
        });
    }

    private void MainLoop()
    {
        var timeBuffer = 0f;
        while (SimulationStatus.WorldAge < 10000)
        {
            timeBuffer += Time.deltaTime;

            if (!SimulationStatus.IsSimulationRun || timeBuffer < worldYearInSeconds) return;

            timeBuffer = 0f;

            var simulationStatus = SimulationStatus;
            ++simulationStatus.WorldAge;
            SimulationStatus = simulationStatus;
            
            OnWorldAgeChange?.Invoke();
        }
    }

    private void Awake()
    {
        GetComponent<WorldStartDataHandler>().InitializeResources();
    }
}