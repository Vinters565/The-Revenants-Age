using UnityEngine;

public class StaticLineDrawer : MonoBehaviour
{
    public static StaticLineDrawer Instance { get; set; }

    [SerializeField]private new LineRenderer renderer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void DrawLine(Vector3 pos1, Vector3 pos2)
    {
        renderer.positionCount = 2;
        renderer.SetPositions(new [] {pos1, pos2});
    }

    public void DeleteLine()
    {
        renderer.positionCount = 0;
    }
}
