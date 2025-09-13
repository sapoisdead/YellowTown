using System.Collections;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
    [SerializeField] private GameObject _playerCorpsePrefab;
    [SerializeField] private PlayerHealth _playerHealth;
    private Animator _playerAnimator;
    private readonly float _delayTime = 2f;

    private void Start()
    {
        _playerHealth.OnDeath += PlayerHealth_OnDeath;
        PlayerAction.Instance.OnDeathGround += PlayerAction_OnDeathGround;
        PlayerAction.Instance.OnDeathDivoured += PlayerAction_OnDeathDivoured;
    }

    private void OnDisable()
    {
        _playerHealth.OnDeath -= PlayerHealth_OnDeath;
        PlayerAction.Instance.OnDeathGround -= PlayerAction_OnDeathGround;
        PlayerAction.Instance.OnDeathDivoured -= PlayerAction_OnDeathDivoured;
    }

    private void PlayerHealth_OnDeath(object sender, System.EventArgs e)
    {
        HandleDeath(); 
        _playerAnimator.Play("Death_Final");
        StartCoroutine(LoadEndScreen());
    }
    private void PlayerAction_OnDeathGround(object sender, System.EventArgs e)
    {
        HandleDeath();
        _playerAnimator.Play("Death_Ground");
        StartCoroutine(LoadEndScreen());
    }

    private void PlayerAction_OnDeathDivoured(object sender, System.EventArgs e)
    {
        StartCoroutine(LoadEndScreen());
    }

    private void HandleDeath()
    {
        GameObject corpse = Instantiate(_playerCorpsePrefab, PlayerAction.Instance.transform.position, Quaternion.identity);

        _playerAnimator = corpse.GetComponent<Animator>();

        PlayerAction.Instance.ChangeStateToDeath();
        PlayerAction.Instance.gameObject.SetActive(false);
    }

    private IEnumerator LoadEndScreen()
    {
        yield return new WaitForSeconds(_delayTime);
        GameManager.Instance.GameOver(); 
    }
}
