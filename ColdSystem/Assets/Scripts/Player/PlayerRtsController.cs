using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRtsController : MonoBehaviour
{
    [SerializeField] private PlayerUnitManager _unitManager;
    [SerializeField] private RtsCamera _rtsCamera;
    [SerializeField] private float _cameraPanScreenEdgeSize = 40;

    private Camera _camera;
    private RtsInputActions _rtsInpusActions;
    private InputAction[] _selectUnitInputActions;

    private void Awake()
    {
        _rtsInpusActions = new RtsInputActions();
        _camera = Camera.main;
        _selectUnitInputActions = new InputAction[5]
        {
            _rtsInpusActions.Gameplay.SelectUnit1,
            _rtsInpusActions.Gameplay.SelectUnit2,
            _rtsInpusActions.Gameplay.SelectUnit3,
            _rtsInpusActions.Gameplay.SelectUnit4,
            _rtsInpusActions.Gameplay.SelectUnit5
        };
    }

    private void OnEnable()
    {
        _rtsInpusActions.Enable();
        _rtsInpusActions.Gameplay.SelectLocation.performed += OnLocationSelected;
        _rtsInpusActions.Gameplay.ZoomCamera.performed += OnCameraZoom;
    }

    private void OnDisable()
    {
        _rtsInpusActions.Gameplay.SelectLocation.performed -= OnLocationSelected;
        _rtsInpusActions.Gameplay.ZoomCamera.performed -= OnCameraZoom;
        _rtsInpusActions.Disable();
    }

    private void Update()
    {
        for (var i = 0; i < _selectUnitInputActions.Length; i++)
        {
            _unitManager?.SetUnitSelected(i, _selectUnitInputActions[i].IsPressed() || _rtsInpusActions.Gameplay.SelectAllUnits.IsPressed());
        }

        var cameraMoveDirection = _rtsInpusActions.Gameplay.MoveCamera.ReadValue<Vector2>();
        var mousePosition = Mouse.current.position.ReadValue();
        if (Screen.width - mousePosition.x < _cameraPanScreenEdgeSize)
        {
            cameraMoveDirection.x = 1;
        }
        if (mousePosition.x < _cameraPanScreenEdgeSize)
        {
            cameraMoveDirection.x = -1;
        }
        if (Screen.height - mousePosition.y < _cameraPanScreenEdgeSize)
        {
            cameraMoveDirection.y = 1;
        }
        if (mousePosition.y < _cameraPanScreenEdgeSize)
        {
            cameraMoveDirection.y = -1;
        }

        _rtsCamera.MoveCamera(cameraMoveDirection.normalized);
        _rtsCamera.RotateCamera(_rtsInpusActions.Gameplay.RotateCamera.ReadValue<float>());
    }

    private void OnLocationSelected(InputAction.CallbackContext context)
    {
        var ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out var hit) && hit.collider)
        {
            _unitManager?.SetDestination(hit.point);
        }
    }

    private void OnCameraZoom(InputAction.CallbackContext context)
    {
        // This stupid bugs requires me to clamp axis value: https://forum.unity.com/threads/how-do-you-get-mouse-scroll-input.825672/
        var value = Mathf.Clamp(context.ReadValue<float>(), -1, 1);
        _rtsCamera.ZoomCamera(value);
    }
}
