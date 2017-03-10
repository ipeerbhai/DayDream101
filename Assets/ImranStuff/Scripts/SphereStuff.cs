using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SphereStuff : MonoBehaviour, IGvrPointerHoverHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Renderer sphereColor;
    public Color highlightColor;

    private Color m_originalColor;
	// Use this for initialization
	public void Start ()
    {
        m_originalColor = sphereColor.material.color;
	}
	
	// Update is called once per frame
	public void Update ()
    {
		
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.logger.Log(eventData);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.logger.Log(eventData);

    }

    public void OnGvrPointerHover(PointerEventData eventData)
    {
        Debug.logger.Log(eventData);
    }
}
