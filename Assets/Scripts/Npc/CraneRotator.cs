using System.Collections;
using UnityEngine;

public class CraneRotator : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float angle = 180f;
    [SerializeField] private float pauseTime = 1f;

    private IEnumerator Start()
    {
        float dir = 1f;

        while (true)
        {
            float rotated = 0f;

            while (rotated < angle)
            {
                float step = Mathf.Min(speed * Time.deltaTime, angle - rotated);
                transform.Rotate(0f, step * dir, 0f);
                rotated += step;
                yield return null;
            }

            yield return new WaitForSeconds(pauseTime);
            dir = -dir;
        }
    }
}