using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using CodeMonkey.Utils;
using UnityEngine.UI;

public class WindowGraph : MonoBehaviour
{
    private static WindowGraph Instance;
    [SerializeField] private Sprite dotSprite;
    [SerializeField] private RectTransform graphContainer;
    [SerializeField] private RectTransform labelTemplateX;
    [SerializeField] private RectTransform labelTemplateY;
    [SerializeField] private RectTransform dashTemplateX;
    [SerializeField] private RectTransform dashTemplateY;
    [SerializeField] private GameObject toolTip;
    private List<GameObject> gameObjectList;

    [SerializeField] private Button BarChartButton;
    [SerializeField] private Button LineGraphButton;
    [SerializeField] private Button DecreaseVisibleAmountButton;
    [SerializeField] private Button IncreaseVisibleAmountButton;
    [SerializeField] private Button DollerButton;
    [SerializeField] private Button EuroButton;
    private IGraphVisual barChartVisual;
    private IGraphVisual lineGraphVisual;

    //cached values
    private List<int> valueList;
    private IGraphVisual graphVisual;
    private int maxVisibleValueAmount;
    private Func<int, string> getAxisLabelX;
    private Func<float, string> getAxisLabelY;
    private void Awake()
    {
        Instance = this;
        gameObjectList = new List<GameObject>();

        List<int> valueList = new List<int>() { 5, 98, 56, 45, 30, 22, 17, 15, 13, 17, 25, 37, 40, 36, 33 };

        barChartVisual = new BarChartVisual(graphContainer, Color.green, .8f);
        lineGraphVisual = new LineGraphVisual(graphContainer, dotSprite, Color.green, new Color(1, 1, 1, 0.5f));
        ShowGraph(valueList, barChartVisual, -1, (int _i) => "Day " + (_i + 1), (float _f) => "$" + Mathf.RoundToInt(_f));

        BarChartButton.onClick.AddListener(OnBarChartButtonClick);
        LineGraphButton.onClick.AddListener(OnLineGraphButtonClick);
        DecreaseVisibleAmountButton.onClick.AddListener(DecreaseVisibleAmount);
        IncreaseVisibleAmountButton.onClick.AddListener(IncreaseVisibleAmount);
        DecreaseVisibleAmountButton.onClick.AddListener(DecreaseVisibleAmount);
        DollerButton.onClick.AddListener(OnDollerButtonClick);
        EuroButton.onClick.AddListener(OnEuroButtonClick);

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
        //ShowToolTip("THIIS", new Vector2(500, 500));

        //FunctionPeriodic.Create(() => { 
        //    ShowToolTip("THIIS"+UnityEngine.Random.Range(100000,float.MaxValue), new Vector2(500, 500));
        //},.1f);
    }

    public static void ShowTooltip_Static(string tooltipText, Vector2 anchoredPosition)
    {
        Instance.ShowTooltip(tooltipText, anchoredPosition);
    }

    private void ShowTooltip(string tooltipText, Vector2 anchoredPosition)
    {
        // Show Tooltip GameObject
        toolTip.SetActive(true);

        toolTip.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;

        TextMeshProUGUI tooltipUIText = toolTip.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        tooltipUIText.text = tooltipText;

        float textPaddingSize = 4f;
        Vector2 backgroundSize = new Vector2(
            tooltipUIText.preferredWidth + textPaddingSize * 2f,
            tooltipUIText.preferredHeight + textPaddingSize * 2f
        );

        toolTip.transform.Find("BackGround").GetComponent<RectTransform>().sizeDelta = backgroundSize;

        // UI Visibility Sorting based on Hierarchy, SetAsLastSibling in order to show up on top
        toolTip.transform.SetAsLastSibling();
    }

    public static void HideTooltip_Static()
    {
        Instance.HideTooltip();
    }

    private void HideTooltip()
    {
        toolTip.SetActive(false);
    }

    #region Button Clicks

    private void OnBarChartButtonClick()
    {
        SetGraphVisual(barChartVisual);
    }

