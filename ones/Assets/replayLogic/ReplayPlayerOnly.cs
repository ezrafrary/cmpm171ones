using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;
using System.Collections;

public class ReplayPlayerOnly : MonoBehaviour
{
    [Header("Playback Settings")]
    public float delayBetweenFrames = 0.05f;
    public RawImage playbackImage;

    private bool isPlaying = false;
    private Coroutine playbackCoroutine = null;

    public void PlayClip(ReplayClip clip)
    {
        if (!isPlaying && clip != null)
        {
            playbackCoroutine = StartCoroutine(Playback(clip));
        }
        else
        {
            Debug.LogWarning("Cannot play: either already playing or clip is null.");
        }
    }

    private IEnumerator Playback(ReplayClip clip)
    {
        isPlaying = true;

        if (playbackImage != null)
            playbackImage.gameObject.SetActive(true);

        foreach (var rawFrame in clip.rawFrames)
        {
            Texture2D tex = new Texture2D(clip.width, clip.height, clip.format, false);
            tex.LoadRawTextureData(rawFrame);
            tex.Apply();
            DisplayFrame(tex);
            yield return new WaitForSeconds(delayBetweenFrames);
            Destroy(tex);

            if (!isPlaying) // If stopped, break out of the loop early
                break;
        }

        if (playbackImage != null)
            playbackImage.gameObject.SetActive(false);

        isPlaying = false;
        playbackCoroutine = null;
    }

    private void DisplayFrame(Texture2D frame)
    {
        if (playbackImage != null)
        {
            playbackImage.texture = frame;
        }
        else
        {
            Debug.Log("PlaybackImageNull");
        }
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }

    public void StopPlayback()
    {
        if (isPlaying && playbackCoroutine != null)
        {
            StopCoroutine(playbackCoroutine);
            playbackCoroutine = null;
            isPlaying = false;

            if (playbackImage != null)
            {
                playbackImage.texture = null;
                playbackImage.gameObject.SetActive(false);
            }
        }
    }
}
