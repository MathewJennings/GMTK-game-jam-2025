using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public Material lineMaterial;
    public int gridSize = 20;
    public float cellSize = 1f;
    public float scrollSpeed = 2f;
    public Color gridColor = ColorPalette.ElectricCyan;
    public float lineWidth = 0.02f;
    
    [Header("Perspective Settings")]
    public float perspectiveDepth = 50f;
    public float fadeDistance = 30f;
    
    private LineRenderer[] horizontalLines;
    private LineRenderer[] verticalLines;
    private float currentOffset = 0f;
    
    void Start()
    {
        CreateGrid();
        SetupMaterial();
        transform.SetPositionAndRotation(new Vector3(0, -5f, 0), Quaternion.Euler(-17.7f, 0, 0));
    }

    void Update()
    {
        ScrollGrid();
    }
    
    void CreateGrid()
    {
        // Create horizontal lines
        horizontalLines = new LineRenderer[gridSize];
        for (int i = 0; i < gridSize; i++)
        {
            GameObject lineObj = new GameObject($"HorizontalLine_{i}");
            lineObj.transform.parent = transform;
            
            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.material = lineMaterial;
            lr.material.color = gridColor;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.positionCount = 2;
            lr.useWorldSpace = false;
            
            horizontalLines[i] = lr;
        }
        
        // Create vertical lines
        verticalLines = new LineRenderer[gridSize];
        for (int i = 0; i < gridSize; i++)
        {
            GameObject lineObj = new GameObject($"VerticalLine_{i}");
            lineObj.transform.parent = transform;
            
            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.material = lineMaterial;
            lr.material.color = gridColor;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.positionCount = 2;
            lr.useWorldSpace = false;
            
            verticalLines[i] = lr;
        }
    }
    
    void SetupMaterial()
    {
        if (lineMaterial == null)
        {
            // Create a default unlit material if none is assigned
            lineMaterial = new Material(Shader.Find("Unlit/Color"));
            lineMaterial.color = gridColor;
        }
    }
    
    void ScrollGrid()
    {
        currentOffset += scrollSpeed * Time.deltaTime;
        
        // Reset offset to prevent floating point precision issues
        if (currentOffset >= cellSize)
        {
            currentOffset -= cellSize;
        }
        
        UpdateGridPositions();
    }
    
    void UpdateGridPositions()
    {
        float halfGrid = (gridSize - 1) * cellSize * 0.5f;
        
        // Update horizontal lines
        for (int i = 0; i < gridSize; i++)
        {
            float z = (i * cellSize) - currentOffset;
            
            // Apply perspective effect
            float perspectiveFactor = Mathf.Clamp01(z / perspectiveDepth);
            float width = Mathf.Lerp(halfGrid * 2, halfGrid * 0.1f, perspectiveFactor);
            float y = Mathf.Lerp(0, -perspectiveDepth * 0.3f, perspectiveFactor);
            
            // Calculate alpha based on distance for fading effect
            float alpha = Mathf.Clamp01(1f - (z / fadeDistance));
            Color lineColor = new Color(gridColor.r, gridColor.g, gridColor.b, alpha);
            horizontalLines[i].material.color = lineColor;
            
            // Set line positions
            horizontalLines[i].SetPosition(0, new Vector3(-width, y, z));
            horizontalLines[i].SetPosition(1, new Vector3(width, y, z));
            
            // Hide lines that are too far away
            horizontalLines[i].enabled = z < fadeDistance && z > -cellSize;
        }
        
        // Update vertical lines
        for (int i = 0; i < gridSize; i++)
        {
            float x = (i * cellSize - halfGrid);
            
            // Create vertical lines that span the depth
            Vector3[] positions = new Vector3[gridSize];
            Color[] colors = new Color[gridSize];
            
            verticalLines[i].positionCount = gridSize;
            
            for (int j = 0; j < gridSize; j++)
            {
                float z = (j * cellSize) - currentOffset;
                
                // Apply perspective effect
                float perspectiveFactor = Mathf.Clamp01(z / perspectiveDepth);
                float adjustedX = Mathf.Lerp(x, x * 0.1f, perspectiveFactor);
                float y = Mathf.Lerp(0, -perspectiveDepth * 0.3f, perspectiveFactor);
                
                positions[j] = new Vector3(adjustedX, y, z);
                
                // Calculate alpha for fading
                float a = Mathf.Clamp01(1f - (z / fadeDistance));
                colors[j] = new Color(gridColor.r, gridColor.g, gridColor.b, a);
            }
            
            verticalLines[i].SetPositions(positions);
            
            // Apply gradient color (Unity doesn't support per-vertex colors on LineRenderer easily, 
            // so we'll use a single color based on the closest point)
            float closestZ = -currentOffset;
            float alpha = Mathf.Clamp01(1f - (closestZ / fadeDistance));
            verticalLines[i].material.color = new Color(gridColor.r, gridColor.g, gridColor.b, alpha);
            
            verticalLines[i].enabled = true;
        }
    }
    
    void OnValidate()
    {
        // Update colors and settings in editor
        if (Application.isPlaying && horizontalLines != null)
        {
            SetupMaterial();
            for (int i = 0; i < horizontalLines.Length; i++)
            {
                if (horizontalLines[i] != null)
                {
                    horizontalLines[i].material = lineMaterial;
                    horizontalLines[i].startWidth = lineWidth;
                    horizontalLines[i].endWidth = lineWidth;
                }
            }
            for (int i = 0; i < verticalLines.Length; i++)
            {
                if (verticalLines[i] != null)
                {
                    verticalLines[i].material = lineMaterial;
                    verticalLines[i].startWidth = lineWidth;
                    verticalLines[i].endWidth = lineWidth;
                }
            }
        }
    }
}