    private void OnLineGraphButtonClick()
    {
        SetGraphVisual(lineGraphVisual);
    }

    private void OnDollerButtonClick()
    {
        SetGetAxisLabelY((float _f) => "$" + Mathf.RoundToInt(_f));
    }
    private void OnEuroButtonClick()
    {
        SetGetAxisLabelY((float _f) => "e" + Mathf.RoundToInt(_f / 1.18f));
    }
    #endregion

    private void SetGetAxisLabelX(Func<int, string> getAxisLabelX)
    {
        ShowGraph(this.valueList, this.graphVisual, this.maxVisibleValueAmount, getAxisLabelX, this.getAxisLabelY);
    }

    private void SetGetAxisLabelY(Func<float, string> getAxisLabelY)
    {
        ShowGraph(this.valueList, this.graphVisual, this.maxVisibleValueAmount, this.getAxisLabelX, getAxisLabelY);
    }

    private void IncreaseVisibleAmount()
    {
        ShowGraph(this.valueList, this.graphVisual, this.maxVisibleValueAmount + 1, this.getAxisLabelX, this.getAxisLabelY);
    }

    private void DecreaseVisibleAmount()
    {
        ShowGraph(this.valueList, this.graphVisual, this.maxVisibleValueAmount - 1, this.getAxisLabelX, this.getAxisLabelY);
    }

    private void SetGraphVisual(IGraphVisual graphVisual)
    {
        ShowGraph(this.valueList, graphVisual, this.maxVisibleValueAmount, this.getAxisLabelX, this.getAxisLabelY);
    }

    private void ShowGraph(List<int> valueList, IGraphVisual graphVisual, int maxVisibleValueAmount = -1, Func<int, string> getAxisLabelX = null, Func<float, string> getAxisLabelY = null)
    {
        this.valueList = valueList;
        this.graphVisual = graphVisual;
        this.getAxisLabelX = getAxisLabelX;
        this.getAxisLabelY = getAxisLabelY;

        if (maxVisibleValueAmount <= 0)
        {
            // Show all if no amount specified
            maxVisibleValueAmount = valueList.Count;
        }
        if (maxVisibleValueAmount > valueList.Count)
        {
            // Validate the amount to show the maximum
            maxVisibleValueAmount = valueList.Count;
        }

        this.maxVisibleValueAmount = maxVisibleValueAmount;

        // Test for label defaults
        if (getAxisLabelX == null)
        {
            getAxisLabelX = delegate (int _i) { return _i.ToString(); };
        }
        if (getAxisLabelY == null)
        {
            getAxisLabelY = delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };
        }

        // Clean up previous graph
        foreach (GameObject gameObject in gameObjectList)
        {
            Destroy(gameObject);
        }
        gameObjectList.Clear();

        // Grab the width and height from the container
        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;

