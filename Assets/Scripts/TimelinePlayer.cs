using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelinePlayer : MonoBehaviour
{
    public static TimelinePlayer instance;
    public List<PlayableDirector> playableDirector;
    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
    }

    public static void PlayTimeline_Static(int index)
    {
        instance.playableDirector[index].gameObject.SetActive(true);
        instance.playableDirector[index].Play();
    }
}
