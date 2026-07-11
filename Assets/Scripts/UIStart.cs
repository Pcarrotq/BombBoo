using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStart : MonoBehaviour
{
    [SerializeField] private GameObject diffModeSetting;
    [SerializeField] private TMP_Dropdown diffDropdown;
    [SerializeField] private TMP_Dropdown  modeDropdown;
    
    public int diffIndex;
    public int modeIndex;

    void Start()
    {
        diffDropdown.captionText.text = "";
        modeDropdown.captionText.text = "";
    }

    public void SetDiffModeSettingOpen()
    {
        diffModeSetting.SetActive(true);
    }

    public void ChangeDifficulty(int index)
    {
        diffIndex = diffDropdown.value;
        Debug.Log("diffIndex = " + diffIndex);
    }

    public void ChangeGameMode(int index)
    {
        modeIndex = modeDropdown.value;
        Debug.Log("modeIndex = " + modeIndex);
    }
}
