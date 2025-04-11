using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Unity.Collections;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;


[RequireComponent(typeof(Camera))]
public class ReplaySystem : MonoBehaviour
{
    private Camera thisCam;

    [Header("Recording Settings")]
    public float delayBetweenFrames = 0.05f;
    public int resolutionMultiplier = 1;
    public float maxRecordingTime = 6f;

    [Header("UI Reference")]
    public RawImage playbackImage;

    private RenderTexture renderTexture;
    private ReplayClip currentClip;

    private bool IsRecording = false; //not used i guess
    private bool IsPlaying = false;
    private float captureTimer = 0f;
    private int maxFrameCount;

    void Start()
    {
        thisCam = GetComponent<Camera>();

        int width = Screen.width * resolutionMultiplier;
        int height = Screen.height * resolutionMultiplier;

        renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        renderTexture.Create();
        thisCam.targetTexture = renderTexture;

        if (playbackImage != null)
            playbackImage.gameObject.SetActive(false);

        maxFrameCount = Mathf.CeilToInt(maxRecordingTime / delayBetweenFrames);
        StartRecording();
    }






    public void clipThat(){
        StopRecording();
        StartRecording();
    }


    IEnumerator WaitAndDoSomething()
    {
        yield return new WaitForSeconds(2f);
         clipThat();
    }

    public void clipThatWait2(){
         StartCoroutine(WaitAndDoSomething());
    }


    void LateUpdate()
    {
        if (IsRecording)
        {
            captureTimer += Time.deltaTime;
            if (captureTimer >= delayBetweenFrames)
            {
                captureTimer = 0f;
                StartCoroutine(CaptureFrameAsync());
            }
        }
    }

    IEnumerator CaptureFrameAsync()
    {
        yield return new WaitForEndOfFrame();
        AsyncGPUReadback.Request(renderTexture, 0, TextureFormat.RGB24, OnFrameCaptured);
    }

    void OnFrameCaptured(AsyncGPUReadbackRequest request)
    {
        if (request.hasError)
        {
            Debug.LogWarning("GPU readback error!");
            return;
        }

        //NativeArray<byte> data = new NativeArray<byte>(request.GetData<byte>().ToArray(), Allocator.Persistent); <- causes memory leak
        // NativeArray<byte> original = request.GetData<byte>();
        // NativeArray<byte> data = new NativeArray<byte>(original, Allocator.Persistent);

        NativeArray<byte> original = request.GetData<byte>();
        byte[] data = original.ToArray(); // This is safe and managed
    
        if (currentClip.rawFrames.Count >= maxFrameCount)
        {
            currentClip.rawFrames.RemoveAt(0);
        }

        currentClip.rawFrames.Add(data);
    }

    void StartRecording()
    {
        StopRecording(); // Ensure cleanup of previous clip

        currentClip = new ReplayClip(renderTexture.width, renderTexture.height, TextureFormat.RGB24);
        IsRecording = true;
        captureTimer = 0f;
        Debug.Log("Recording started.");
    }

    void StopRecording()
    {
        if (IsRecording && currentClip != null)
        {
            var globalReplaySystem = FindFirstObjectByType<ClipHolder>();

            Debug.Log("ClipHolder found: " + globalReplaySystem); //tester
            if(globalReplaySystem){
                globalReplaySystem.savedReplays.Add(currentClip);
                //Debug.Log(globalReplaySystem.savedReplays.Count);
            }else{
                Debug.Log("no global replay system");
            }
            Debug.Log($"Recording stopped. Saved frames: {currentClip.rawFrames.Count}");
        }

        IsRecording = false;
    }

    // public void PlaySavedReplay(int index)
    // {
    //     if (index >= 0 && index < savedReplays.Count && !IsPlaying)
    //     {
    //         //StopRecording();
    //         StartCoroutine(Playback(savedReplays[index]));
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Invalid replay index or already playing.");
    //     }
    // }

    IEnumerator Playback(ReplayClip clip)
    {
        IsPlaying = true;

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
        }

        if (playbackImage != null)
            playbackImage.gameObject.SetActive(false);

        IsPlaying = false;
        //StartRecording();
    }

    void DisplayFrame(Texture2D frame)
    {
        if (playbackImage != null)
        {
            playbackImage.texture = frame;
        }
    }

    void OnDestroy()
    {
        renderTexture.Release();
        
    }
    public void clearCurrentClip()
    {
        if (currentClip != null)
        {
            currentClip.Dispose();
            currentClip = null;
        }
    }

}
