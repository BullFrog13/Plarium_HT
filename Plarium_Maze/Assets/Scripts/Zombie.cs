using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public Transform Victim;
    private NavMeshAgent NavComponent;

    public void Start()
    {
        NavComponent = transform.GetComponent<NavMeshAgent>();
    }

    public void Update()
    {
        if (Victim)
        {
            NavComponent.SetDestination(Victim.position);
        }
    }
}