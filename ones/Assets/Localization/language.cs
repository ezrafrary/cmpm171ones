using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageSwitcher : MonoBehaviour
{
    // Call this method via the button's OnClick event and pass the locale code you want to switch to
    public void SwitchLanguage(string localeCode)
    {
        StartCoroutine(SetLocale(localeCode));
    }

    private IEnumerator SetLocale(string localeCode)
    {
        // Wait until the Localization system is initialized
        yield return LocalizationSettings.InitializationOperation;

        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            if (locale.Identifier.Code == localeCode)
            {
                LocalizationSettings.SelectedLocale = locale;
                break;
            }
        }
    }
}
