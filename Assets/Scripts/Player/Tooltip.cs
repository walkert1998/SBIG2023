using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    private static Tooltip instance;

    private TMP_Text toolTipText;
    private RectTransform toolTipBackgroundRect;
    [SerializeField]
    private Camera uiCamera;
    protected Vector3[] corners;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        corners = new Vector3[4];
        //toolTipBackgroundRect = transform.Find("ToolTipBackground").GetComponent<RectTransform>();
        toolTipText = transform.Find("ToolTipText").GetComponent<TMP_Text>();
        instance.HideToolTip();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    Vector2 localPoint;
    //    RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), new Vector3(Mouse.current.position.ReadValue().x + 40f, Mouse.current.position.ReadValue().y - 40f, 0), uiCamera, out localPoint);
    //    transform.localPosition = localPoint;
    //}

    protected void LateUpdate()
    {
        var pos = new Vector2(Mouse.current.position.ReadValue().x + 20, Mouse.current.position.ReadValue().y - 20);

        ((RectTransform)transform).GetWorldCorners(corners);
        var width = corners[2].x - corners[0].x;
        var height = corners[1].y - corners[0].y;

        var distPastX = pos.x + width - Screen.width;
        if (distPastX > 0)
            pos = new Vector2(pos.x - distPastX, pos.y);
        var distPastY = pos.y - height;
        if (distPastY < 0)
            pos = new Vector2(pos.x, pos.y - distPastY);

        transform.position = pos;
    }

    public void DisplayToolTip(string ToolTipText)
    {
        toolTipText.text = ToolTipText;
        //float textPadding = 4f;
        //Vector2 backgroundSize = new Vector2(toolTipText.preferredWidth + textPadding * 2f, toolTipText.preferredHeight + textPadding * 2f);
        //toolTipBackgroundRect.sizeDelta = backgroundSize;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), new Vector3(Mouse.current.position.ReadValue().x + 20, Mouse.current.position.ReadValue().y - 20, 0), uiCamera, out localPoint);
        transform.localPosition = localPoint;
        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        gameObject.SetActive(false);
        toolTipText.text = "";
    }

    public static void DisplayToolTip_Static(string ToolTip)
    {
        instance.DisplayToolTip(ToolTip);
    }

    public static void HideToolTip_Static()
    {
        //Debug.Log("Hiding tooltip");
        instance.HideToolTip();
    }
}
