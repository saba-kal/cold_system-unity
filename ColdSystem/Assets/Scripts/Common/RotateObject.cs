using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private float _xSpeed = 2f;
    [SerializeField] private float _ySpeed = 2f;
    [SerializeField] private float _zSpeed = 2f;

    private void Update()
    {
        transform.Rotate(Time.deltaTime * _xSpeed, Time.deltaTime * _ySpeed, Time.deltaTime * _zSpeed);
    }
}
