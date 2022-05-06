using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Stop checking in update whether to zoom in after already zoomed in and vice versa
/// </summary>
[RequireComponent(typeof(Camera))]
public class CentreCamera : MonoBehaviour
{
    [SerializeField] Camera sceneCamera;
    
    [Space]
    [SerializeField]
    float zoomSpeed = .02f;
    [SerializeField]
    float moveSpeed = .03f;

    [Space]
    [SerializeField]
    Rect miniScreen = new Rect(.33f, .33f, .33f, .33f);


    public event System.Action OnZoomIn;
    public event System.Action OnZoomOut;
    
    public UnityEvent OnStartZoomIn;
    public UnityEvent OnEndZoomIn;
    public UnityEvent OnStartZoomOut;
    public UnityEvent OnEndZoomOut;

    public bool isFullscreen;
    public bool isMiniscreen;

    private void Start()
    {
        sceneCamera.rect = miniScreen;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Checks if the mouse is over the scene camera (not the main game camera)
            if (sceneCamera.pixelRect.Contains(Input.mousePosition))
            {
                StartCoroutine(ZoomIn());
                StartCoroutine(Move(Vector2.zero));
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(ZoomOut());
            StartCoroutine(Move(miniScreen.position));
        }
    }

    private IEnumerator ZoomIn() 
    {
        OnStartZoomIn?.Invoke();

        while (sceneCamera.rect.width < 1 && sceneCamera.rect.height < 1)
        {
            Rect newCam = sceneCamera.rect;
            newCam.size += Vector2.one * zoomSpeed;
            newCam.width = Mathf.Clamp(newCam.width, miniScreen.width, 1);
            newCam.height = Mathf.Clamp(newCam.height, miniScreen.height, 1);
            sceneCamera.rect = newCam;
            yield return new WaitForFixedUpdate();
        }

        isMiniscreen = false;
        isFullscreen = true;
        OnEndZoomIn?.Invoke();
        OnZoomIn?.Invoke();
    }
    private IEnumerator ZoomOut()
    {
        OnStartZoomOut?.Invoke();
        OnZoomOut?.Invoke();

        while (sceneCamera.rect.size != miniScreen.size)
        {
            Rect newCam = sceneCamera.rect;
            newCam.size -= Vector2.one * zoomSpeed;
            newCam.width = Mathf.Clamp(newCam.width, miniScreen.width, 1);
            newCam.height = Mathf.Clamp(newCam.height, miniScreen.height, 1);
            sceneCamera.rect = newCam;
            yield return new WaitForFixedUpdate();
        }

        isFullscreen = false;
        isMiniscreen = true;
        OnEndZoomOut?.Invoke();
    }
    private IEnumerator Move(Vector2 dest)
    {
        Vector2 camDir = dest - sceneCamera.rect.position;
        while (dest != sceneCamera.rect.position)
        {
            Rect newCam = sceneCamera.rect;
            newCam.position += camDir * moveSpeed;
            newCam.x = Mathf.Clamp(newCam.x, 0, miniScreen.x);
            newCam.y = Mathf.Clamp(newCam.y, 0, miniScreen.y);
            sceneCamera.rect = newCam;
            yield return new WaitForFixedUpdate();
        }
    }

    public void SetFOV(float value) 
    {
        sceneCamera.fieldOfView = value;
    }
}
