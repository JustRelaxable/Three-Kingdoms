using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] float cameraWASDspeed;
    [SerializeField] Transform target;
    [SerializeField] Transform cameraRigTransform;
    [SerializeField] Transform camZoomInitialPos;
    Vector3 mousePosition;
    const float SCREEN_WIDTH_PERCENTAGE = 0.10f;
    //public PlayerNetworkManager playerNetworkManager;

    float zoomOutFactor = 0;
    float zoomInFactor = 0;
    float velocity = 0f;
    Vector3 zoomStartPos;
    Vector3 furtherPosCamAngle;
    Vector3 closePosCamAngle;
    void Start()
    {
        zoomStartPos = transform.position;
        furtherPosCamAngle = new Vector3(80,0,0);
        closePosCamAngle = new Vector3(45, 0, 0);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //if (playerNetworkManager.isLocalPlayer)
        {
            var mouseScrool = Input.GetAxis("Mouse ScrollWheel");
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");

            if (mouseScrool > 0) // yakınlaşma
            {
                /*
                target.transform.parent = TemporaryParent.transform;
                camZoomInitialPos.parent = TemporaryParent.transform;
                */
                zoomOutFactor = 0;

                zoomInFactor += Time.deltaTime;
                //transform.position = Vector3.Lerp(transform.position, zoomStartPos, zoomInFactor);
                transform.position = Vector3.Lerp(transform.position, camZoomInitialPos.position, zoomInFactor);

                //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, closePosCamAngle, zoomInFactor);
                float xAxis = Mathf.Lerp(transform.eulerAngles.x, closePosCamAngle.x, zoomInFactor);
                transform.eulerAngles = new Vector3(xAxis, transform.eulerAngles.y, transform.eulerAngles.z);
            }

            else if (mouseScrool < 0)   // uzaklaşma
            {   /*
            target.transform.parent = TemporaryParent.transform;
            camZoomInitialPos.transform.parent = TemporaryParent.transform;
            */
                zoomInFactor = 0;

                zoomOutFactor += Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, target.position, zoomOutFactor);

                //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, furtherPosCamAngle, zoomOutFactor);
                float xAxis = Mathf.Lerp(transform.eulerAngles.x, furtherPosCamAngle.x, zoomOutFactor);
                transform.eulerAngles = new Vector3(xAxis, transform.eulerAngles.y, transform.eulerAngles.z);
                //Debug.Log(xAxis);
            }

            else
            {
                zoomOutFactor -= Time.deltaTime / 10;
                zoomInFactor -= Time.deltaTime / 10;
                zoomOutFactor = Mathf.Clamp(zoomOutFactor, 0, Mathf.Infinity);
                zoomInFactor = Mathf.Clamp(zoomInFactor, 0, Mathf.Infinity);
                if(zoomOutFactor > 0)
                {
                    transform.position = Vector3.Lerp(transform.position, target.position, zoomOutFactor);
                    //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, furtherPosCamAngle, zoomOutFactor);
                    float xAxis = Mathf.Lerp(transform.eulerAngles.x, furtherPosCamAngle.x, zoomOutFactor);
                    transform.eulerAngles = new Vector3(xAxis, transform.eulerAngles.y, transform.eulerAngles.z);
                }
                else if(zoomInFactor > 0)
                {

                    transform.position = Vector3.Lerp(transform.position, camZoomInitialPos.position, zoomInFactor);
                    //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, closePosCamAngle, zoomInFactor);
                    float xAxis = Mathf.Lerp(transform.eulerAngles.x, closePosCamAngle.x, zoomInFactor);
                    transform.eulerAngles = new Vector3(xAxis, transform.eulerAngles.y, transform.eulerAngles.z);
                }
            }

            if (horizontal != 0)
            {
                cameraRigTransform.position += transform.right * horizontal * Time.deltaTime * cameraWASDspeed;
            }
            if (vertical != 0)
            {
                Vector3 forwardVector = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
                Debug.Log(Vector3.Dot(cameraRigTransform.forward,transform.forward));
                cameraRigTransform.position += forwardVector * vertical * Time.deltaTime * cameraWASDspeed;
            }
        }
    }

    private void Update()
    {
        mousePosition = Input.mousePosition;
        RotateCamera();
    }

    public void RotateCamera()
    {
        float percentage = Screen.width * SCREEN_WIDTH_PERCENTAGE;
        if(mousePosition.x > Screen.width - percentage)
        {
            transform.parent.Rotate(Vector3.up, 1f, Space.World);
        }
        else if(mousePosition.x < percentage)
        {
            transform.parent.Rotate(Vector3.up, -1f, Space.World);
        }
    }
}
