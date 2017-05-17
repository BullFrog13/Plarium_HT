using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject Follow;

    private void Start()
    {
    }

    private void LateUpdate()
    {
        transform.position = Follow.transform.position;
    }
}