        // Identify y Min and Max values
        float yMaximum = valueList[0];
        float yMinimum = valueList[0];

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
            yDifference = 5f;
        }
        yMaximum = yMaximum + (yDifference * 0.2f);
        yMinimum = yMinimum - (yDifference * 0.2f);

        yMinimum = 0f; // Start the graph at zero

        // Set the distance between each point on the graph 
        float xSize = graphWidth / (maxVisibleValueAmount + 1);

        // Cycle through all visible data points
        int xIndex = 0;
        for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++)
        {
            float xPosition = xSize + xIndex * xSize;
            float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;

            // Add data point visual
            string tooltipText = getAxisLabelY(valueList[i]);
            gameObjectList.AddRange(graphVisual.AddGraphVisual(new Vector2(xPosition, yPosition), xSize, tooltipText));

            // Duplicate the x label template
            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer, false);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition, -25f);
            labelX.GetComponent<TextMeshProUGUI>().text = getAxisLabelX(i);
            gameObjectList.Add(labelX.gameObject);

            // Duplicate the x dash template
            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(graphContainer, false);
            dashX.gameObject.SetActive(true);
            dashX.anchoredPosition = new Vector2(xPosition, -3f);
            gameObjectList.Add(dashX.gameObject);

            xIndex++;
        }

        // Set up separators on the y axis
        int separatorCount = 10;
        for (int i = 0; i <= separatorCount; i++)
        {
            // Duplicate the label template
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer, false);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / separatorCount;
            labelY.anchoredPosition = new Vector2(-25f, normalizedValue * graphHeight);
            labelY.GetComponent<TextMeshProUGUI>().text = getAxisLabelY(yMinimum + (normalizedValue * (yMaximum - yMinimum)));
            gameObjectList.Add(labelY.gameObject);

            // Duplicate the dash template
            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(graphContainer, false);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(-4f, normalizedValue * graphHeight);
            gameObjectList.Add(dashY.gameObject);
        }
    }



    /*
     * Interface definition for showing visual for a data point
     * */
    private interface IGraphVisual
    {

        List<GameObject> AddGraphVisual(Vector2 graphPosition, float graphPositionWidth, string tooltipText);

    }


    /*
     * Displays data points as a Bar Chart
     * */
    private class BarChartVisual : IGraphVisual
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

        public List<GameObject> AddGraphVisual(Vector2 graphPosition, float graphPositionWidth, string tooltipText)
        {
            GameObject barGameObject = CreateBar(graphPosition, graphPositionWidth);

            // Add Button_UI Component which captures UI Mouse Events
            Button_UI barButtonUI = barGameObject.AddComponent<Button_UI>();

            // Show Tooltip on Mouse Over
            barButtonUI.MouseOverOnceFunc += () => {
                ShowTooltip_Static(tooltipText, graphPosition);
            };

            // Hide Tooltip on Mouse Out
            barButtonUI.MouseOutOnceFunc += () => {
                HideTooltip_Static();
            };
            return new List<GameObject>() { barGameObject };
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
            rectTransform.pivot = new Vector2(.5f, 0f);
            return gameObject;
        }
    }


    /*
     * Displays data points as a Line Graph
     * */
    private class LineGraphVisual : IGraphVisual
    {

        private RectTransform graphContainer;
        private Sprite dotSprite;
        private GameObject lastDotGameObject;
        private Color dotColor;
        private Color dotConnectionColor;

        public LineGraphVisual(RectTransform graphContainer, Sprite dotSprite, Color dotColor, Color dotConnectionColor)
        {
            this.graphContainer = graphContainer;
            this.dotSprite = dotSprite;
            this.dotColor = dotColor;
            this.dotConnectionColor = dotConnectionColor;
            lastDotGameObject = null;
        }


        public List<GameObject> AddGraphVisual(Vector2 graphPosition, float graphPositionWidth, string tooltipText)
        {
            List<GameObject> gameObjectList = new List<GameObject>();
            GameObject dotGameObject = CreateDot(graphPosition);

            // Add Button_UI Component which captures UI Mouse Events
            Button_UI dotButtonUI = dotGameObject.AddComponent<Button_UI>();

            // Show Tooltip on Mouse Over
            dotButtonUI.MouseOverOnceFunc += () => {
                ShowTooltip_Static(tooltipText, graphPosition);
            };

            // Hide Tooltip on Mouse Out
            dotButtonUI.MouseOutOnceFunc += () => {
                HideTooltip_Static();
            };

            gameObjectList.Add(dotGameObject);
            if (lastDotGameObject != null)
            {
                GameObject dotConnectionGameObject = CreateDotConnection(lastDotGameObject.GetComponent<RectTransform>().anchoredPosition, dotGameObject.GetComponent<RectTransform>().anchoredPosition);
                gameObjectList.Add(dotConnectionGameObject);
            }
            lastDotGameObject = dotGameObject;
            return gameObjectList;
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

        private GameObject CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
        {
            GameObject gameObject = new GameObject("dotConnection", typeof(Image));
            gameObject.transform.SetParent(graphContainer, false);
            gameObject.GetComponent<Image>().color = dotConnectionColor;
            gameObject.GetComponent<Image>().raycastTarget = false;
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
}
