using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Play();
            Debug.Log("Video started!");
        }
        else
        {
            Debug.LogError("VideoPlayer not assigned!");
        }
    }
}
