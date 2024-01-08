using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRtsController : MonoBehaviour
{
    [SerializeField] private float _cameraPanScreenEdgeSize = 40;
    [SerializeField] private Texture2D _attackCursor;

    private RtsCamera _rtsCamera;
    private PlayerUnitManager _unitManager;
    private Camera _camera;
    private RtsInputActions _rtsInpusActions;
    private InputAction[] _selectUnitInputActions;
    private GroupFormationManager _groupFormationManager;
    private Vector3 _mouseGroundPosition;
    private Unit _enemyUnitUnderMouse;

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
        _groupFormationManager = GetComponentInChildren<GroupFormationManager>();
    }

    private void Start()
    {
        _unitManager = PlayerUnitManager.Instance;
        _rtsCamera = RtsCamera.Instance;
        if (_rtsCamera == null)
        {
            Debug.LogError("RTS camera is missing in the scene.");
        }
    }

    private void OnEnable()
    {
        _rtsInpusActions.Enable();
        _rtsInpusActions.Gameplay.SelectLocation.performed += OnLocationSelected;
        _rtsInpusActions.Gameplay.ActivateAbility.performed += OnAbilityActivated;
        _rtsInpusActions.Gameplay.ZoomCamera.performed += OnCameraZoom;
        _rtsInpusActions.Gameplay.ToggleAutoAttack.performed += OnToggleAutoAttack;
    }

    private void OnDisable()
    {
        _rtsInpusActions.Gameplay.SelectLocation.performed -= OnLocationSelected;
        _rtsInpusActions.Gameplay.ActivateAbility.performed -= OnAbilityActivated;
        _rtsInpusActions.Gameplay.ZoomCamera.performed -= OnCameraZoom;
        _rtsInpusActions.Gameplay.ToggleAutoAttack.performed -= OnToggleAutoAttack;
        _rtsInpusActions.Disable();
    }

    private void Update()
    {
        var selectedUnitCount = 0;
        for (var i = 0; i < _selectUnitInputActions.Length; i++)
        {
            var isSelected = _selectUnitInputActions[i].IsPressed() || _rtsInpusActions.Gameplay.SelectAllUnits.IsPressed();
            var isSelectSuccessful = _unitManager?.SetUnitSelected(i, isSelected) ?? false;
            if (isSelected && isSelectSuccessful) selectedUnitCount++;
        }
        _groupFormationManager.SelectedUnitCount = selectedUnitCount;

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

        _rtsCamera?.MoveCamera(cameraMoveDirection.normalized);
        _rtsCamera?.RotateCamera(-_rtsInpusActions.Gameplay.RotateCamera.ReadValue<float>());


        var ray = _camera.ScreenPointToRay(mousePosition);
        var layerMask = LayerMask.GetMask(Constants.GROUND_LAYER, Constants.ENEMY_UNIT_LAYER);
        if (Physics.Raycast(ray, out var hit, 2000f, layerMask))
        {
            var unit = hit.collider.GetComponent<Unit>();
            if (unit != null)
            {
                _enemyUnitUnderMouse = unit;
                _groupFormationManager.SelectedUnitCount = 0;
                if (_attackCursor != null)
                {
                    Cursor.SetCursor(_attackCursor, new Vector2(64, 0), CursorMode.Auto);
                }
            }
            else
            {
                _mouseGroundPosition = hit.point;
                _groupFormationManager.FormationPosition = _mouseGroundPosition;
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
        }
    }

    private void OnLocationSelected(InputAction.CallbackContext context)
    {
        if (_enemyUnitUnderMouse != null)
        {
            _unitManager?.SetTargetUnitToAttack(_enemyUnitUnderMouse);
        }
        else
        {
            var formationPositions = _groupFormationManager.GetFormationPositions();
            var i = 0;
            foreach (var unit in _unitManager.GetSelectedUnits())
            {
                unit.SetDestination(formationPositions[i]);
                i++;
            }
        }
    }

    private void OnAbilityActivated(InputAction.CallbackContext context)
    {
        _unitManager?.ActivateAbilities();
    }

    private void OnCameraZoom(InputAction.CallbackContext context)
    {
        // This stupid bugs requires me to clamp axis value: https://forum.unity.com/threads/how-do-you-get-mouse-scroll-input.825672/
        var value = Mathf.Clamp(context.ReadValue<float>(), -1, 1);
        _rtsCamera.ZoomCamera(value);
    }

    private void OnToggleAutoAttack(InputAction.CallbackContext context)
    {
        _unitManager?.ToggleAutoAttack();
    }
}
