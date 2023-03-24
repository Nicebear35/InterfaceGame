using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _boxCollider2D;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<ShapeSquare>())
        {
            Destroy(collision.gameObject);
        }
    }
}
