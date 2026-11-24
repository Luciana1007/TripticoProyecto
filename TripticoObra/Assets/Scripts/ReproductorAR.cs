using UnityEngine;
using UnityEngine.Video;
using Vuforia;

public class ReproductorAR : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPrincipal;

    private ObserverBehaviour observador;

    void Start()
    {
        observador = GetComponent<ObserverBehaviour>();

        if (observador != null)
        {
            observador.OnTargetStatusChanged +=
                OnTargetStatusChanged;
        }

        if (videoPrincipal != null)
        {
            videoPrincipal.Stop();
        }
    }

    void OnTargetStatusChanged(
        ObserverBehaviour behaviour,
        TargetStatus status)
    {
        bool detectado =
            status.Status == Status.TRACKED ||
            status.Status == Status.EXTENDED_TRACKED;

        if (detectado)
        {
            if (!videoPrincipal.isPlaying)
            {
                videoPrincipal.Play();
            }
        }
        else
        {
            videoPrincipal.Pause();
        }
    }

    void OnDestroy()
    {
        if (observador != null)
        {
            observador.OnTargetStatusChanged -=
                OnTargetStatusChanged;
        }
    }
}