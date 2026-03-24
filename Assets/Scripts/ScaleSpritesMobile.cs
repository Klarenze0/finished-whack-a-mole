using System.Collections.Generic;
using UnityEngine;

public class SpriteVerticalSpacing : MonoBehaviour
{
    [SerializeField] private List<GameObject> fleasObjs = new List<GameObject>();

    private void Start()
    {
        // Calculate the aspect ratio as a float
        float screenWidth = (float)Screen.width;
        float screenHeight = (float)Screen.height;
        float aspectRatio = screenWidth / screenHeight;

        // Log the result
        Debug.Log("Aspect Ratio (float): " + aspectRatio);

        if (aspectRatio < 0.5)
        {
            for (int i = 0; i < fleasObjs.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        fleasObjs[i].transform.localPosition = new Vector3(-3.53f, -0.1799998f, 0f);
                        fleasObjs[i].transform.localScale = new Vector3(0.7f, 0.7f, 1);
                        break;
                    case 1:
                        fleasObjs[i].transform.localPosition = new Vector3(-2.06f, -0.2000003f, 0);
                        fleasObjs[i].transform.localScale = new Vector3(0.7f, 0.7f, 1);
                        break;
                    case 2:
                        fleasObjs[i].transform.localPosition = new Vector3(-0.67f, -0.13f, 0);
                        fleasObjs[i].transform.localScale = new Vector3(0.7f, 0.7f, 1);
                        break;
                    case 3:
                        fleasObjs[i].transform.localPosition = new Vector3(-3.53f, -1.94f, 0);
                        fleasObjs[i].transform.localScale = new Vector3(0.7f, 0.7f, 1);
                        break;
                    case 4:
                        fleasObjs[i].transform.localPosition = new Vector3(-2.06f, -1.960001f, 0);
                        fleasObjs[i].transform.localScale = new Vector3(0.7f, 0.7f, 1);
                        break;
                    case 5:
                        fleasObjs[i].transform.localPosition = new Vector3(-0.6699998f, -1.89f, 0);
                        fleasObjs[i].transform.localScale = new Vector3(0.7f, 0.7f, 1);
                        break;
                    case 6:
                        fleasObjs[i].transform.localPosition = new Vector3(-3.53f, -3.87f, 0);
                        fleasObjs[i].transform.localScale = new Vector3(0.7f, 0.7f, 1);
                        break;
                    case 7:
                        fleasObjs[i].transform.localPosition = new Vector3(-2.06f, -3.89f, 0);
                        fleasObjs[i].transform.localScale = new Vector3(0.7f, 0.7f, 1);
                        break;
                    case 8:
                        fleasObjs[i].transform.localPosition = new Vector3(-0.6699998f, -3.82f, 0);
                        fleasObjs[i].transform.localScale = new Vector3(0.7f, 0.7f, 1);
                        break;
                }
            }
        }
    }
}
