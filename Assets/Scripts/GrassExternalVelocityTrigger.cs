using System.Collections;
using UnityEngine;

public class GrassExternalVelocityTrigger : MonoBehaviour
{
    public GrassVelocityController _grassVelocityController;

    public GameObject _player;
    public Material _material;
    public Rigidbody2D _playerRB;

    private bool _easeInCoroutineRunning;
    private bool _easeOutCoroutineRunning;

    private int _externalInfluence = Shader.PropertyToID("_ExternalInfluence");

    private float _startingXVelocity;
    private float _velocityLastframe;

    public GameObject particle;
    public AudioSource audioSource;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerRB = _player.GetComponent<Rigidbody2D>();

        _grassVelocityController =GetComponentInParent<GrassVelocityController>();

        _material = GetComponent<SpriteRenderer>().material;
        _startingXVelocity = _material.GetFloat(_externalInfluence);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject== _player)
        {
            if(particle != null)
            {
                Instantiate(particle, _player.transform.position, Quaternion.identity);
            }
            if(audioSource != null)
            {
                audioSource.pitch = Random.Range(1.1f, 0.6f);
                audioSource.Play();
            }

            if (!_easeInCoroutineRunning && Mathf.Abs(_playerRB.linearVelocity.x) > Mathf.Abs(_grassVelocityController.VelocityThreshold))
            {
                StartCoroutine(EaseIn(_playerRB.linearVelocity.x * _grassVelocityController.ExternalInfluenceStrenght));
            }                     
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject== _player)
        {
            if (particle != null)
            {
                Instantiate(particle, _player.transform.position, Quaternion.identity);
            }            
            StartCoroutine(EaseOut());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject == _player)
        {
            if(Mathf.Abs(_velocityLastframe)>Mathf.Abs(_grassVelocityController.VelocityThreshold)&&
                Mathf.Abs(_playerRB.linearVelocity.x) < Mathf.Abs(_grassVelocityController.VelocityThreshold))
            {
                StartCoroutine(EaseOut());
            }

            else if(Mathf.Abs(_velocityLastframe)<Mathf.Abs(_grassVelocityController.VelocityThreshold)&&
                Mathf.Abs(_playerRB.linearVelocity.x) > Mathf.Abs(_grassVelocityController.VelocityThreshold))
            {
                StartCoroutine(EaseIn(_playerRB.linearVelocity.x * _grassVelocityController.ExternalInfluenceStrenght));
            }

            else if (!_easeInCoroutineRunning && !_easeOutCoroutineRunning &&
                Mathf.Abs(_playerRB.linearVelocity.x)>Mathf.Abs(_grassVelocityController.VelocityThreshold))
            {
                _grassVelocityController.InfluenceGrass(_material, _playerRB.linearVelocity.x * _grassVelocityController.ExternalInfluenceStrenght);
            }

            _velocityLastframe = _playerRB.linearVelocity.x;
        }
    }

    private IEnumerator EaseIn(float XVelocity)
    {
        _easeInCoroutineRunning = true;
        float elapsedTime = 0f;
        while (elapsedTime < _grassVelocityController.EaseInTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedAmount = Mathf.Lerp(_startingXVelocity, XVelocity, (elapsedTime / _grassVelocityController.EaseInTime));
            _grassVelocityController.InfluenceGrass(_material, lerpedAmount);

            yield return null;
        }

        _easeInCoroutineRunning =false;
    }

    private IEnumerator EaseOut()
    {
        _easeOutCoroutineRunning=true;
        float currentXInfluence = _material.GetFloat(_externalInfluence);

        float elapsedTime = 0f;
        while (elapsedTime<_grassVelocityController.EaseOutTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedAmount = Mathf.Lerp(currentXInfluence, _startingXVelocity, (elapsedTime / _grassVelocityController.EaseOutTime));
            _grassVelocityController.InfluenceGrass(_material, lerpedAmount);

            yield return null;
        }

        _easeOutCoroutineRunning =false;
    }
}
