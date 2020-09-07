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

    void Update()
    {
        SetGenerationStatusLabels();
    }

    private void SetGenerationStatusLabels()
    {
        yearsText.SetText("Year " + WorldController.GetYear());
        generationText.SetText("Generation " + WorldController.GetGeneration());
        countText.SetText("Trees " + WorldController.GetForestSize());
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
        World.SimulationStatus = new SimulationStatus(default, default, true);
        World.SimulationConstants = new SimulationConstants (
            int.Parse(treeMaxAge.text),
            int.Parse(startTreeEnergy.text),
            int.Parse(treeGenesCount.text),
            int.Parse(cellEnergyUsage.text),
            int.Parse(startForestSize.text)
            );
    }
}