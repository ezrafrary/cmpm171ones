using UnityEngine;
using UnityEngine.EventSystems;

public class PanelFollowMouse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform panel; // assign the panel to follow
    private bool isHovering = false;

    void Update()
    {
        if (isHovering && panel != null)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                panel.parent as RectTransform,
                Input.mousePosition,
                null,
                out pos
            );
            panel.anchoredPosition = pos;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        panel.gameObject.SetActive(true); // Optional: show the panel on hover
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        panel.gameObject.SetActive(false); // Optional: hide the panel when not hovering
    }
}
