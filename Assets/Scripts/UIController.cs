using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _yearsText;
    [SerializeField] private TextMeshProUGUI _generationText;
    [SerializeField] private TextMeshProUGUI _countText;

    [SerializeField] private TMP_InputField _coordX;
    [SerializeField] private TMP_InputField _coordZ;
    [SerializeField] private TMP_InputField _treeAge;
    [SerializeField] private TMP_InputField _startEnergy;
    [SerializeField] private TMP_InputField _genesCount;
    [SerializeField] private TMP_InputField _cellUsage;
    [SerializeField] private TMP_InputField _startForestSize;

    [SerializeField] private GameObject _worldController;

    [SerializeField] private GameObject _startCanvas;
    [SerializeField] private GameObject _ingameCanvas;

    void Update()
    {
        SetGenerationStatusLabels();
    }

    private void SetGenerationStatusLabels()
    {
        _yearsText.SetText("Year " + WorldController.GetYear());
        _generationText.SetText("Generation " + WorldController.GetGeneration());
        _countText.SetText("Trees " + WorldController.GetForestSize());
    }

    public void OnStartClick()
    {
        if (_coordX.text != "" && _coordZ.text != "" && _treeAge.text != "" && _startEnergy.text != "" &&
            _genesCount.text != "" && _cellUsage.text != "" && _startForestSize.text != "" &&
            int.Parse(_coordX.text) != 0 && int.Parse(_coordZ.text) != 0 && int.Parse(_treeAge.text) != 0 &&
            int.Parse(_startEnergy.text) != 0 && int.Parse(_genesCount.text) != 0 && int.Parse(_cellUsage.text) != 0 &&
            int.Parse(_startForestSize.text) != 0)
        {
            SetWorldParams();

            _startCanvas.SetActive(false);
            _ingameCanvas.SetActive(true);
            _worldController.GetComponent<WorldController>().StartWorld();
        }
    }

    private void SetWorldParams()
    {
        WorldController.SetGroundLength(int.Parse(_coordX.text));
        WorldController.SetGroundWidth(int.Parse(_coordZ.text));
        WorldController.SetMaxAge(int.Parse(_treeAge.text));
        WorldController.SetGenesCount(int.Parse(_genesCount.text));
        WorldController.SetStartEnergy(int.Parse(_startEnergy.text));
        WorldController.SetCellUsage(int.Parse(_cellUsage.text));
        WorldController.SetStartForestSize(int.Parse(_startForestSize.text));
    }
}