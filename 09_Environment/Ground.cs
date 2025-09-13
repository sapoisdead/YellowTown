using UnityEngine;

public class Ground : MonoBehaviour, IWalkable, IObstacle
{
    public void SetGroundedState()
    {
        PlayerAction.Instance.SetIsGrounded(true);
    }

}
