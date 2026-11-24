using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class ActivarVideosLat : MonoBehaviour
{
    [Header("Objetos laterales")]
    public GameObject objetoIzquierda;
    public GameObject objetoDerecha;

    [Header("Videos secundarios")]
    public VideoPlayer videoIzquierda;
    public VideoPlayer videoDerecha;

    [Header("Renderers")]
    public Renderer rendererIzquierda;
    public Renderer rendererDerecha;

    [Header("Zona interactiva")]
    public Collider zonaInteractiva;

    [Header("Camara AR")]
    public Camera camaraAR;

    [Header("Animacion")]
    public float duracionAparicion = 1.5f;

    private bool videosActivados = false;

    void Awake()
    {
        if (objetoIzquierda != null)
            objetoIzquierda.SetActive(false);

        if (objetoDerecha != null)
            objetoDerecha.SetActive(false);
    }

    void Update()
    {
        if (videosActivados)
            return;


        if (Input.touchCount > 0 &&
            Input.GetTouch(0).phase == TouchPhase.Began)
        {
            DetectarToque(Input.GetTouch(0).position);
        }

        if (Input.GetMouseButtonDown(0))
        {
            DetectarToque(Input.mousePosition);
        }
    }

    void DetectarToque(Vector2 posicion)
    {
        Ray rayo = camaraAR.ScreenPointToRay(posicion);

        if (Physics.Raycast(rayo, out RaycastHit hit))
        {
            if (zonaInteractiva != null &&
                hit.collider == zonaInteractiva)
            {
                MostrarVideos();
            }
        }
    }

    void MostrarVideos()
    {
         videosActivados = true;

        objetoIzquierda.SetActive(true);
        objetoDerecha.SetActive(true);

        StartCoroutine(
            PrepararYMostrar(videoIzquierda, rendererIzquierda)
        );

        StartCoroutine(
            PrepararYMostrar(videoDerecha, rendererDerecha)
        );
    }

    IEnumerator PrepararYMostrar(
        VideoPlayer video,
        Renderer quad)
    {
        if (video == null || quad == null)
            yield break;

        
        quad.enabled = false;

        
        video.Stop();
        video.Prepare();

        while (!video.isPrepared)
            yield return null;

        
        bool primerFotograma = false;

        video.sendFrameReadyEvents = true;

        VideoPlayer.FrameReadyEventHandler evento =
            (source, frame) => primerFotograma = true;

        video.frameReady += evento;

        video.Play();

        float espera = 0f;

        while (!primerFotograma && espera < 5f)
        {
            espera += Time.unscaledDeltaTime;
            yield return null;
        }

        video.frameReady -= evento;

        if (!primerFotograma)
        {
            Debug.LogWarning(
                "No se recibio el primer fotograma de: " + video.name
            );
            yield break;
        }

        
        Material material = quad.material;

        
        CambiarAlpha(material, 0f);

        
        quad.enabled = true;

        
        float tiempo = 0f;

        while (tiempo < duracionAparicion)
        {
            tiempo += Time.unscaledDeltaTime;

            float alpha = Mathf.Clamp01(
                tiempo / duracionAparicion
            );

            CambiarAlpha(material, alpha);

            yield return null;
        }

        
        CambiarAlpha(material, 1f);
    }

    void CambiarAlpha(Material material, float alpha)
    {
        Color color;

        if (material.HasProperty("_BaseColor"))
        {
            color = material.GetColor("_BaseColor");
            color.a = alpha;
            material.SetColor("_BaseColor", color);
        }
        else if (material.HasProperty("_Color"))
        {
            color = material.GetColor("_Color");
            color.a = alpha;
            material.SetColor("_Color", color);
        }
    }
}
