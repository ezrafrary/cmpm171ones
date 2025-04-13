using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Unity.Collections;
using System.Collections;
using System.Collections.Generic;


public class ClipHolder : MonoBehaviour
{
    public List<ReplayClip> savedReplays = new List<ReplayClip>();
    

    public ReplayPlayerOnly replayObj;
    public int clipIndexToPlay = 0;


    public void playClip(int replayID){
        replayObj.PlayClip(savedReplays[replayID]);
    }

    public void DisposeAllReplays()
    {
        foreach (var clip in savedReplays)
        {
            clip.Dispose();
        }
        savedReplays.Clear();
    }

    void OnDestroy(){
        DisposeAllReplays();
    }


}
