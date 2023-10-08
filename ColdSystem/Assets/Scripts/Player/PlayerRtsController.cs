using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRtsController : MonoBehaviour
{
    [SerializeField] private PlayerUnitManager _unitManager;
    [SerializeField] private Camera _camera;

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
    }

    private void OnDisable()
    {
        _rtsInpusActions.Gameplay.SelectLocation.performed -= OnLocationSelected;
        _rtsInpusActions.Disable();
    }

    private void Update()
    {
        for (var i = 0; i < _selectUnitInputActions.Length; i++)
        {
            _unitManager.SetUnitSelected(i, _selectUnitInputActions[i].IsPressed() || _rtsInpusActions.Gameplay.SelectAllUnits.IsPressed());
        }
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
