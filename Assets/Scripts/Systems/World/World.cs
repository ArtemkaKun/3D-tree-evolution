using System;
using Systems.World;
using Components.World;
using UniRx;
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

    public static void IncreaseWorldAge()
    {
        var simulationStatus = SimulationStatus;
        ++simulationStatus.WorldAge;
        SimulationStatus = simulationStatus;
            
        OnWorldAgeChange?.Invoke();
    }
    
    public void StartSimulation()
    {
        transform.localScale = new Vector3(GroundDimensions, 1, GroundDimensions);

        SpawnStartForest();
        
        OnWorldAgeChange?.Invoke();
        
        ToggleSimulationStatus();
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

    private void Awake()
    {
        var test = GetComponent<WorldStartDataHandler>();
        test.InitializeResources();
        
        InitializeSimulationControllers();
    }

    private void InitializeSimulationControllers()
    {
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Space))
            .Subscribe(_ => ToggleSimulationStatus());
    }

    private void ToggleSimulationStatus()
    {
        var simulationStatus = SimulationStatus;
        simulationStatus.IsSimulationRun = !simulationStatus.IsSimulationRun;
        SimulationStatus = simulationStatus;
    }
}