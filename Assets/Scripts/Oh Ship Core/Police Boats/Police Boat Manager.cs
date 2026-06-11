using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

public class PoliceBoatManager : MonoBehaviour
{
    [SerializeField] private string _playerTag ="Player Steam Boat";
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private GameObject _playerSteamBoat;
    private SplineContainer _spline;
    private float _policeBoatPathingProgress;
    [SerializeField]private UnityEvent _transitionEvent;
    
    void Start()
    {
        _spline = FindAnyObjectByType<SplineContainer>();
        if(_transitionEvent == null) Debug.LogError($"{nameof(_transitionEvent)} is null");
    }

    // Update is called once per frame
    void Update()
    {
        _policeBoatPathingProgress += _speed / _spline.CalculateLength() * Time.deltaTime;
        transform.position = _spline.EvaluatePosition(_policeBoatPathingProgress);
        transform.forward = -(_spline.EvaluateTangent(_policeBoatPathingProgress));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            Debug.Log("Hit the player");
            _transitionEvent.Invoke();
        }
    }
}
