using UnityEngine;

public class DamageDealer : MonoBehaviour, IDamageable, IObstacle
{
   public int Damage { get; set; } = 5; 
}
