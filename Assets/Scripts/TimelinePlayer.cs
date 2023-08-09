using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.Playables;

public class TimelinePlayer : MonoBehaviour
{
    public static TimelinePlayer instance;
    public List<PlayableDirector> playableDirector;
    public ThirdPersonController playerController;
    PlayableDirector currentPlayable;
    public GameObject jaxxon;
    public GameObject vernon;
    public GameObject alien;
    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
        currentPlayable = playableDirector[0];
    }
    void Update()
    {
        if (currentPlayable != null && currentPlayable.gameObject.activeSelf && currentPlayable.time >= currentPlayable.duration)
        {
            playerController.LockMovement(true);
            PlayerInteraction.UnlockInteraction();
            currentPlayable.gameObject.SetActive(false);
        }
    }

    public static void PlayTimeline_Static(int index)
    {
        Debug.Log("Play timeline number " + index);
        if (index == 0)
        {
            instance.jaxxon.SetActive(true);
            instance.vernon.SetActive(true);
            instance.alien.SetActive(true);
        }
        PlayerInteraction.LockInteraction();
        instance.playerController.LockMovement(false);
        instance.currentPlayable = instance.playableDirector[index];
        instance.currentPlayable.gameObject.SetActive(true);
        instance.currentPlayable.Play();
    }
}
