using UnityEngine;
using UnityEngine.UI;
public class ShipHoleVisual : MonoBehaviour
{
    [SerializeField] private Image _radialImage;
    
    public void UpdateRadialProgress(float radialProgress)
    {
        _radialImage.fillAmount = radialProgress;
    }
    
    
}
