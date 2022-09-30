using UnityEngine;

namespace HiGames.Framework.Utilities
{
    [ExecuteInEditMode]
    public class SafeAreaContainer : MonoBehaviour
    {
        public Canvas CanvasToScale;
        private RectTransform panelSafeArea;

        private Rect currentSafeArea = new Rect();
        private ScreenOrientation currentOrientation = ScreenOrientation.AutoRotation;

    
        private void Start()
        {
            panelSafeArea = GetComponent<RectTransform>();

            currentOrientation = Screen.orientation;
            currentSafeArea = Screen.safeArea;

            ApplySafeArea();
        }

        private void ApplySafeArea()
        {
            if (panelSafeArea == null)
                return;

            Rect safeArea = Screen.safeArea;

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            Rect pixelRect = CanvasToScale.pixelRect;
        
            anchorMin.x /= pixelRect.width;
            anchorMin.y /= pixelRect.height;

            anchorMax.x /= pixelRect.width;
            anchorMax.y /= pixelRect.height;

            panelSafeArea.anchorMin = anchorMin;
            panelSafeArea.anchorMax = anchorMax;

            currentOrientation = Screen.orientation;
            currentSafeArea = Screen.safeArea;

        }

    }
}