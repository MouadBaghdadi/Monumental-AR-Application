using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageToggle : MonoBehaviour
{
    public void ToggleLanguage()
    {
        LanguageManager.CycleLanguage();
    }
}