﻿using Cinemachine;
using UnityEngine;


public class RtsCamera : MonoBehaviour
{
    [SerializeField] private float _panSpeed = 10f;
    [SerializeField] private float _rotateSpeed = 100f;
    [SerializeField] private float _zoomSpeed = 6f;
    [SerializeField] private float _zoomSmoothness = 1f;
    [SerializeField] private float _minZoom = 1f;
    [SerializeField] private float _maxZoom = 40f;
    [SerializeField] private Transform _cameraTarget;

    private CinemachineTransposer _cameraTransposer;
    private float _targetZoom = -30f;

    private void Awake()
    {
        var virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        _cameraTransposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        _targetZoom = _cameraTransposer.m_FollowOffset.z;
    }

    private void Update()
    {
        _cameraTransposer.m_FollowOffset = new Vector3(0, 0, Mathf.Lerp(_cameraTransposer.m_FollowOffset.z, _targetZoom, _zoomSmoothness * Time.deltaTime));
    }

    public void MoveCamera(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.1f)
        {
            return;
        }

        var translation = _cameraTarget.TransformDirection(new Vector3(direction.x, 0, direction.y)) * _panSpeed * Time.deltaTime;
        var rayStartPosition = _cameraTarget.position + new Vector3(0, 10, 0) + translation;
        if (Physics.Raycast(rayStartPosition, Vector3.down, out var hit, 500f))
        {
            _cameraTarget.position = hit.point;
        }
        else
        {
            _cameraTarget.Translate(translation);
        }
    }

    public void RotateCamera(float direction)
    {
        _cameraTarget.Rotate(new Vector3(0, 1, 0), direction * _rotateSpeed * Time.deltaTime, Space.World);
    }

    public void ZoomCamera(float direction)
    {
        // The zoom is determined by the z offset of the camera transposer.
        // This offset is always negative. The smaller the number, the more far away the camera will be.
        // This is why zoom calculation is negated.
        var newZoom = _cameraTransposer.m_FollowOffset.z - direction * _zoomSpeed;
        _targetZoom = Mathf.Clamp(newZoom, -_maxZoom, -_minZoom);
    }
}