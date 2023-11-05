using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 targetPosition;
    CharacterController myController;
    [SerializeField]
    float movementSpeed;
    [SerializeField]
    float stopDistance;

    void Start()
    {
        myController = transform.GetComponent<CharacterController>();   
    }

    void Update()
    {
        HandleMovement();
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            targetPosition = hit.point;

    }
    void HandleMovement()
    {
        if (Vector3.Distance(transform.position, targetPosition) > stopDistance)
        {
            targetPosition.y = 0;
            Vector3 myDir = (targetPosition - transform.position);
            myDir=Vector3.ClampMagnitude(myDir, movementSpeed);
            myController.Move(myDir);      
        }
    }
}
