using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform resetTransform;
    [SerializeField] GameObject player;
    [SerializeField] Camera playerhead;

    [ContextMenu("Reset Position")]

    public void ResetPosition()
    {
        var rotationAngleY = playerhead.transform.rotation.eulerAngles.y - 
                            resetTransform.rotation.eulerAngles.y;

        player.transform.Rotate(0, -rotationAngleY, 0);

        var distanceDiff = resetTransform.position - playerhead.transform.position;

        player.transform.position += distanceDiff;

    }
    
}
