using UnityEngine;
using UnityEngine.Playables;

public class WinScreen : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayableDirector timeline;
    
    public float TimelineDuration => timeline ? (float)timeline.duration : 0f;

    public void PlayWinScreen()
    {
        timeline.Play();
    }
}