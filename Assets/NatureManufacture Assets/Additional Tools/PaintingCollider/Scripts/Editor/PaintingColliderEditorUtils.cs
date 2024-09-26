// /**
//  * Created by Pawel Homenko on  11/2023
//  */

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NatureManufacture.Painting.Editor
{
    // This class provides utility methods for the PaintingColliderEditor
    public static class PaintingColliderEditorUtils
    {
        // This method paints the given PaintingCollider
        public static void PaintCollider(PaintingCollider paintingCollider)
        {
            Collider collider = paintingCollider.GetComponent<Collider>();
            if (collider == null)
            {
                Debug.LogError("Collider is null");
                return;
            }

            HashSet<MeshFilter> meshFilters = FindMeshFiltersInsideColliderBounds(collider, paintingCollider.PaintLayerMask);

            if (meshFilters.Count > 0)
                PaintColors(meshFilters, collider, paintingCollider.PaintColor);
        }

// This method finds MeshFilters in the given PaintingCollider
        public static HashSet<MeshFilter> FindMeshFiltersInPainCollider(PaintingCollider paintingCollider)
        {
            var collider = paintingCollider.GetComponent<Collider>();
            if (collider == null)
            {
                Debug.LogError("Collider is null");
                return new HashSet<MeshFilter>();
            }

            var meshFilters = FindMeshFiltersInsideColliderBounds(collider, paintingCollider.PaintLayerMask);

            return meshFilters;
        }

        // This method finds MeshFilters inside the bounds of the given collider
        private static HashSet<MeshFilter> FindMeshFiltersInsideColliderBounds(Collider collider, LayerMask paintLayerMask)
        {
            HashSet<MeshFilter> meshFiltersInside = new();

            if (collider == null)
            {
                Debug.LogError("Collider is null");
                return meshFiltersInside;
            }

            // Get the bounds of the collider
            Bounds colliderBounds = collider.bounds;


            Collider[] collidersArray = Physics.OverlapBox(colliderBounds.center, colliderBounds.extents);


            foreach (Collider coll in collidersArray)
            {
                var group = coll.GetComponentInParent<LODGroup>();
                var collMeshFilter = coll.GetComponent<MeshFilter>();

                if (group == null && collMeshFilter == null)
                {
                    continue;
                }

                MeshFilter[] meshFilters = group ? group.GetComponentsInChildren<MeshFilter>() : new[] { collMeshFilter };

                foreach (MeshFilter meshFilter in meshFilters)
                {
                    if (meshFilter == null || !IsLayerInLayerMask(meshFilter.gameObject.layer, paintLayerMask)) continue;

                    meshFiltersInside.Add(meshFilter);
                }
            }

            return meshFiltersInside;
        }

        // This method checks if the given layer is in the given LayerMask
        private static bool IsLayerInLayerMask(int layer, LayerMask layerMask)
        {
            return layerMask == (layerMask | (1 << layer));
        }

        // This method paints the given MeshFilters with the given color
        private static void PaintColors(HashSet<MeshFilter> meshFilters, Collider collider, Color color)
        {
            foreach (MeshFilter meshFilter in meshFilters)
            {
                Mesh mesh = meshFilter.sharedMesh;
                Undo.RecordObject(meshFilter, "Coloring mesh change values");

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(mesh)))
                {
                    mesh = Object.Instantiate(mesh);
                    meshFilter.sharedMesh = mesh;
                }

                if (mesh == null)
                {
                    Debug.LogError($"{meshFilter.name} has no mesh", meshFilter.gameObject);
                    continue;
                }

                Vector3[] vertices = mesh.vertices;
                Color[] colors = mesh.colors;

                if (colors == null || colors.Length == 0)
                {
                    colors = new Color[vertices.Length];
                    //set all colors white
                    for (int i = 0; i < colors.Length; i++)
                    {
                        colors[i] = Color.white;
                    }
                }


                for (int i = 0; i < vertices.Length; i++)
                {
                    Vector3 worldPosition = meshFilter.transform.TransformPoint(vertices[i]);

                    if (!IsPointInsideCollider(worldPosition, collider)) continue;

                    colors[i] = color;
                    mesh.colors = colors;
                }

                EditorUtility.SetDirty(mesh);
            }
        }

        // This method checks if the given point is inside the given collider
        private static bool IsPointInsideCollider(Vector3 point, Collider collider)
        {
            Vector3 closestPoint = collider.ClosestPoint(point);

            return closestPoint == point;
        }
        
        // This method reverts overrides in the given PaintingCollider
        public static void RevertOverride(PaintingCollider paintingCollider)
        {
            HashSet<MeshFilter> meshFilters = FindMeshFiltersInPainCollider(paintingCollider);
            foreach (var meshFilter in meshFilters)
            {
                Undo.RecordObject(meshFilter, "Coloring mesh change values");
                PrefabUtility.RevertObjectOverride(meshFilter, InteractionMode.AutomatedAction);
            }
        }
    }
}
