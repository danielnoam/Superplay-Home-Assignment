using UnityEngine;

public class RotateEffect : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 7f;

    private void Update()
    {
        target.Rotate(0f, 0f, speed * Time.deltaTime);
    }
}