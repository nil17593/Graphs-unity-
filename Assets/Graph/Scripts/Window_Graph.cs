using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using TMPro;
using System;

public class Window_Graph : MonoBehaviour
{

    [SerializeField] private Sprite dotSprite;
    [SerializeField] private RectTransform graphContainer;
    private RectTransform lableTemplateX;
    private RectTransform lableTemplateY;
    [SerializeField] private RectTransform dashTemplateX;
    [SerializeField] private RectTransform dashTemplateY;
    private List<GameObject> gameobjectsList;

    //cached values
    private List<int> valueList;
    private IGraphVisual graphVisual;
    private int maxVisibleValueAmount;
    private Func<int, string> getAxisLableX;
    private Func<float, string> getAxisLableY;

    private void Awake()
    {
        lableTemplateX = graphContainer.Find("lableTemplateX").GetComponent<RectTransform>();
        lableTemplateY = graphContainer.Find("lableTemplateX").GetComponent<RectTransform>();
        gameobjectsList = new List<GameObject>();

        List<int> valueList = new List<int>() { 5, 98, 56, 45, 30, 22, 17, 15, 13, 17, 25, 37, 40, 36, 33 };

        IGraphVisual barChartVisual = new BarChartVisual(graphContainer, Color.green, .8f);
        IGraphVisual lineGraphVisual = new LineGraphVisual(graphContainer, dotSprite, Color.green, new Color(1, 1, 1, 0.5f));
        ShowGraph(valueList, barChartVisual, -1, (int _i) => "Day " + (_i + 1), (float _f) => "$" + Mathf.RoundToInt(_f));

        transform.Find("BarChartButton").GetComponent<Button_UI>().ClickFunc= () =>
        {
            SetGraphVisual(barChartVisual);
        };
        transform.Find("LineGraphButton").GetComponent<Button_UI>().ClickFunc = () =>
        {
            SetGraphVisual(lineGraphVisual);
        };

        transform.Find("DecreaseVisibleAmountButton").GetComponent<Button_UI>().ClickFunc = () =>
        {
            DecreaseVisibleAmount();
        };

        transform.Find("IncreaseVisibleAmountButton").GetComponent<Button_UI>().ClickFunc = () =>
        {
            IncreaseVisibleAmount();
        };

        transform.Find("DollerButton").GetComponent<Button_UI>().ClickFunc = () =>
        {
            SetGetAxisLableY((float _f) => "$" + Mathf.RoundToInt(_f));
        };
        transform.Find("EuroButton").GetComponent<Button_UI>().ClickFunc = () =>
        {
            SetGetAxisLableY((float _f) => "€" + Mathf.RoundToInt(_f/1.18f));
        };
        //bool useBarChart = true;
        //FunctionPeriodic.Create(() =>
        //{
        //    if (useBarChart)
        //    {
        //        ShowGraph(valueList, barGraphVisual, -1, (int _i) => "Day " + (_i + 1), (float _f) => "$" + Mathf.RoundToInt(_f));
        //    }
        //    else
        //    {
        //        ShowGraph(valueList, lineGraphVisual, -1, (int _i) => "Day " + (_i + 1), (float _f) => "$" + Mathf.RoundToInt(_f));
        //    }
        //    useBarChart = !useBarChart;
        //}, 0.5f);
    }

    private void SetGetAxisLableX(Func<int,string> getAxisLableX)
    {
        ShowGraph(this.valueList, this.graphVisual, this.maxVisibleValueAmount + 1, this.getAxisLableX, this.getAxisLableY);
    }

    private void SetGetAxisLableY(Func<float, string> getAxisLableY)
    {
        ShowGraph(this.valueList, this.graphVisual, this.maxVisibleValueAmount + 1, this.getAxisLableX, this.getAxisLableY);
    }

    private void IncreaseVisibleAmount()
    {
        ShowGraph(this.valueList, this.graphVisual, this.maxVisibleValueAmount + 1, this.getAxisLableX, this.getAxisLableY);
    }
    private void DecreaseVisibleAmount()
    {
        ShowGraph(this.valueList, this.graphVisual, this.maxVisibleValueAmount - 1, this.getAxisLableX, this.getAxisLableY);
    }

    private void SetGraphVisual(IGraphVisual graphVisual)
    {
        ShowGraph(this.valueList, graphVisual, this.maxVisibleValueAmount, this.getAxisLableX, this.getAxisLableY);
    }

