using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static LanguageManager;
using ArabicSupport; // Assuming you have a namespace for Arabic support

public class LanguageResponsive : MonoBehaviour
{
    [TextArea] public string englishText;
    [TextArea] public string frenchText;
    [TextArea] public string arabicText;

    private Text uiText;

    private void Awake()
    {
        uiText = GetComponent<Text>();
    }

    private void OnEnable()
    {
        LanguageManager.OnLanguageChanged += ApplyLanguage;
        ApplyLanguage(LanguageManager.CurrentLanguage);
    }

    private void OnDisable()
    {
        LanguageManager.OnLanguageChanged -= ApplyLanguage;
    }

    private void ApplyLanguage(Language lang)
    {
        if (uiText == null) return;

        switch (lang)
        {
            case Language.English:
                uiText.text = englishText;
                break;
            case Language.French:
                uiText.text = frenchText;
                break;
            case Language.Arabic:
                uiText.text = ArabicFixer.Fix(arabicText, showTashkeel: true, useHinduNumbers: false);
                break;
        }
    }
}
