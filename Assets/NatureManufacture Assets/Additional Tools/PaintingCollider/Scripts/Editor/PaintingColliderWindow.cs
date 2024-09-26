using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NatureManufacture.Painting.Editor
{
    // This class provides a custom editor window for managing PaintingCollider objects
    public class PaintingColliderWindow : EditorWindow
    {
        [MenuItem("Tools/Nature Manufacture/Painting Collider Manager")]
        private static void OpenWindow()
        {
            GetWindow<PaintingColliderWindow>("Paint Collider Manager");
        }

        private void OnGUI()
        {
            GUILayout.Label("Paint Collider Window", EditorStyles.boldLabel);

            if (GUILayout.Button("Paint All PaintColliders"))
            {
                PaintAllPaintColliders();
            }

            if (GUILayout.Button("Paint Selected PaintColliders"))
            {
                PainSelectedColliders();
            }

            EditorGUILayout.Space();
            
            if (GUILayout.Button("Revert Override In All Paint Collider Objects"))
            {
                RevertOverrideAll();
            }

            if (GUILayout.Button("Revert Override In Selected Paint Collider Objects"))
            {
                RevertOverrideSelected();
            }
        }

        // This method reverts overrides in all PaintingCollider objects
        private void RevertOverrideAll()
        {
            PaintingCollider[] paintingColliders = FindObjectsOfType<PaintingCollider>();
            RevertOverrides(paintingColliders);
        }

        // This method reverts overrides in selected PaintingCollider objects
        private void RevertOverrideSelected()
        {
            PaintingCollider[] paintingColliders = Selection.GetFiltered<PaintingCollider>(SelectionMode.Deep);
            RevertOverrides(paintingColliders);
        }

        // This method paints selected PaintingCollider objects
        private void PainSelectedColliders()
        {
            PaintingCollider[] paintingColliders = Selection.GetFiltered<PaintingCollider>(SelectionMode.Deep);

            PaintPaintColliders(paintingColliders);
        }

        // This method reverts overrides in the given PaintingCollider objects
        private static void RevertOverrides(PaintingCollider[] paintingColliders)
        {
            foreach (PaintingCollider paintingCollider in paintingColliders)
            {
                PaintingColliderEditorUtils.RevertOverride(paintingCollider);
            }
        }

        // This method paints all PaintingCollider objects
        private void PaintAllPaintColliders()
        {
            PaintingCollider[] paintingColliders = FindObjectsOfType<PaintingCollider>();

            PaintPaintColliders(paintingColliders);
        }

        // This method paints the given PaintingCollider objects
        private void PaintPaintColliders(PaintingCollider[] paintingColliders)
        {
            foreach (var paintingCollider in paintingColliders)
            {
                if (paintingCollider != null)
                {
                    PaintingColliderEditorUtils.PaintCollider(paintingCollider);
                }
            }
        }
    }
}