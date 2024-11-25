using UnityEngine;


public class ResetTrigger : MonoBehaviour
{
public GameObject resetArea;

void OnTriggerEnter(Collider other)

{

    if (other.gameObject == resetArea)

    {

        resetArea.GetComponent<PlayerController>().ResetPosition();

    }

}

}
