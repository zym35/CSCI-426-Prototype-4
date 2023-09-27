using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TorcheyeUtility;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public enum ControlScheme
    {
        WASDSpace,
        ArrowKeyL,
        Controller
    }

    public ControlScheme controlScheme;
    public float moveSpeed;
    public float gemSpeedSlow, gemBlowSizeup, gemBlowPowerup, gemBlowOffProbabilityUp, gemKnockoffDurationUp;
    public float stackSpacing;
    public Transform stackBegin;
    public float autoActionTime, blowTime;
    public Transform blowObject;
    public float blowStartSize, blowMaxSize;
    public float blowStartPower;
    public int score;
    public ChargeCanvasBehavior chargeCanvasBehavior;
    public BlowTrigger blowTrigger;
    public TMP_Text scoreText;
    public GameObject endGame;
    public bool knockoff;
    public float knockoffDuration;

    private Rigidbody _rb;
    private List<Transform> _gemInRange;
    private Stack<Transform> _gemStacked;
    private float _dropoffTimer, _pickupTimer, _chargeTimer;
    private bool _inDropoffArea;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _gemInRange = new List<Transform>();
        _gemStacked = new Stack<Transform>();
    }

    private void Update()
    {
        if (!knockoff)
        {
            HandleInput();
        }

        if (_chargeTimer <= 0)
        {
            if (_inDropoffArea)
            {
                DoDropoff();
            }
            else if (_gemInRange.Count > 0)
            {
                OnGemInRange();
            } 
        }
    }

    private void HandleInput()
    {
        switch (controlScheme)
        {
            case ControlScheme.WASDSpace:
                OnMove(Input.GetAxis("HorizontalWASD"), Input.GetAxis("VerticalWASD"));
                if (Input.GetKey(KeyCode.Space))
                    OnChargeAction();
                if (Input.GetKeyUp(KeyCode.Space))
                    OnAction();
                break;
            case ControlScheme.ArrowKeyL:
                OnMove(Input.GetAxis("HorizontalArrow"), Input.GetAxis("VerticalArrow"));
                if (Input.GetKey(KeyCode.L))
                    OnChargeAction();
                if (Input.GetKeyUp(KeyCode.L))
                    OnAction();
                break;
            case ControlScheme.Controller:
                OnMove(Input.GetAxis("HorizontalController"), Input.GetAxis("VerticalController"));
                if (Input.GetKey(KeyCode.Joystick1Button0))
                    OnChargeAction();
                if (Input.GetKeyUp(KeyCode.Joystick1Button0))
                    OnAction();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnMove(float horizontal, float vertical)
    {
        _rb.AddForce(Mathf.Pow(1f - gemSpeedSlow, _gemStacked.Count)
                     * Time.deltaTime * moveSpeed * new Vector3(horizontal, 0, vertical));
    }

    private void OnAction()
    {
        if (_chargeTimer >= blowTime)
        {
            var force = blowStartPower + _gemStacked.Count * gemBlowPowerup;
            
            foreach (var rb in blowTrigger.gemInRange)
            {
                var dir = Vector3.Normalize(rb.transform.position - transform.position);
                rb.AddForce(force * dir, ForceMode.Impulse);
            }

            foreach (var p in blowTrigger.playerInRange)
            {
                var dirPlayer = Vector3.Normalize(p.transform.position - transform.position);
                StartCoroutine(p.DoKnockoff(force * dirPlayer, knockoffDuration + _gemStacked.Count * gemKnockoffDurationUp));
                
                if (Random.Range(0f, 1f) < p.GetGemBlowOffProbability())
                {
                    var g = p.PopGemStacked();
                    var dir = Vector3.Normalize(g.position - transform.position);
                    g.SetParent(null);

                    var rb = g.GetComponent<Rigidbody>();
                    rb.isKinematic = false;
                    rb.AddForce(force * dir, ForceMode.Impulse);
                }
            }
        }
        
        chargeCanvasBehavior.Fill(0, ChargeCanvasBehavior.FillType.Blow);
        _chargeTimer = 0;
        blowObject.localScale = blowStartSize * Vector3.one;
    }

    private IEnumerator DoKnockoff(Vector3 force, float duration)
    {
        knockoff = true;

        var constraints = _rb.constraints;
        _rb.constraints = RigidbodyConstraints.None;
        _rb.AddForce(force, ForceMode.Impulse);
        
        AudioManager.Instance.PlaySoundEffect(AudioManager.SoundEffect.Knockoff);
        
        yield return new WaitForSeconds(duration);
        transform.localRotation = Quaternion.identity;
        _rb.constraints = constraints;
        knockoff = false;
    }
    
    public Transform PopGemStacked()
    {
        return _gemStacked.Pop();
    }

    public float GetGemBlowOffProbability()
    {
        if (_gemStacked.Count == 0)
            return -1;
        return _gemStacked.Count * gemBlowOffProbabilityUp + 0.1f;
    }

    private void OnChargeAction()
    {
        _chargeTimer += Time.deltaTime;
        var percent = Mathf.Clamp01(_chargeTimer / blowTime);
        chargeCanvasBehavior.Fill(percent, ChargeCanvasBehavior.FillType.Blow);

        blowObject.localScale = 
            Mathf.Lerp(blowStartSize, blowMaxSize + gemBlowSizeup * _gemStacked.Count, percent) * Vector3.one;
    }

    private void DoDropoff()
    {
        if (_gemStacked.Count == 0)
            return;
            
        _dropoffTimer += Time.deltaTime;
        chargeCanvasBehavior.Fill(_dropoffTimer / autoActionTime, ChargeCanvasBehavior.FillType.Dropoff);
        if (_dropoffTimer > autoActionTime)
        {
            // TODO: animation and effects
            _dropoffTimer = 0;
            chargeCanvasBehavior.Fill(0, ChargeCanvasBehavior.FillType.Dropoff);
            Destroy(_gemStacked.Pop().gameObject);
            
            score++;
            scoreText.text = score.ToString();
            
            AudioManager.Instance.PlaySoundEffect(AudioManager.SoundEffect.Score);

            if (score == 10)
            {
                endGame.SetActive(true);
            }
        }
    }

    private void OnGemInRange()
    {
        _pickupTimer += Time.deltaTime;
        chargeCanvasBehavior.Fill(_pickupTimer / autoActionTime, ChargeCanvasBehavior.FillType.Pickup);
        if (_pickupTimer > autoActionTime)
        {
            _pickupTimer = 0;
            chargeCanvasBehavior.Fill(0, ChargeCanvasBehavior.FillType.Pickup);
            var g = _gemInRange[0];
        
            g.SetParent(stackBegin);
            g.localPosition = Vector3.zero;
            g.Translate(stackSpacing * _gemStacked.Count * Vector3.up);
        
            g.GetComponent<Rigidbody>().isKinematic = true;
            _gemStacked.Push(g);
            _gemInRange.Remove(g);
            
            AudioManager.Instance.PlaySoundEffect(AudioManager.SoundEffect.Pickup);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gem"))
        {
            if (other.transform.parent != null)
                return;
            var g = other.transform;
            if (!_gemInRange.Contains(g) && !_gemStacked.Contains(g)) 
                _gemInRange.Add(g);
        }
        
        if (other.CompareTag("Dropoff"))
        {
            _inDropoffArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Gem"))
        {
            var g = other.transform;
            if (_gemInRange.Contains(g) && !_gemStacked.Contains(g)) 
                _gemInRange.Remove(g);

            if (_gemInRange.Count == 0)
            {
                chargeCanvasBehavior.Fill(0, ChargeCanvasBehavior.FillType.Dropoff);
                _pickupTimer = 0;
            }
        }
        
        if (other.CompareTag("Dropoff"))
        {
            _inDropoffArea = false;
            chargeCanvasBehavior.Fill(0, ChargeCanvasBehavior.FillType.Dropoff);
            _dropoffTimer = 0;
        }
    }
}
