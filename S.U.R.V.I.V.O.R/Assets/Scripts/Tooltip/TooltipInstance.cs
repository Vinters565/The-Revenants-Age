using System.Collections;
using System.Collections.Generic;
using Interface;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipInstance : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    protected bool mouseEnter;
    protected const float SECONDS = 0.5f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseEnter = true;
        StartCoroutine(ShowTooltip());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseEnter = false;
        Tooltip.Instance.HideTooltip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        mouseEnter = false;
        Tooltip.Instance.HideTooltip();
    }

    private IEnumerator ShowTooltip()
    {
        var parts = GetComponents<ITooltipPart>();
        yield return new WaitForSeconds(SECONDS);
        if (mouseEnter && !ContextMenuController.Instance.IsActive)
            Tooltip.Instance.ShowTooltip(parts);
    }
}
