using UnityEngine;

// GridOverlay draws a grid of lines over the map background.
// Lines are masked to a circle so they don't extend beyond the map.
public class GridOverlay : MonoBehaviour
{
    [Header("Grid Size - match your MapBackground scale")]
    public float mapRadius = 10f;      // Half of MapBackground scale (scale is 20, so radius is 10)

    [Header("Grid Spacing")]
    public float cellSize = 2f;        // Distance between grid lines in Unity units

    [Header("Line Appearance")]
    public float lineWidth = 0.03f;    // Thickness of each line
    public Color lineColor = new Color(1f, 1f, 1f, 0.15f);

    [Header("Sorting")]
    public int sortingOrder = 1;       // Above MapBackground (0), below crystals (2)

    void Start()
    {
        DrawGrid();
    }

    void DrawGrid()
    {
        float halfExtent = mapRadius;  // Lines extend from -radius to +radius

        // Vertical lines
        for (float x = -halfExtent; x <= halfExtent + 0.01f; x += cellSize)
        {
            // Calculate how far this vertical line reaches inside the circle
            // using the circle equation: y = sqrt(r^2 - x^2)
            float reach = GetCircleReach(x, mapRadius);
            if (reach <= 0) continue;  // Line is outside the circle entirely

            CreateLine(
                new Vector3(x, -reach, 0),
                new Vector3(x,  reach, 0)
            );
        }

        // Horizontal lines
        for (float y = -halfExtent; y <= halfExtent + 0.01f; y += cellSize)
        {
            float reach = GetCircleReach(y, mapRadius);
            if (reach <= 0) continue;

            CreateLine(
                new Vector3(-reach, y, 0),
                new Vector3( reach, y, 0)
            );
        }
    }

    // Returns how far a chord extends inside a circle at a given offset from center.
    // offset = distance from center (x position for vertical lines, y for horizontal)
    // radius = circle radius
    float GetCircleReach(float offset, float radius)
    {
        float distSq = radius * radius - offset * offset;
        if (distSq <= 0) return 0f;
        return Mathf.Sqrt(distSq);
    }

    void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("GridLine");
        lineObj.transform.SetParent(this.transform);
        lineObj.transform.localPosition = Vector3.zero;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();

        // Try URP shader first, fall back to Sprites/Default
        Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        if (shader == null)
            shader = Shader.Find("Sprites/Default");

        lr.material = new Material(shader);
        lr.startColor = lineColor;
        lr.endColor = lineColor;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.useWorldSpace = true;

        lr.sortingLayerName = "Default";
        lr.sortingOrder = sortingOrder;

        // Tell this line renderer to only draw inside the sprite mask
        lr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
    }
}