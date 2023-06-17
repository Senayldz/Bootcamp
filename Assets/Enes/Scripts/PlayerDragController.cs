using UnityEngine;

public class PlayerDragController : MonoBehaviour
{
    [Header("Drag Settings")]
    [SerializeField] private float dragForce;
    [SerializeField] private float rotateSpeed;

    private Camera mainCam;

    private GameObject heldObject;
    private Rigidbody heldObjectRb;

    private LayerMask draggableLayer;

    private void Awake()
    {
        mainCam = Camera.main;
        draggableLayer = LayerMask.GetMask("Draggable");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Is an object currently being hold?
            if (heldObject == null)
            {
                Ray cameraRay = mainCam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue, draggableLayer))
                {
                    PickupObject(hit.transform.gameObject);
                }
                    
            }         
        }
        // Is an object currently being hold?
        if (heldObject != null) 
        {
            if (Input.GetMouseButtonUp(0))
            {
                ReleaseObject();
            }
            else
            {
                MoveObject();
                // Left Rotate
                if(Input.GetKey(KeyCode.Q))
                {
                    RotateObject(1);
                }
                // Right Rotate
                if(Input.GetKey(KeyCode.E))
                {
                    RotateObject(-1);
                }
            }
        }
    }

    private void PickupObject(GameObject pickedObject)
    {
        if (pickedObject.GetComponent<Rigidbody>()) 
        {
            heldObjectRb = pickedObject.GetComponent<Rigidbody>();
            heldObjectRb.useGravity = false;
            heldObjectRb.drag = 10;
            heldObjectRb.constraints = RigidbodyConstraints.FreezeRotation;
            heldObject = pickedObject;
        }
        else
        {
            Debug.Log("The object you try to pick up has no Rigidbody! Add it.");
        }
    }

    private void ReleaseObject()
    {
        heldObjectRb.useGravity = true;
        heldObjectRb.drag = 1;
        heldObjectRb.constraints = RigidbodyConstraints.None;

        heldObject = null;
    }

    private void MoveObject()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = -mainCam.transform.position.z;

        Vector3 mouseWorldPosition = mainCam.ScreenToWorldPoint(mouseScreenPosition);
        
        if (Vector3.Distance(heldObject.transform.position, mouseWorldPosition) > 0.1f)
        {
            Vector3 moveDirection = mouseWorldPosition - heldObject.transform.position;
            heldObjectRb.AddForce(moveDirection * dragForce * Time.deltaTime);
        }
    }

    private void RotateObject(int direction)
    {
        heldObject.transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime * direction);
    }
}
