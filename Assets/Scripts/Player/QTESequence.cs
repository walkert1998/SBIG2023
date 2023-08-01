using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class QTESequence : MonoBehaviour
{
    public List<QTE> quickTimeEvents;

    public void PlayPrompt(int index)
    {
        Sprite spriteToAdd;
        switch (quickTimeEvents[index].action.name)
        {
            case "upArrow":
                spriteToAdd = QTEController.instance.upArrowSprite;
            break;
            case "downArrow":
                spriteToAdd = QTEController.instance.downarrowSprite;
            break;
            case "rightArrow":
                spriteToAdd = QTEController.instance.rightArrowSprite;
            break;
            case "leftArrow":
                spriteToAdd = QTEController.instance.leftArrowSprite;
            break;
            default:
                spriteToAdd = QTEController.instance.spaceBarSprite;
            break;
        }
        // QTEController.instance.qtePrompt.PopulateQTEPrompt(quickTimeEvents[index].action, spriteToAdd, quickTimeEvents[index].time);
    }

    IEnumerator QTEPlay(int index)
    {
        float timer = 0;
        while (timer < quickTimeEvents[index].time && !QTEController.instance.qtePrompt.succeeded)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        if (!QTEController.instance.qtePrompt.succeeded)
        {

        }
    }
}
