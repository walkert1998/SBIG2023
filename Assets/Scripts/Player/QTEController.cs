using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEController : MonoBehaviour
{
    public static QTEController instance;
    public QTEPrompt qtePrompt;
    public Sprite upArrowSprite;
    public Sprite downarrowSprite;
    public Sprite leftArrowSprite;
    public Sprite rightArrowSprite;
    public Sprite spaceBarSprite;
    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
    }

    public static void ShowQTE(QTE qte)
    {
        instance.qtePrompt.PopulateQTEPrompt(qte.action, qte.time);
    }
}
