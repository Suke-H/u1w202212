using DG.Tweening;  
using UnityEngine;  
using UnityEngine.EventSystems;  

public class CustomButton : MonoBehaviour,  
    IPointerClickHandler,  
    IPointerDownHandler,  
    IPointerUpHandler  
{
    public System.Action onClickCallback;  

    [SerializeField] private CanvasGroup _canvasGroup;
    
    bool isActive = true;

    public void setActive(bool activeType){
        this.gameObject.SetActive(activeType);
        isActive = activeType;
    }

    public void OnPointerClick(PointerEventData eventData)  
    {
        if (isActive) { onClickCallback?.Invoke(); }
    }

    public void OnPointerDown(PointerEventData eventData)  
    {
        transform.DOScale(0.95f, 0.24f).SetEase(Ease.OutCubic);  
        _canvasGroup.DOFade(0.8f, 0.24f).SetEase(Ease.OutCubic);
    }

    public void OnPointerUp(PointerEventData eventData)  
    {
        transform.DOScale(1f, 0.24f).SetEase(Ease.OutCubic);  
        _canvasGroup.DOFade(1f, 0.24f).SetEase(Ease.OutCubic);
    }
}
