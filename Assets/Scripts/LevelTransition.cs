using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class LevelTransition : MonoBehaviour
{
    public string levelName;
    public VideoPlayer player;
	// Use this for initialization
	void Start () {
        if (player != null)
            StartCoroutine(PlayVideo());
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void ChangeLevelTo(string scene)
    {
        //Debug.Log(player.isPlaying);
        SceneManager.LoadScene("Scenes/" + scene);
    }

    IEnumerator PlayVideo()
    {
        yield return new WaitForSeconds((float)player.clip.length);
        ChangeLevelTo(levelName);
    }

    public string scene_name
    {
        get
        {
            return levelName;
        }
    }

    //private void OnMouseEnter()
    //{
    //    PlayerInteraction.SetPrompt("[E] Travel To " + levelName);
    //}

    //private void OnMouseExit()
    //{
    //    PlayerInteraction.SetPrompt("");
    //}
}
