using UnityEngine;

public class GridOverlay : MonoBehaviour
{
    [Header("Grid Size")]
    public float mapRadius = 10f;

    [Header("Grid Spacing")]
    public float cellSize = 2f;

    [Header("Line Appearance")]
    public float lineWidth = 0.03f;
    public Color lineColor = new Color(0f, 0.8f, 0.2f, 0.4f);

    [Header("Sorting")]
    public int sortingOrder = 1;

    void Start()
    {
        DrawGrid();
    }

    void DrawGrid()
    {
        float half = mapRadius;

        for (float x = -half; x <= half + 0.01f; x += cellSize)
        {
            float reach = CircleReach(x, mapRadius);
            if (reach <= 0) continue;
            CreateLine(new Vector3(x, -reach, 0), new Vector3(x, reach, 0));
        }

        for (float y = -half; y <= half + 0.01f; y += cellSize)
        {
            float reach = CircleReach(y, mapRadius);
            if (reach <= 0) continue;
            CreateLine(new Vector3(-reach, y, 0), new Vector3(reach, y, 0));
        }
    }

    float CircleReach(float offset, float radius)
    {
        float d = radius * radius - offset * offset;
        return d <= 0 ? 0f : Mathf.Sqrt(d);
    }

    void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject obj = new GameObject("GridLine");
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;

        LineRenderer lr = obj.AddComponent<LineRenderer>();

        Shader s = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        if (s == null) s = Shader.Find("Sprites/Default");
        lr.material = new Material(s);

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
        lr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
    }
}