using System;
using Components.World;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI yearsText;
    [SerializeField] private TextMeshProUGUI generationText;
    [SerializeField] private TextMeshProUGUI countText;

    [SerializeField] private TMP_InputField groundSize;
    [SerializeField] private TMP_InputField treeMaxAge;
    [SerializeField] private TMP_InputField startTreeEnergy;
    [SerializeField] private TMP_InputField treeGenesCount;
    [SerializeField] private TMP_InputField cellEnergyUsage;
    [SerializeField] private TMP_InputField startForestSize;

    [SerializeField] private World world;

    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject ingameCanvas;

    private void Awake()
    {
        World.OnWorldAgeChange += UpdateYearText;
        World.OnTreeGenerationChange += UpdateGenerationText;
        World.OnForestSizeChange += UpdateForestSizeText;
    }

    private void UpdateYearText()
    {
        yearsText.SetText("Year " + World.SimulationStatus.WorldAge);
    }
    
    private void UpdateGenerationText()
    {
        generationText.SetText("Generation " + World.SimulationStatus.TreeGeneration);
    }
    
    private void UpdateForestSizeText()
    {
        countText.SetText("Trees " + World.SimulationStatus.ForestSize);
    }

    public void OnStartClick()
    {
        SetWorldParams();

        startCanvas.SetActive(false);
        ingameCanvas.SetActive(true);
        world.GetComponent<World>().StartSimulation();
    }

    private void SetWorldParams()
    {
        World.GroundDimensions = int.Parse(groundSize.text);
        
        var startForestSizeValue = int.Parse(startForestSize.text);
        World.SimulationStatus = new SimulationStatus(1, 1, startForestSizeValue,false);
        
        World.SimulationConstants = new SimulationConstants (
            int.Parse(treeMaxAge.text),
            int.Parse(startTreeEnergy.text),
            int.Parse(treeGenesCount.text),
            int.Parse(cellEnergyUsage.text),
            startForestSizeValue
            );
    }
}