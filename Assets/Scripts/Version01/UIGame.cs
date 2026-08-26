using UnityEngine;
using TMPro;

public class UIGame : MonoBehaviour
{
    [SerializeField] private TMP_Text waveText;

    void Start()
    {
        waveText ??= GetComponentInChildren<TMP_Text>(true);
        if (waveText != null) waveText.text = $"WAVE {Exit.WaveNumber}";
    }
}
