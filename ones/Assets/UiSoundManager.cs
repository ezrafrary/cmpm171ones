using Rive;
using Rive.Components;
using UnityEngine;


public class UiSoundManager : MonoBehaviour
{
    public AudioSource uiSoundSource;
    public AudioClip[] allUiSFX;


    //rive
    [SerializeField] private RiveWidget m_riveWidget;













    private void OnEnable()
    {
        if (m_riveWidget)
        {
            m_riveWidget.OnRiveEventReported += OnRiveEventReported;
        }
    }


    private void OnRiveEventReported(ReportedEvent evt)
    {
        if (evt.Name.StartsWith("HoverEvent"))
        {
            playUiSFX(0);
        }
    }




    private void OnDisable()
    {
        if (m_riveWidget)
        {
            m_riveWidget.OnRiveEventReported -= OnRiveEventReported;
        }
    }



    private float GetMasterVolume()
    {
        return PlayerPrefs.GetInt("MasterVolume", 100) / 100f;
    }

    public void playUiSFX(int soundIndex)
    {
        uiSoundSource.clip = allUiSFX[soundIndex];
        uiSoundSource.volume = 0.2f * GetMasterVolume();
        uiSoundSource.Play();
    }

}
