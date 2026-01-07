using System.Collections;
using UnityEngine;

public class BloodScreen : MonoBehaviour
{
    public static BloodScreen Instance;

    public CanvasGroup bloodCanvasGroup; 
    private float fadeDuration = 5f;
    private float maxAlpha = 0.6f; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (bloodCanvasGroup == null)
        {
            bloodCanvasGroup = GetComponent<CanvasGroup>();
        }
    }

    private void Start()
    {
        if (bloodCanvasGroup != null)
        {
            bloodCanvasGroup.alpha = 0f;
        }
    }

    public void ShowDeathEffect()
    {
        if (bloodCanvasGroup != null)
        {
            bloodCanvasGroup.alpha = 0f;
            StartCoroutine(FadeInBlood());
        }
    }

    private IEnumerator FadeInBlood()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / fadeDuration);
            bloodCanvasGroup.alpha = Mathf.Lerp(0f, maxAlpha, progress);
            yield return null;
        }

        bloodCanvasGroup.alpha = maxAlpha;
    }
}
