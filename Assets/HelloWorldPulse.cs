using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Attach to a TextMeshPro 3D object. Displays "Hello World" in yellow
/// with a bright white-yellow glowing outline.
/// Automatically centers the GameObject in the display, rotates horizontally,
/// and hops like a bunny every 5 seconds.
/// </summary>
[RequireComponent(typeof(TextMeshPro))]
public class HelloWorldPulse : MonoBehaviour
{
    // ── Face color ────────────────────────────────────────────────────────
    private static readonly Color FaceColor = new Color(1f, 0.92f, 0f, 1f);        // saturated yellow

    // ── Outline color & width (provides the glowing-edge look) ────────────
    private static readonly Color OutlineColor = new Color(1f, 1f, 0.55f, 1f);    // bright white-yellow
    private static readonly Color ShadowColor = new Color(0.2f, 0.2f, 0.2f, 0.8f); // dark shadow
    private const float OutlineWidth = 0.6f;   // thick outline for 3D depth

    // ── Rotation speed (degrees per second) ───────────────────────────────
    private const float RotationSpeed = 54f;  // rotate 54 degrees per second (20% faster)

    // ── Hop timing and height ─────────────────────────────────────────────
    private const float HopInterval = 5f;      // hop every 5 seconds
    private const float HopDuration = 0.5f;    // each hop takes 0.5 seconds
    private const float HopHeight = 2f;        // how high to hop

    // ── 3D Depth ───────────────────────────────────────────────────────────
    private const float ExtrusionDepth = 0.8f;   // depth of 3D letters
    private const int DepthLayers = 50;          // many layers for completely solid appearance
    private const float FontSize = 12f;          // size of 3D letters (12 point)

    private TextMeshPro _tmp;
    private Vector3 _basePosition;  // stores the resting position
    private TextMeshPro[] _depthLayers;  // array to store depth layer text meshes

    private void Start()
    {
        _tmp = GetComponent<TextMeshPro>();

        // Center the GameObject in the camera's view
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Position at camera center, 10 units in front
            transform.position = mainCamera.transform.position + mainCamera.transform.forward * 10f;
        }

        _basePosition = transform.position;  // store the resting position

        // Hide the original TextMeshPro (we'll use depth layers instead)
        _tmp.enabled = false;

        // Create 3D depth layers
        _depthLayers = new TextMeshPro[DepthLayers];
        
        for (int i = 0; i < DepthLayers; i++)
        {
            // Create child GameObject for each depth layer
            GameObject depthObj = new GameObject($"DepthLayer_{i}");
            depthObj.transform.SetParent(transform);
            depthObj.transform.localPosition = Vector3.back * (ExtrusionDepth / DepthLayers * i);
            
            // Add TextMeshPro component
            TextMeshPro depthTmp = depthObj.AddComponent<TextMeshPro>();
            _depthLayers[i] = depthTmp;
            
            // Configure the depth layer
            depthTmp.text = "Hello World";
            depthTmp.alignment = TextAlignmentOptions.Center;
            depthTmp.fontSize = FontSize;
            
            // Darker shades for depth effect
            float darkFactor = 1f - (i / (float)DepthLayers) * 0.7f;  // fade to darker
            Color depthColor = new Color(FaceColor.r * darkFactor, FaceColor.g * darkFactor, FaceColor.b * darkFactor, FaceColor.a);
            depthTmp.color = depthColor;
            
            // Only add outline to front layers
            if (i < 2)
            {
                depthTmp.outlineColor = OutlineColor;
                depthTmp.outlineWidth = OutlineWidth;
            }
            else
            {
                depthTmp.outlineWidth = 0f;
            }
        }

        StartCoroutine(HopLoop());
    }

    private void Update()
    {
        // Rotate horizontally (around Y-axis) in 3 dimensions
        transform.Rotate(0f, RotationSpeed * Time.deltaTime, 0f);
    }

    private IEnumerator HopLoop()
    {
        while (true)
        {
            // Wait before hopping
            yield return new WaitForSeconds(HopInterval);

            // Perform a bunny hop
            yield return Hop();
        }
    }

    private IEnumerator Hop()
    {
        float elapsed = 0f;
        while (elapsed < HopDuration)
        {
            elapsed += Time.deltaTime;
            // Use parabolic motion (upside-down parabola) for realistic hop
            float t = elapsed / HopDuration;  // normalized time 0→1
            float height = HopHeight * (1f - (2f * t - 1f) * (2f * t - 1f));  // parabolic arc
            
            transform.position = _basePosition + Vector3.up * height;
            yield return null;
        }
        // Snap back to base position
        transform.position = _basePosition;
    }
}
