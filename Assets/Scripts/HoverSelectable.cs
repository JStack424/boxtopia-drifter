using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class HoverSelectable : MonoBehaviour, IPointerEnterHandler, IDeselectHandler
{
    private bool _wasSelected;
    private void Update()
    {
        if (_wasSelected && EventSystem.current.currentSelectedGameObject == null)
        {
            var thisSelectable = GetComponent<Selectable>();
            var selectables = new List<Selectable>()
            {
                thisSelectable,
                thisSelectable.FindSelectableOnDown(),
                thisSelectable.FindSelectableOnUp(),
                thisSelectable.FindSelectableOnRight(),
                thisSelectable.FindSelectableOnLeft(),
            };
            Selectable next = selectables.Where(x => x != null && x.IsInteractable()).First();
            EventSystem.current.SetSelectedGameObject(next?.gameObject);
        }

        _wasSelected = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!GetComponent<Selectable>().IsInteractable())
            return;
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        GetComponent<Selectable>().OnPointerExit(null);
        _wasSelected = true;
    }
}
