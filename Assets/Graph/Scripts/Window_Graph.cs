using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using TMPro;
using System;

public class Window_Graph : MonoBehaviour
{

    [SerializeField] private Sprite circleSprite;
    [SerializeField] private RectTransform graphContainer;
    private RectTransform lableTemplateX;
    private RectTransform lableTemplateY;
    [SerializeField] private RectTransform dashTemplateX;
    [SerializeField] private RectTransform dashTemplateY;
    private List<GameObject> gameobjectsList;

    private void Awake()
    {
        lableTemplateX = graphContainer.Find("lableTemplateX").GetComponent<RectTransform>();
        lableTemplateY = graphContainer.Find("lableTemplateX").GetComponent<RectTransform>();
        gameobjectsList = new List<GameObject>();
        List<int> valueList = new List<int>() { 5, 98, 56, 45, 30, 22, 17, 15, 13, 17, 25, 37, 40, 36, 33 };
        ShowGraph(valueList, (int _i) => "Day " + (_i + 1), (float _f) => "$" + Mathf.RoundToInt(_f));
        FunctionPeriodic.Create(() =>
        {
            valueList.Clear();
            for (int i = 0; i < 15; i++)
            {
                valueList.Add(UnityEngine.Random.Range(0, 500));
            }
            ShowGraph(valueList, (int _i) => "Day " + (_i + 1), (float _f) => "$" + Mathf.RoundToInt(_f));
        }, 0.5f);
    }

    private GameObject CreateCircle(Vector2 anchoredPosition)
    {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return gameObject;
    }

    private void ShowGraph(List<int> valueList, Func<int, string> getAxisLableX = null, Func<float, string> getAxisLableY = null)
    {
        if (getAxisLableX == null)
        {
            getAxisLableX = delegate (int _i) { return _i.ToString(); };
        }
        if (getAxisLableY == null)
        {
            getAxisLableY = delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };
        }

        foreach(GameObject gameObject in gameobjectsList)
        {
            Destroy(gameObject);
        }
        gameobjectsList.Clear();

        float graphHeight = graphContainer.sizeDelta.y;
        float yMaximum = valueList[0];
        float yMinimum = valueList[0];

        //calculating the max value for y axis depending on the highest data
        foreach (int value in valueList)
        {
            if (value > yMaximum)
            {
                yMaximum = value;
            }
            if (value < yMinimum)
            {
                yMinimum = value;
            }
        }
        yMaximum = yMaximum + ((yMaximum - yMinimum) * 0.2f);
        yMinimum = yMinimum - ((yMaximum - yMinimum) * 0.2f);

        float xSize = 50f;

        GameObject lastCircleGameObject = null;
        for (int i = 0; i < valueList.Count; i++)
        {
            float xPosition = xSize + i * xSize;
            float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
            gameobjectsList.Add(circleGameObject);
            if (lastCircleGameObject != null)
            {
                GameObject connectionGameobject= CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
                gameobjectsList.Add(connectionGameobject);
            }
            lastCircleGameObject = circleGameObject;

            RectTransform lableX = Instantiate(lableTemplateX);
            lableX.SetParent(graphContainer);
            lableX.gameObject.SetActive(true);
            lableX.anchoredPosition = new Vector2(xPosition, -20f);
            lableX.GetComponent<TextMeshProUGUI>().text = getAxisLableX(i);
            gameobjectsList.Add(lableX.gameObject);

            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(graphContainer, false);
            dashX.gameObject.SetActive(true);
            dashX.anchoredPosition = new Vector2(xPosition, -20f);
            gameobjectsList.Add(dashX.gameObject);
        }

        int separatorCount = 10;

        for (int i = 0; i <= separatorCount; i++)
        {
            RectTransform lableY = Instantiate(lableTemplateY);
            lableY.SetParent(graphContainer, false);
            lableY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / separatorCount;// multiply by 1 to convert it into float value
            lableY.anchoredPosition = new Vector2(-20f, normalizedValue * graphHeight);// multiply by graphHeight to get the actual graph height if it is 1 then it will be at graph height
            lableY.GetComponent<TextMeshProUGUI>().text = getAxisLableY(yMinimum + (normalizedValue * (yMaximum - yMinimum)));// if this is 1 then it will be a graph max value
            gameobjectsList.Add(lableY.gameObject);

            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(graphContainer);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(-20f, normalizedValue * graphHeight);
            gameobjectsList.Add(dashY.gameObject);
        }
    }

    private GameObject CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, .5f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir));
        return gameObject;
    }
}
