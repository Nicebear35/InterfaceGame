using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeStorage : MonoBehaviour
{
    public List<ShapeData> ShapeData;
    public List<Shape> ShapeList;

    private void OnEnable()
    {
        GameEvents.RequestNewShapes += RequestNewShapes;
    }

    private void OnDisable()
    {
        GameEvents.RequestNewShapes -= RequestNewShapes;
    }

    private void Start()
    {
        foreach (var shape in ShapeList)
        {
            var index = UnityEngine.Random.Range(0, ShapeData.Count);

            shape.CreateShape(ShapeData[index]);
        }
    }

    public Shape GetCurrentSelectedShape()
    {
        foreach (var shape in ShapeList)
        {
            if (shape.IsOnStartPosition() == false && shape.IsAnyShapeSquareActive())
            {
                return shape;
            }
        }

        return null;
    }

    private void RequestNewShapes()
    {
        foreach (var shape in ShapeList)
        {
            var shapeIndex = UnityEngine.Random.Range(0, ShapeData.Count);
            shape.RequestNewShape(ShapeData[shapeIndex]);
        }
    }
}
