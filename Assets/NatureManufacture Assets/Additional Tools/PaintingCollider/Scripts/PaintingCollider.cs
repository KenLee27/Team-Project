using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace NatureManufacture.Painting
{
    public class PaintingCollider : MonoBehaviour
    {
        [SerializeField] private Color paintColor = Color.white;
        [SerializeField] private LayerMask paintLayerMask = 0;


        public Color PaintColor
        {
            get => paintColor;
            set => paintColor = value;
        }

        public LayerMask PaintLayerMask
        {
            get => paintLayerMask;
            set => paintLayerMask = value;
        }
    }
}