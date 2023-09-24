using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ChargeCanvasBehavior : MonoBehaviour
{
    public enum FillType
    {
        Pickup,
        Dropoff,
        Blow
    }

    public Color pickupColor, dropoffColor, blowColor;
    
    private Transform _cam;
    private Image _image;

    private void Awake()
    {
        _cam = Camera.main.transform;
        _image = GetComponentInChildren<Image>();
    }
    
    private void Update()
    {
        transform.LookAt(_cam);
    }

    public void Fill(float amount, FillType type)
    {
        _image.fillAmount = amount;

        _image.color = type switch
        {
            FillType.Pickup => pickupColor,
            FillType.Dropoff => dropoffColor,
            FillType.Blow => blowColor,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
