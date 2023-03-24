using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeSquare : MonoBehaviour
{
    [SerializeField] private Image _occupiedImage;

    private void Start()
    {
        _occupiedImage.gameObject.SetActive(false);
    }

    public void DeactivateShape()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        gameObject.SetActive(false);
    }

    public void ActivateShape()
    {
        GetComponent<BoxCollider2D>().enabled = true;
        gameObject.SetActive(true);

    }
}
