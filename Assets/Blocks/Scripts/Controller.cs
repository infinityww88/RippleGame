using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using ScriptableObjectArchitecture;

public class Controller: MonoBehaviour
{
    public GameObjectGameEvent ReleasePatternEvent;
    public GameObjectGameEvent PreviewPatternEvent;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void OnPointerDown(BaseEventData eventData)
    {
        BlockPattern pattern = GetComponentInChildren<BlockPattern>();
        var pointerEventData = (PointerEventData)eventData;
        var pos = Camera.main.ScreenToWorldPoint(pointerEventData.position);
        pos.z = 0;
	    transform.position = pos + new Vector3(0, 5, 0);
        PreviewPatternEvent.Raise(gameObject);
        pattern.SetNormal();
    }

    public void OnPointerUp(BaseEventData eventData)
    {
        ReleasePatternEvent.Raise(gameObject);
        BlockPattern pattern = GetComponentInChildren<BlockPattern>();
        pattern.SetSmall();
    }

    public void BeginDrag(BaseEventData eventData)
    {
        Debug.Log("Begin Drag");
    }
    public void EndDrag(BaseEventData eventData)
    {
        Debug.Log("End Drag");
    }
    public void OnDrag(BaseEventData eventData)
    {
        var pointerEventData = (PointerEventData)eventData;
        var pos = Camera.main.ScreenToWorldPoint(pointerEventData.position);
        pos.z = 0;
        transform.position = pos + new Vector3(0, 5, 0);
        PreviewPatternEvent.Raise(gameObject);
    }
}