    private void ShowGraph(List<int> valueList, IGraphVisual graphVisual, int maxVisibleValueAmount = -1, Func<int, string> getAxisLableX = null, Func<float, string> getAxisLableY = null)
    {
        this.valueList = valueList;
        this.graphVisual = graphVisual;
        this.maxVisibleValueAmount = maxVisibleValueAmount;
        this.getAxisLableX = getAxisLableX;
        this.getAxisLableY = getAxisLableY;

        if (maxVisibleValueAmount <= 0)
        {
            //show all if no amount is specified
            maxVisibleValueAmount = valueList.Count;
        }
        if (maxVisibleValueAmount > valueList.Count)
        {
            //validate the amount to show the maximum
            maxVisibleValueAmount = valueList.Count;
        }

        if (getAxisLableX == null)
        {
            getAxisLableX = delegate (int _i) { return _i.ToString(); };
        }
        if (getAxisLableY == null)
        {
            getAxisLableY = delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };
        }
       
        foreach (GameObject gameObject in gameobjectsList)
        {
            Destroy(gameObject);
        }
        gameobjectsList.Clear();

        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;


        float yMaximum = valueList[0];
        float yMinimum = valueList[0];

        //calculating the max value for y axis depending on the highest data
        for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++)
        {
            int value = valueList[i];
            if (value > yMaximum)
            {
                yMaximum = value;
            }
            if (value < yMinimum)
            {
                yMinimum = value;
            }
        }

        float yDifference = yMaximum - yMinimum;
        if (yDifference <= 0)
        {
            yDifference = 5f;//this is just a positive value because if there is zero it will add
        }
        yMaximum = yMaximum + (yDifference * 0.2f);
        yMinimum = yMinimum - (yDifference * 0.2f);

        float xSize = graphWidth / (maxVisibleValueAmount + 1);

        int xIndex = 0;
        for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++)
        {
            float xPosition = xSize + xIndex * xSize;
            float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;
            gameobjectsList.AddRange(graphVisual.AddGraphVisual(new Vector2(xPosition, yPosition), xSize));
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
            xIndex++;
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

    private interface IGraphVisual
    {
        List<GameObject> AddGraphVisual(Vector2 graphPosition, float graphPositionWidth);
    }

    private class BarChartVisual: IGraphVisual
    {
        private RectTransform graphContainer;
        private Color barColor;
        private float barWidthMultiplier;

        public BarChartVisual(RectTransform graphContainer, Color barColor, float barWidthMultiplier)
        {
            this.graphContainer = graphContainer;
            this.barColor = barColor;
            this.barWidthMultiplier = barWidthMultiplier;
        }

        public List<GameObject> AddGraphVisual(Vector2 graphPosition, float graphPositionWidth)
        {
            GameObject barGameObject = CreateBar(graphPosition, graphPositionWidth);
            return new List<GameObject> { barGameObject };
        }

        private GameObject CreateBar(Vector2 graphPosition, float barWidth)
        {
            GameObject gameObject = new GameObject("bar", typeof(Image));
            gameObject.transform.SetParent(graphContainer, false);
            gameObject.GetComponent<Image>().color = barColor;

            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(graphPosition.x, 0f);
            rectTransform.sizeDelta = new Vector2(barWidth * barWidthMultiplier, graphPosition.y);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(0.5f, 0f);
            return gameObject;

        }
    }

    private class LineGraphVisual: IGraphVisual
    {
        private RectTransform graphContainer;
        private Sprite dotSprite;
        private GameObject lastDotGameObject;
        private Color dotColor;
        private Color dotConnectionColor;

        public LineGraphVisual(RectTransform graphContainer, Sprite dotSprite,Color dotColor,Color dotConnectionColor)
        {
            this.graphContainer = graphContainer;
            this.dotSprite = dotSprite;
            lastDotGameObject = null;
            this.dotColor = dotColor;
            this.dotConnectionColor = dotConnectionColor;
        }

        public List<GameObject> AddGraphVisual(Vector2 graphPosition, float graphPositionWidth)
        {
            List<GameObject> gameobjectsList = new List<GameObject>();
            GameObject dotGameObject = CreateDot(graphPosition);
            gameobjectsList.Add(dotGameObject);
            if (lastDotGameObject != null)
            {
                GameObject connectionGameobject = CreateDotConnection(lastDotGameObject.GetComponent<RectTransform>().anchoredPosition, dotGameObject.GetComponent<RectTransform>().anchoredPosition);
                gameobjectsList.Add(connectionGameobject);
            }
            lastDotGameObject = dotGameObject;
            return gameobjectsList;
        }

        private GameObject CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
        {
            GameObject gameObject = new GameObject("dotConnection", typeof(Image));
            gameObject.transform.SetParent(graphContainer, false);
            gameObject.GetComponent<Image>().color = dotConnectionColor;
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

        private GameObject CreateDot(Vector2 anchoredPosition)
        {
            GameObject gameObject = new GameObject("dot", typeof(Image));
            gameObject.transform.SetParent(graphContainer, false);
            gameObject.GetComponent<Image>().sprite = dotSprite;
            gameObject.GetComponent<Image>().color = dotColor;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = new Vector2(11, 11);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            return gameObject;
        }
    }
}
