using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRtsController : MonoBehaviour
{
    [SerializeField] private PlayerUnitManager _unitManager;
    [SerializeField] private Camera _camera;

    private RtsInputActions _rtsInpusActions;

    private void Awake()
    {
        _rtsInpusActions = new RtsInputActions();
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        _rtsInpusActions.Enable();
        _rtsInpusActions.Gameplay.SelectLocation.performed += OnLocationSelected;
    }


    private void OnDisable()
    {
        _rtsInpusActions.Gameplay.SelectLocation.performed -= OnLocationSelected;
        _rtsInpusActions.Disable();
    }

    private void OnLocationSelected(InputAction.CallbackContext context)
    {
        var ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out var hit) && hit.collider)
        {
            _unitManager.SetDestination(hit.point);
        }
    }
}
