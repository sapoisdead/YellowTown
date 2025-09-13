using System.Collections;
using UnityEngine;
using System;

public class Cube : MonoBehaviour
{
    private Animator _animator;
    private AudioSource _audioSource;
    private GameObject _frog;
    private EnemyHealth _enemyHealth;

    private uint _collisionCount = 0;

    private void OnDisable()
    {
        _enemyHealth.OnDeath -= KillFrog;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _frog = transform.GetChild(0).gameObject;
        _frog.SetActive(false);
        _enemyHealth = _frog.GetComponent<EnemyHealth>();
    }

    private void Start()
    {
        _enemyHealth.OnDeath += KillFrog;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.y > 0)
        {
            _collisionCount++;
            CubeInteract();
        }
    }

    private void CubeInteract()
    {
        _animator.Play("Cube_Bounce");

        switch (_collisionCount)
        {
            case 1:
                _animator.Play("CubeInteract");
                break;
            case 2:
                SpawnFrog();
                break;
            default:
                _animator.Play("CubeInteract");
                break; 
        }

        StartCoroutine(AnimateBump());
    }

    private void SpawnFrog()
    {
        _audioSource.Play();
        _frog.SetActive(true);
    }

    private void KillFrog(object sender, EventArgs e)
    {
        _frog.SetActive(false);
    }

    private IEnumerator AnimateBump()
    {
        Vector3 originalPosition = transform.position;
        Vector3 targetPosition = originalPosition + Vector3.up * 0.2f;

        float speed = 10f;
        float duration = 0.1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime * speed;
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(targetPosition, originalPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime * speed;
            yield return null;
        }

        transform.position = originalPosition;
    }
}
