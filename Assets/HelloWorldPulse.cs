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
    /// <summary>
    /// SECTION: Color Configuration
    /// Sets up the visual styling for the "Hello World" text.
    /// FaceColor: The main text color - a saturated yellow (RGB: 1, 0.92, 0, 1).
    /// OutlineColor: A brighter white-yellow shade (RGB: 1, 1, 0.55, 1) applied to text borders for a glowing effect.
    /// ShadowColor: A dark gray (RGB: 0.2, 0.2, 0.2, 0.8) for depth and shadow effects.
    /// OutlineWidth: Set to 0.6 for a thick, visible outline that enhances the 3D appearance.
    /// </summary>
    // ── Face color ────────────────────────────────────────────────────────
    // PRIVATE: Only THIS class needs this color; no external code should change it.
    // STATIC: Shared across all HelloWorldPulse instances (memory efficient).
    // READONLY: Set once at declaration, never changes—pure data configuration.
    private static readonly Color FaceColor = new Color(1f, 0.92f, 0f, 1f);        // saturated yellow

    // ── Outline color & width (provides the glowing-edge look) ────────────
    // PRIVATE STATIC READONLY: Shared configuration; all instances use the same glow color.
    private static readonly Color OutlineColor = new Color(1f, 1f, 0.55f, 1f);    // bright white-yellow
    // PRIVATE STATIC READONLY: Shared shadow color for all depth layers.
    private static readonly Color ShadowColor = new Color(0.2f, 0.2f, 0.2f, 0.8f); // dark shadow
    // PRIVATE CONST: A compile-time constant—hardcoded into the code itself.
    // CONST values are faster and never change, perfect for configuration.
    private const float OutlineWidth = 1.6f;   // thick outline for 3D depth

    /// <summary>
    /// SECTION: Rotation Configuration
    /// Controls the horizontal spinning animation of the text.
    /// RotationSpeed: 54 degrees per second - roughly 1/7th of a full rotation per second.
    /// This is applied around the Y-axis (vertical axis) creating a continuous spinning effect.
    /// </summary>
    // ── Rotation speed (degrees per second) ───────────────────────────────
    // PRIVATE: Animation parameter used only internally by Update().
    // CONST: A fixed game balance value—compile-time constant, cannot be modified at runtime.
    private const float RotationSpeed = 54f;  // rotate 54 degrees per second (20% faster)

    /// <summary>
    /// SECTION: Hop/Jump Animation Configuration
    /// Defines the timing and motion parameters for the bunny-hop style animation.
    /// HopInterval: 5 seconds between each hop (bunny hops once every 5 seconds).
    /// HopDuration: Each individual hop lasts 0.5 seconds (quick, snappy animation).
    /// HopHeight: The text jumps 2 units high in world space during each hop.
    /// </summary>
    // ── Hop timing and height ─────────────────────────────────────────────
    // PRIVATE CONST: Animation timing—used by HopLoop() coroutine, never changes.
    // CONST = compile-time constant, zero runtime overhead.
    private const float HopInterval = 5f;      // hop every 5 seconds
    // PRIVATE CONST: Duration of a single hop animation.
    private const float HopDuration = 0.5f;    // each hop takes 0.5 seconds
    // PRIVATE CONST: Maximum height reached during a hop.
    private const float HopHeight = 2f;        // how high to hop

    /// <summary>
    /// SECTION: 3D Depth/Extrusion Configuration
    /// These parameters control how "thick" or 3D the text appears by creating layered copies.
    /// ExtrusionDepth: Total depth in world units (0.8 units deep - moderate thickness).
    /// DepthLayers: Number of individual text mesh layers (50 layers create smooth 3D appearance).
    /// FontSize: Base size of each text layer (12 point font).
    /// The combination creates a 3D extrusion effect through stacked layers with color fading.
    /// </summary>
    // ── 3D Depth ───────────────────────────────────────────────────────────
    // PRIVATE CONST: 3D rendering configuration—fixed formula for depth effect.
    private const float ExtrusionDepth = 0.8f;   // depth of 3D letters
    // PRIVATE CONST: Number of depth layers. Const ensures consistency across all runs.
    private const int DepthLayers = 50;          // many layers for completely solid appearance
    // PRIVATE CONST: Font size configuration—a gameplay constant, not meant to change.
    private const float FontSize = 12f;          // size of 3D letters (12 point)

    // PRIVATE FIELD: Reference to the TextMeshPro component. Private because only THIS script uses it.
    // Prefixed with _ by convention to indicate it's a private member variable.
    // NOT const because it's assigned at runtime (in Start).
    private TextMeshPro _tmp;
    // PRIVATE FIELD: Stores the resting/idle position. Changes during hop, so NOT const.
    // Only needed internally—no reason for external code to access it directly.
    private Vector3 _basePosition;  // stores the resting position
    // PRIVATE FIELD: Array of child TextMeshPro components (one per depth layer).
    // Private because this is an internal implementation detail of the 3D effect.
    private TextMeshPro[] _depthLayers;  // array to store depth layer text meshes

    /// <summary>
    /// SECTION: Member Variables / Private Fields
    /// These store data needed throughout the object's lifetime.
    /// _tmp: Reference to the TextMeshPro component (cached for performance).
    /// _basePosition: Stores the resting/idle position - the point the text returns to after hopping.
    /// _depthLayers: Array of 50 TextMeshPro components, each positioned slightly further back,
    ///               creating the illusion of thick 3D text through layering and color gradients.
    /// </summary>

    /// <summary>
    /// SECTION: Start() Method - Initialization
    /// Called once when the script first runs (before first frame). Performs one-time setup:
    /// 
    /// 1. Gets the TextMeshPro component attached to this GameObject and caches it in _tmp.
    /// 2. Positions the GameObject in front of the camera at the center of the view (10 units forward).
    /// 3. Stores this position as _basePosition for the hopping animation to return to.
    /// 4. Hides the original TextMeshPro component since we'll use custom depth layers instead.
    /// 5. Creates 50 depth layers:
    ///    - Each layer is a child GameObject positioned progressively further back (Z-axis).
    ///    - Each layer gets its own TextMeshPro component displaying "Hello World".
    ///    - Colors fade to darker shades as layers go deeper (creating depth perception).
    ///    - Only the front 2 layers get the bright outline to avoid visual clutter.
    /// 6. Starts the HopLoop coroutine to begin the hopping animation cycle.
    /// </summary>
    // PRIVATE METHOD: Called once by Unity when the script initializes.
    // Private because Unity automatically calls this—no external code should invoke it.
    // VOID: No return value; it performs setup actions, not calculations.
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

    /// <summary>
    /// SECTION: Update() Method - Continuous Rotation
    /// Called every frame while the script is enabled. Handles the continuous spinning animation:
    /// Rotates the entire GameObject (and all its child depth layers) around the Y-axis
    /// (vertical axis) at a constant RotationSpeed of 54 degrees per second.
    /// Formula: transform.Rotate(X, Y, Z) where Y is multiplied by Time.deltaTime to ensure
    /// frame-rate-independent smooth rotation (adapts to any frame rate).
    /// </summary>
    // PRIVATE METHOD: Called every frame by Unity.
    // Private because Unity automatically calls this—external code shouldn't need to call it.
    // VOID: No return value; it rotates the object as a side effect.
    private void Update()
    {
        // Rotate horizontally (around Y-axis) in 3 dimensions
        transform.Rotate(0f, RotationSpeed * Time.deltaTime, 0f);
    }

    /// <summary>
    /// SECTION: HopLoop() Coroutine - Animation Loop Controller
    /// A coroutine that runs indefinitely, managing the timing of the hopping animation.
    /// This creates a repeating cycle:
    /// 1. Waits HopInterval seconds (5 seconds) - giving time between hops.
    /// 2. Calls the Hop() coroutine to execute a single hop animation.
    /// 3. Repeats forever.
    /// This produces the effect of the text hopping like a bunny every 5 seconds.
    /// Running as a coroutine allows this loop to pause and resume timing smoothly.
    /// </summary>
    // PRIVATE METHOD: A coroutine that runs the hop animation loop indefinitely.
    // Private because only Start() calls this; it's an internal implementation detail.
    // IENUMERATOR: Allows code to "pause" and resume (using yield), perfect for timed animations.
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

    /// <summary>
    /// SECTION: Hop() Coroutine - Parabolic Jump Animation
    /// Animates a single "hop" by moving the text upward then back down over HopDuration (0.5 seconds).
    /// Uses parabolic motion (mathematical curve) for realistic physics-based movement:
    /// 1. elapsed time is tracked and normalized to a 0→1 range (t = elapsed / HopDuration).
    /// 2. The height calculation uses (1 - (2t - 1)²) which produces an upside-down parabola:
    ///    - At t=0, height = 0 (starts at base position).
    ///    - At t=0.5, height = HopHeight (peaks at 2 units).
    ///    - At t=1, height = 0 (returns to base).
    /// 3. Updates position each frame: basePosition + vertical offset.
    /// 4. After the hop completes, snaps the position back to _basePosition (prevents drift).
    /// The parabolic curve creates natural-looking gravity and acceleration/deceleration.
    /// </summary>
    // PRIVATE METHOD: A coroutine that animates a single hop upward then downward.
    // Private because only HopLoop() calls this; it's part of the internal animation system.
    // IENUMERATOR: Allows the animation to spread across multiple frames using yield.
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
        _basePosition = _basePosition*1.2f;
        transform.position = _basePosition;
    }
}
