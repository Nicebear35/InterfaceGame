using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSquare : MonoBehaviour
{
    public Image NormalImage;
    public Image HoverImage;
    public Image ActiveImage;
    public List<Sprite> NormalImages;

    public bool IsSelected { get; set; }
    public int SquareIndex { get; set; }
    public bool IsSquareOccupied { get; set; }

    public void SetImage(bool setFirstImage)
    {
        NormalImage.GetComponent<Image>().sprite = setFirstImage ? NormalImages[1] : NormalImages[0];
    }

    private void Start()
    {
        IsSelected = false;
        IsSquareOccupied = false;
    }

    public bool CanWeUseThisSquare()
    {
        return HoverImage.gameObject.activeSelf;
    }

    public void PlaceShapeOnBoard()
    {
        ActivateSquare();
    }
    
    public void ActivateSquare()
    {
        HoverImage.gameObject.SetActive(false);
        ActiveImage.gameObject.SetActive(true);
        IsSelected = true;
        IsSquareOccupied = true;
    }

    public void DeactivateSquare()
    {
        ActiveImage.gameObject.SetActive(false);
    }

    public void ClearOccupied()
    {
        IsSelected = false;
        IsSquareOccupied = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsSquareOccupied == false)
        {
            IsSelected = true;
            HoverImage.gameObject.SetActive(true);
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        IsSelected = true;

        if (IsSquareOccupied == false)
        {
            HoverImage.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsSquareOccupied == false)
        {
            IsSelected = false;
            HoverImage.gameObject.SetActive(false);
        }
    }
}
