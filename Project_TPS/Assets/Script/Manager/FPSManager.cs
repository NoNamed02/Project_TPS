using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    private int frameCount = 0;
    private float deltaTime = 0.0f;
    private float fps = 0.0f;
    private const float updateRate = 0.5f; // FPS 업데이트 주기 (초 단위)

    void Update()
    {
        frameCount++;
        deltaTime += Time.unscaledDeltaTime;

        if (deltaTime > updateRate)
        {
            fps = frameCount / deltaTime;
            frameCount = 0;
            deltaTime -= updateRate;
        }
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle
        {
            fontSize = 20,
            normal = { textColor = Color.white }
        };

        Rect rect = new Rect(10, 10, 200, 50);
        GUI.Label(rect, $"FPS: {fps:F1}", style);
    }
}
