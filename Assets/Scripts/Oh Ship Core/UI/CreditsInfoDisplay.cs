using System;
using UnityEngine;
using UnityEngine. UI;
using UnityEngine.EventSystems;
using System.Collections;


public class CreditsInfoDisplay : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Image _imageToFlip;
    [SerializeField] private Sprite _informationSprite;
    //[SerializeField] private Sprite _mugShotSprite;
    [SerializeField] private GameObject _mugShotPictureToFlip;
    [SerializeField] private Sprite _mugShotPictureSprite;
    private Sprite _ogSprite;
    private Transform _transform;
    private Coroutine _coroutine;
    
    private void Start()
    {
        _transform = transform.GetChild(0);
        _ogSprite = _mugShotPictureSprite;
      //  _mugShotPictureSprite = GetComponentInChildren<Image>().sprite;
        Debug.Log(_mugShotPictureSprite);
    }

    private void SelectCreditButton()
    {
       
        FlipMugShotImage(180);
    }

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("OnSelect");
        SelectCreditButton();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        FlipMugShotImage(0);
    }

    public void FlipMugShotImage(int rotation)
    {
        if(_coroutine != null)StopCoroutine(_coroutine);
        _coroutine =  StartCoroutine(SmoothRotate(rotation, .5f));
    }

    private IEnumerator SmoothRotate(int rotation, float duration)
    {
        
        float timePassed = 0f;
        
        bool isFlipped = false;
        
        Quaternion startRotation = _transform.localRotation;
        
        Quaternion endRotation = Quaternion.Euler(_transform.localRotation.x, rotation, _transform.localRotation.z);

        Sprite spriteToChange = (rotation == 0) ? _ogSprite : _informationSprite;

        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            
            float inBetween = Mathf.Clamp01(timePassed / duration);
            
            inBetween = inBetween * inBetween * (3f - 2f * inBetween);
            
            _transform.localRotation = Quaternion.Slerp(startRotation, endRotation, inBetween);

            if (!isFlipped && inBetween >= 0.5f)
            {
                Debug.Log("Flipped");
                Debug.Log(_mugShotPictureToFlip);
                Debug.Log(_mugShotPictureToFlip.GetComponent<Image>().sprite.name);

                _mugShotPictureToFlip.GetComponent<Image>().sprite = spriteToChange;
                Debug.Log(_mugShotPictureToFlip.GetComponent<Image>().sprite.name);
                isFlipped = true;
            }
            
            yield return null;
        }

        _transform.localRotation = endRotation;
    }
}
