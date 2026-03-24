using UnityEngine;

public class ScaleScreenMobile : MonoBehaviour
{
    [SerializeField] private GameObject backgroundImage, dogImage;
    [SerializeField] private Camera mainCam;

    private float deviceScreenWidth, deviceScreenHeight;

    private void Start()
    {
        FitToScreenMobile();
    }

    private void FitToScreenMobile()
    {
        // Get device screen aspect

        deviceScreenWidth = Screen.width;
        deviceScreenHeight = Screen.height;

        Vector2 deviceScreenResolution = new Vector2(deviceScreenWidth, deviceScreenHeight);

        print(deviceScreenResolution);

        float deviceScreenAspect = deviceScreenWidth / deviceScreenHeight;

        // Set main camera's aspect = device's aspect

        mainCam.aspect = deviceScreenAspect;

        // Scale background image to fit with camera size

        float camheight = 100.0f * mainCam.orthographicSize * 2.1f;
        float camWidth = camheight * deviceScreenAspect;

        // Get background image size;

        SpriteRenderer backgroundImageSR = backgroundImage.GetComponent<SpriteRenderer>();
        float bgImageHeight = backgroundImageSR.sprite.rect.height;
        float bgImageWidth = backgroundImageSR.sprite.rect.width;

        SpriteRenderer dogImageSR = dogImage.GetComponent<SpriteRenderer>();
        float dogImageHeight = dogImageSR.sprite.rect.height;
        float dogImageWidth = dogImageSR.sprite.rect.width;

        // Calculate screen ratio for scaling

        float backgroundImage_scale_ratio_height = camheight / bgImageHeight;
        float backgroundImage_scale_ratio_width = camWidth / bgImageWidth;

        backgroundImage.transform.localScale = new Vector3(backgroundImage_scale_ratio_width, backgroundImage_scale_ratio_height, 1);

        float dogImage_scale_ratio_height = camheight / dogImageHeight;
        float dogImage_scale_ratio_width = camWidth / dogImageWidth;

        dogImage.transform.localScale = new Vector3(dogImage_scale_ratio_width, dogImage_scale_ratio_height, 1);
    }
}
