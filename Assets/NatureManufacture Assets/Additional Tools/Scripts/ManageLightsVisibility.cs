using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages the visibility of light sources based on their distance from the camera.
/// </summary>
public class ManageLightsVisibility : MonoBehaviour
{
    
    /// <summary>
    /// List of light sources to manage.
    /// </summary>
    [Tooltip("List of light sources to manage.")]
    public List<Light> lightSources;

    /// <summary>
    /// Distance at which shadows are culled.
    /// </summary>
    [Tooltip("Distance at which shadows are culled.")]
    public float shadowCullingDistance = 30;

    /// <summary>
    /// Distance at which lights are culled.
    /// </summary>
    [Tooltip("Distance at which lights are culled.")]
    public float lightCullingDistance = 100;

    private CullingGroup cullingGroup;
    private BoundingSphere[] boundingSpheres;

    /// <summary>
    /// Array to remember the original shadow settings of each light source.
    /// </summary>
    private LightShadows[] originalShadows;

    private void Start()
    {
        cullingGroup = new CullingGroup();

        // If there's no main camera, exit early
        if (Camera.main == null) return;

        cullingGroup.targetCamera = Camera.main;

        boundingSpheres = new BoundingSphere[lightSources.Count];
        originalShadows = new LightShadows[lightSources.Count];

        // Initialize bounding spheres and remember original shadow settings
        for (int i = 0; i < lightSources.Count; i++)
        {
            boundingSpheres[i] = new BoundingSphere(lightSources[i].transform.position, 1);
            originalShadows[i] = lightSources[i].shadows;
        }

        cullingGroup.SetBoundingSpheres(boundingSpheres);
        cullingGroup.SetBoundingSphereCount(lightSources.Count);
        cullingGroup.SetBoundingDistances(new[] { shadowCullingDistance, lightCullingDistance });
        cullingGroup.SetDistanceReferencePoint(Camera.main.transform);
        cullingGroup.onStateChanged += OnStateChanged;
    }

    /// <summary>
    /// Callback for when the state of a bounding sphere changes.
    /// Adjusts the shadow settings or disables the light source based on the current distance.
    /// </summary>
    private void OnStateChanged(CullingGroupEvent sphere)
    {
        // If the current distance hasn't changed, exit early
        if (sphere.currentDistance == sphere.previousDistance) return;

        // If the current distance is between 0 and 1, disable shadows but keep the light source enabled
        if (sphere.currentDistance is > 0 and <= 1)
        {
            lightSources[sphere.index].shadows = LightShadows.None;
            lightSources[sphere.index].enabled = true;
        }
        // If the current distance is greater than 1, disable the light source
        else if (sphere.currentDistance > 1)
        {
            lightSources[sphere.index].enabled = false;
        }
        // If the current distance is 0, restore the original shadow settings
        else
        {
            lightSources[sphere.index].shadows = originalShadows[sphere.index];
        }
    }

    /// <summary>
    /// Callback for when the object is destroyed.
    /// Disposes of the CullingGroup.
    /// </summary>
    private void OnDestroy()
    {
        if (cullingGroup != null)
        {
            cullingGroup.Dispose();
            cullingGroup = null;
        }
    }
}