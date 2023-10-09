using UnityEngine;


public class SphereGizmo : MonoBehaviour
{
    [SerializeField] private Color _color = Color.white;
    [SerializeField][Range(0.01f, 20f)] private float _radius;

    private void OnDrawGizmos()
    {
        Gizmos.color = _color;
        Gizmos.DrawSphere(transform.position, _radius);
    }
}