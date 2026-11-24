using UnityEngine;
using UnityEngine.Video;

public class ActivarVideosLat : MonoBehaviour
{
    [Header("Videos secundarios")]
    public VideoPlayer videoIzquierda;
    public VideoPlayer videoDerecha;

    [Header("Renderers")]
    public Renderer rendererIzquierda;
    public Renderer rendererDerecha;

    [Header("Camara AR")]
    public Camera camaraAR;

    [Header("Zona sensible (UV)")]
    [Range(0f, 1f)] public float minX = 0.35f;
    [Range(0f, 1f)] public float maxX = 0.65f;
    [Range(0f, 1f)] public float minY = 0.35f;
    [Range(0f, 1f)] public float maxY = 0.65f;

    private bool videosActivados = false;

    void Start()
    {
        rendererIzquierda.enabled = false;
        rendererDerecha.enabled = false;

        videoIzquierda.Stop();
        videoDerecha.Stop();
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

    void DetectarToque(Vector2 posicionPantalla)
    {
        Ray rayo = camaraAR.ScreenPointToRay(posicionPantalla);

        if (Physics.Raycast(rayo, out RaycastHit hit))
        {
            if (hit.collider.gameObject.name == "ZonaInteractiva")
            {
                MostrarVideos();
            }
        }
    }

    void MostrarVideos()
    {
        videosActivados = true;

        rendererIzquierda.enabled = true;
        rendererDerecha.enabled = true;

        videoIzquierda.Play();
        videoDerecha.Play();
    }
}
