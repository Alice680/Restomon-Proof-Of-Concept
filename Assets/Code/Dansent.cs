using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
/*
[RequireComponent(typeof(Selectable))]
public class HoverOverSelectable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Vector3 hoveredScale = new Vector3(1.1f, 1.1f, 1.1f);
    private Vector3 defaultScale;

    [HideInInspector] public bool isHovered = false;
    private Selectable selectable;

    private void Awake()
    {
        SetDefaultScale();
        selectable = GetComponent<Selectable>();
    }

    public void SetDefaultScale()
    {
        defaultScale = Vector3.one;
    }

    public Vector3 GetDefaultScale()
    {
        return transform.localScale = defaultScale;
    }
    public Vector3 SetHoveredScale()
    {
        return GetHoveredScale();
    }

    public Vector3 GetHoveredScale()
    {
        return transform.localScale = hoveredScale;
    }

    public void ListenerHovered()
    {
        SetHoveredScale();
    }

    public void ListenerDefault()
    {
        GetDefaultScale();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        transform.localScale = defaultScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectable.IsInteractable())
        {
            isHovered = true;
            transform.localScale = SetHoveredScale();

        }
        if (TryGetComponent(out ActionOnSelect aos))
        {
            if (selectable.IsInteractable())
            {
                aos.OnSelect(null);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selectable.IsInteractable())
        {
            isHovered = false;
            transform.localScale = GetDefaultScale();
            if (TryGetComponent(out ActionOnSelect aos))
            {
                aos.OnDeselect(null);
            }
        }
    }

    private void OnDisable()
    {
        isHovered = false;
        transform.localScale = GetDefaultScale();
    }
}*/