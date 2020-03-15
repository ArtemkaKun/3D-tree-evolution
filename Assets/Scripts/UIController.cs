using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _yearsText;
    [SerializeField] private TextMeshProUGUI _generationText;
    [SerializeField] private TextMeshProUGUI _countText;
    void Update()
    {
        _yearsText.SetText("Year " + WorldController.GetYear());
        _generationText.SetText("Generation " + WorldController.GetGeneration());
        _countText.SetText("Trees " + WorldController.GetForestSize());
    }
}
