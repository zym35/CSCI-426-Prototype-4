using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float gemSpeedSlowMultiplier;
    public float stackSpacing;
    public Transform stackBegin;
    public float dropoffTime;
    public int score;

    private Rigidbody _rb;
    private List<Transform> _gemInRange;
    private Stack<Transform> _gemStacked;
    private float _dropoffTimer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _gemInRange = new List<Transform>();
        _gemStacked = new Stack<Transform>();
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        switch (controlScheme)
        {
            case ControlScheme.WASDSpace:
                OnMove(Input.GetAxis("HorizontalWASD"), Input.GetAxis("VerticalWASD"));
                if (Input.GetKeyDown(KeyCode.Space))
                    OnAction();
                break;
            case ControlScheme.ArrowKeyL:
                OnMove(Input.GetAxis("HorizontalArrow"), Input.GetAxis("VerticalArrow"));
                if (Input.GetKeyDown(KeyCode.L))
                    OnAction();
                break;
            case ControlScheme.Controller:
                OnMove(Input.GetAxis("HorizontalController"), Input.GetAxis("VerticalController"));
                if (Input.GetKeyDown(KeyCode.Joystick1Button0))
                    OnAction();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnMove(float horizontal, float vertical)
    {
        _rb.AddForce(Time.deltaTime * moveSpeed * new Vector3(horizontal, 0, vertical));
    }

    private void OnAction()
    {
        if (_gemInRange.Count == 0)
            return;

        var g = _gemInRange[0];
        
        g.SetParent(stackBegin);
        g.localPosition = Vector3.zero;
        g.Translate(stackSpacing * _gemStacked.Count * Vector3.up);
        g.GetComponent<Rigidbody>().isKinematic = true;
        _gemStacked.Push(g);
        _gemInRange.Remove(g);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gem"))
        {
            var g = other.transform;
            if (!_gemInRange.Contains(g) && !_gemStacked.Contains(g)) 
                _gemInRange.Add(g);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Dropoff"))
        {
            if (_gemStacked.Count == 0)
                return;
            
            _dropoffTimer += Time.deltaTime;
            if (_dropoffTimer > dropoffTime)
            {
                // TODO: animation and effects
                _dropoffTimer = 0;
                Destroy(_gemStacked.Pop().gameObject);
                score++;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Gem"))
        {
            var g = other.transform;
            if (_gemInRange.Contains(g) && !_gemStacked.Contains(g)) 
                _gemInRange.Remove(g);
        }
    }
}
