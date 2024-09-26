namespace NatureManufacture.Painting.Editor
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    // Custom editor for the PaintingCollider component
    [CustomEditor(typeof(PaintingCollider))]
    public class PaintingColliderEditor : Editor
    {
        // Reference to the PaintingCollider component
        private PaintingCollider _paintingCollider;

        public override void OnInspectorGUI()
        {
            _paintingCollider = (PaintingCollider)target;

            _paintingCollider.PaintColor = EditorGUILayout.ColorField("Paint Color", _paintingCollider.PaintColor);

            _paintingCollider.PaintLayerMask = EditorGUILayout.MaskField("Paint Layer Mask", _paintingCollider.PaintLayerMask, UnityEditorInternal.InternalEditorUtility.layers);

            // Get the MeshCollider component
            MeshCollider meshCollider = _paintingCollider.GetComponent<MeshCollider>();

            // If the MeshCollider is not convex, display a warning
            if (meshCollider != null && !meshCollider.convex)
                EditorGUILayout.HelpBox("Mesh Collider is not convex. Painting will not work correctly.", MessageType.Error);

            if (GUILayout.Button("Paint"))
            {
                // Call the PaintCollider method when the button is clicked
                PaintingColliderEditorUtils.PaintCollider(_paintingCollider);
            }

            if (GUILayout.Button("Revert Override"))
            {
                // Call the RevertOverride method when the button is clicked
                PaintingColliderEditorUtils.RevertOverride(_paintingCollider);
            }
        }
    }
}