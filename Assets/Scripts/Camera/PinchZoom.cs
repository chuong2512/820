using UnityEngine;

namespace Version3
{
    public class PinchZoom : MonoBehaviour
    {
        public static float MaxZoom = 100f;
        public static float StdZoom = 1.44f;
        public static float MinZoom = 60f;

        public Camera targetCamera;
        public float initialFieldOfViewSize;

        public float panSpeedAdjustment = 1.0f;
        public float currentFieldOfViewSize = 0f;
        public float lastDistance = 0f;

        private void Awake()
        {
            initialFieldOfViewSize = targetCamera.fieldOfView;
        }

        public void ChangeCamera(Camera newTargetCamera)
        {
            float newInitialSize = newTargetCamera.fieldOfView;
            newTargetCamera.fieldOfView = targetCamera.fieldOfView;
            targetCamera.fieldOfView = initialFieldOfViewSize;
            initialFieldOfViewSize = newInitialSize;
        }

        private void Update()
        {
            if (Input.touchCount >= 2)
            {
                //twoTouches = true;
                Vector2 touch0, touch1;
                float distance;
                touch0 = Input.GetTouch(0).position;
                touch1 = Input.GetTouch(1).position;
                distance = Vector2.Distance(touch0, touch1);
                if (lastDistance == 0f)
                {
                    lastDistance = distance;
                    currentFieldOfViewSize = targetCamera.fieldOfView;
                }
                else
                {
                    if (distance > lastDistance)
                    {
                        // zoom in
                        float multiplier = lastDistance / distance;
                        float newOrthoSize = currentFieldOfViewSize * multiplier;
                        if (newOrthoSize >= MinZoom)
                        {
                            targetCamera.fieldOfView = newOrthoSize;
                            AdjustPanSpeed(newOrthoSize);
                        }
                        else
                        {
                            targetCamera.fieldOfView = MinZoom;
                            AdjustPanSpeed(newOrthoSize);
                        }
                    }
                    else
                    {
                        float multiplier = lastDistance / distance;
                        float newOrthoSize = currentFieldOfViewSize * multiplier;
                        if (newOrthoSize <= MaxZoom)
                        {
                            targetCamera.fieldOfView = newOrthoSize;
                            AdjustPanSpeed(newOrthoSize);
                        }
                        else
                        {
                            targetCamera.fieldOfView = MaxZoom;
                            AdjustPanSpeed(newOrthoSize);
                        }
                    }
                }
            }
            else
            {
                lastDistance = 0f;
                currentFieldOfViewSize = 0f;
                AdjustPanSpeed(targetCamera.fieldOfView);
            }
        }


        // Exists for other zoom changing functions in other places.
        public void AdjustPanSpeed(float _fieldOfView)
        {
            if (_fieldOfView <= StdZoom)
            {
                panSpeedAdjustment = _fieldOfView / StdZoom; // slows down panning
            }
            else
            {
                panSpeedAdjustment = 1.0f; // no speed change
            }
        }
    }
}