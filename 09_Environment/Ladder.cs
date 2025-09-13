using UnityEngine;

public class Ladder : MonoBehaviour, IClimbable
{
    public void CanClimb()
    {
        if (PlayerAction.Instance.IsJumping) return; 
        PlayerAction.Instance.SetCanClimb(true);
    }
}
