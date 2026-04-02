using UnityEngine;
using UnityEngine.UI;

public class BreatheEffect : MonoBehaviour
{
    [SerializeField] private Image target;
    [SerializeField] private float frequency = 2f;
    [SerializeField, Range(0f, 1f)] private float minAlpha;
    [SerializeField, Range(0f, 1f)] private float maxAlpha = 1f;

    private void Update()
    {
        var t = (Mathf.Sin(Time.time * frequency * Mathf.PI * 2f) + 1f) * 0.5f;
        var a = Mathf.Lerp(minAlpha, maxAlpha, t);
        target.color = new Color(target.color.r, target.color.g, target.color.b, a);
    }
}