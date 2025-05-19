using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LanguageManager
{
    public enum Language { English, French, Arabic /* Add more as needed */ }

    private static Language current = Language.English;
    public static Language CurrentLanguage
    {
        get { return current; }
        set
        {
            if (current != value)
            {
                current = value;
                OnLanguageChanged?.Invoke(current);
            }
        }
    }

    public static event System.Action<Language> OnLanguageChanged;

    public static void CycleLanguage()
    {
        int next = ((int)current + 1) % System.Enum.GetValues(typeof(Language)).Length;
        CurrentLanguage = (Language)next;
    }
}