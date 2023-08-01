using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QTEPrompt : MonoBehaviour
{
    public InputAction promptKey;
    public Image promptImage;
    public Slider QTETimerSlider;
    public bool mashKey = false;
    public float timeToPress = 3.0f;
    private float timer = 0;
    public bool succeeded = false;

    // Start is called before the first frame update
    // void Start()
    // {

    // }

    void OnEnable()
    {
        promptKey.started += PressKey;
        promptKey.Enable();
    }

    void OnDisable()
    {
        promptKey.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < timeToPress)
        {
            timer += Time.deltaTime;
            QTETimerSlider.value = timeToPress - timer;
        }
    }

    public void PopulateQTEPrompt(InputAction keyToPrompt, float time)
    {
        Sprite spriteToAdd;
        switch (keyToPrompt.name)
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
        promptKey = keyToPrompt;
        promptImage.sprite = spriteToAdd;
        gameObject.SetActive(true);
        timeToPress = time;
        timer = 0;
        QTETimerSlider.maxValue = timeToPress;
        QTETimerSlider.value = timeToPress;
    }

    public void PressKey(InputAction.CallbackContext context)
    {
        succeeded = true;
        timer = 0;
        gameObject.SetActive(false);
    }
}
