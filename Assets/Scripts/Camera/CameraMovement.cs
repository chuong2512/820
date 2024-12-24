using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            float xMovement = Input.touches[0].deltaPosition.x;
            float yMovement = Input.touches[0].deltaPosition.y;

            gameObject.LeanMove(new Vector3(gameObject.transform.position.x - xMovement, gameObject.transform.position.y - yMovement, gameObject.transform.position.z), 0f);

            if (gameObject.transform.position.x < -40)
                gameObject.LeanMove(new Vector3(-40, gameObject.transform.position.y, gameObject.transform.position.z), 0f);
            if (gameObject.transform.position.x > 760)
                gameObject.LeanMove(new Vector3(760, gameObject.transform.position.y, gameObject.transform.position.z), 0f);
            if (gameObject.transform.position.y < 140)
                gameObject.LeanMove(new Vector3(gameObject.transform.position.x, 140, gameObject.transform.position.z), 0f);
            if (gameObject.transform.position.y > 940)
                gameObject.LeanMove(new Vector3(gameObject.transform.position.x, 940, gameObject.transform.position.z), 0f);
        }
    }
}