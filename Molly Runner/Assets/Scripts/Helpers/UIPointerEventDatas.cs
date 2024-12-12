using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPointerEventDatas : MonoBehaviour, IDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        if (PlayerController.instance)
            StartCoroutine(GameManager.instance.OnPlayClick());

        if (GameManager.instance.gameObject != null)
            if (GameManager.instance)
                PlayerController.instance.currentPhase = PlayerController.Phases.inGame;
        Destroy(this.gameObject);
    }
}
