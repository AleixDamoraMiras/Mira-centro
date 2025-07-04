using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationsUI : MonoBehaviour
{
    // Instructions - configuration container Step1
    [Header("📋 Elementos animados")]
    public List<GameObject> elementosAnimados = new List<GameObject>();

    [Header("🎬 Configuración de animación")]
    public float fadeDuration = 1.0f;
    public float delayEntreElementos = 0.3f;
    public float delayInicial = 0.5f;
    public bool autoPlayOnStart = true;

    private void Start()
    {
        if (autoPlayOnStart)
        {
            StartAnimation();
        }
    }

    public void StartAnimation()
    {
        StartCoroutine(FadeInSecuencial());
    }

    // Instructions - Function container Step1
    private IEnumerator FadeInSecuencial()
    {
        yield return new WaitForSeconds(delayInicial);

        foreach (GameObject obj in elementosAnimados)
        {
            if (obj == null) continue;

            CanvasGroup cg = obj.GetComponent<CanvasGroup>();
            if (cg == null)
            {
                Debug.LogWarning($"Elemento {obj.name} no tiene CanvasGroup.");
                continue;
            }

            float tiempo = 0f;
            while (tiempo < fadeDuration)
            {
                tiempo += Time.deltaTime;
                cg.alpha = Mathf.Clamp01(tiempo / fadeDuration);
                yield return null;
            }

            cg.interactable = true;
            cg.blocksRaycasts = true;
            yield return new WaitForSeconds(delayEntreElementos);
        }
    }
}
