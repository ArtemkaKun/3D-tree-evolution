using System;
using Systems.Tree;
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
            Systems.Tree.TreeController.InitializeNewTree();
        }
        
        OnTreeGenerationChange?.Invoke();
        OnForestSizeChange?.Invoke();
    }

    private void Awake()
    {
        GetComponent<WorldStartDataHandler>().InitializeResources();
        
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