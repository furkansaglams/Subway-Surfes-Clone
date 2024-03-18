using UnityEngine;

public class CameraMovemnt : MonoBehaviour
{
    public Transform target;
    [SerializeField] private float slidingSpeed;

    private Vector3 intialPos;


    private void Start()
    {
        intialPos = transform.position;
    }
    private void Update()
    {
        Vector3 moveVector = new Vector3(target.position.x, target.position.y + intialPos.y, target.position.z + intialPos.z);
        transform.position = Vector3.Lerp(transform.position, moveVector, Time.deltaTime * slidingSpeed);
    }
}
