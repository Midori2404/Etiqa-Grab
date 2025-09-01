using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Camera mainCamera;
    private PlayerInputActions playerInputActions;
    private Vector2 dragDelta;
    private Vector2 lastTouchPosition;
    [SerializeField] private bool isDragging;
    [SerializeField] private Transform draggedObject;
    [SerializeField] private LayerMask layerMask;

    // Start is called before the first frame update
    void OnEnable()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        // Tap action
        playerInputActions.Player.Tap.performed += OnTap;

        // Press action to detect both clicking & dragging
        playerInputActions.Player.Press.started += OnPress;
        playerInputActions.Player.Press.canceled += OnRelease;

        // Drag action
        playerInputActions.Player.Drag.performed += OnDrag;
    }

    void OnDisable()
    {
        playerInputActions.Player.Tap.performed -= OnTap;
        playerInputActions.Player.Press.started -= OnPress;
        playerInputActions.Player.Press.canceled -= OnRelease;
        playerInputActions.Player.Drag.performed -= OnDrag;
        playerInputActions.Player.Disable();
    }

    private void OnRelease(InputAction.CallbackContext context)
    {
        if (draggedObject != null)
        {
            DraggableObject draggable = draggedObject.GetComponent<DraggableObject>();
            if (draggable != null)
            {
                draggable.SnapToDropArea();
            }
        }

        isDragging = false;
        draggedObject = null;
        Debug.Log("Stopped Dragging");
    }


    private void OnPress(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.isLost)
            return;
            
        Vector2 pointerPosition = GetPointerPosition();
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(pointerPosition.x, pointerPosition.y, mainCamera.transform.position.z));
        worldPosition.z = 0f; // Keep on the same plane as the object

        // Raycast to find a draggable object
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero, 150f, layerMask);
        if (hit.collider != null && hit.collider.CompareTag("DraggableObject"))
        {
            isDragging = true;
            draggedObject = hit.collider.transform;
            draggedObject.GetComponent<DraggableObject>().SetMovingPermission(isDragging);

            Debug.Log($"Started Dragging: {draggedObject.name}");
        }
    }


    private void OnTap(InputAction.CallbackContext context)
    {
        // Debug.Log("Tap or Click detected!");
        // Add your tap/click behavior here
    }

    private void OnDrag(InputAction.CallbackContext context)
    {
        if (isDragging && draggedObject != null)
        {
            Vector2 pointerPosition = GetPointerPosition();
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(pointerPosition.x, pointerPosition.y, mainCamera.transform.position.z));
            worldPosition.z = 0f;

            // Directly move the object to the pointer position
            draggedObject.position = worldPosition;
        }
    }

    // Helper function to get pointer position (mouse or touch)
    private Vector2 GetPointerPosition()
    {
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            return Mouse.current.position.ReadValue();
        }
        else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            lastTouchPosition = Touchscreen.current.primaryTouch.position.ReadValue(); // Cache touch position
            return lastTouchPosition;
        }

        return lastTouchPosition; // Return last valid position instead of (0,0)
    }
}
