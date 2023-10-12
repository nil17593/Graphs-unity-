using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using CodeMonkey.Utils;
using UnityEngine.UI;


namespace Garuna.Graphs
{
    public class WindowGraph : MonoBehaviour
    {
        private static WindowGraph Instance;
        [Header("Serialized Properties")]
        [SerializeField] private Sprite dotSprite;
        [SerializeField] private RectTransform graphContainer;
        [SerializeField] private RectTransform labelTemplateX;
        [SerializeField] private RectTransform labelTemplateY;
        [SerializeField] private RectTransform dashTemplateX;
        [SerializeField] private RectTransform dashTemplateY;
        [SerializeField] private GameObject toolTip;
        [SerializeField] private Button BarChartButton;
        [SerializeField] private Button LineGraphButton;
        [SerializeField] private Button DecreaseVisibleAmountButton;
        [SerializeField] private Button IncreaseVisibleAmountButton;
        [SerializeField] private Button DollerButton;
        [SerializeField] private Button EuroButton;

        private List<GameObject> gameObjectList;
        private List<IGraphVisualObject> graphVisualObjectList;
        private List<RectTransform> yLabelList;

        private IGraphVisual barChartVisual;
        private IGraphVisual lineGraphVisual;
        //cached values
        private List<int> valueList;
        private IGraphVisual graphVisual;
        private int maxVisibleValueAmount;
        private Func<int, string> getAxisLabelX;
        private Func<float, string> getAxisLabelY;
        private float xSize;
        private bool startYScaleAtZero;


        private void Awake()
        {
            Instance = this;
            gameObjectList = new List<GameObject>();
            graphVisualObjectList = new List<IGraphVisualObject>();
            yLabelList = new List<RectTransform>();
            List<int> valueList = new List<int>() { 5, 98, 56, 45, 30, 22, 17, 15, 13, 17, 25, 37, 40, 36, 33 };
            startYScaleAtZero = true;
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

        public void SetGetAxisLabelX(Func<int, string> getAxisLabelX)
        {
            ShowGraph(this.valueList, this.graphVisual, this.maxVisibleValueAmount, getAxisLabelX, this.getAxisLabelY);
        }

        public void SetGetAxisLabelY(Func<float, string> getAxisLabelY)
        {
            ShowGraph(this.valueList, this.graphVisual, this.maxVisibleValueAmount, this.getAxisLabelX, getAxisLabelY);
        }

        public void IncreaseVisibleAmount()
        {
            ShowGraph(this.valueList, this.graphVisual, this.maxVisibleValueAmount + 1, this.getAxisLabelX, this.getAxisLabelY);
        }

        public void DecreaseVisibleAmount()
        {
            ShowGraph(this.valueList, this.graphVisual, this.maxVisibleValueAmount - 1, this.getAxisLabelX, this.getAxisLabelY);
        }

        private void SetGraphVisual(IGraphVisual graphVisual)
        {
            ShowGraph(this.valueList, graphVisual, this.maxVisibleValueAmount, this.getAxisLabelX, this.getAxisLabelY);
        }

        public void ShowGraph(List<int> valueList, IGraphVisual graphVisual, int maxVisibleValueAmount = -1, Func<int, string> getAxisLabelX = null, Func<float, string> getAxisLabelY = null)
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
            yLabelList.Clear();

            foreach (IGraphVisualObject graphVisualObject in graphVisualObjectList)
            {
                graphVisualObject.CleanUp();
            }

            graphVisualObjectList.Clear();

            // Grab the width and height from the container
            float graphWidth = graphContainer.sizeDelta.x;
            float graphHeight = graphContainer.sizeDelta.y;

            float yMinimum, yMaximum;
            CalculateYScale(out yMinimum, out yMaximum);

            // Set the distance between each point on the graph 
            xSize = graphWidth / (maxVisibleValueAmount + 1);

            // Cycle through all visible data points
            int xIndex = 0;
            for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++)
            {
                float xPosition = xSize + xIndex * xSize;
                float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;

                // Add data point visual
                string tooltipText = getAxisLabelY(valueList[i]);
                graphVisualObjectList.Add(graphVisual.CreateGraphVisualObject(new Vector2(xPosition, yPosition), xSize, tooltipText));

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
                yLabelList.Add(labelY);
                gameObjectList.Add(labelY.gameObject);
                // Duplicate the dash template
                RectTransform dashY = Instantiate(dashTemplateY);
                dashY.SetParent(graphContainer, false);
                dashY.gameObject.SetActive(true);
                dashY.anchoredPosition = new Vector2(-4f, normalizedValue * graphHeight);
                gameObjectList.Add(dashY.gameObject);
            }
        }
        public void UpdateLastIndexValue(int value)
        {
            UpdateValue(valueList.Count - 1, value);
        }
        private void UpdateValue(int index, int value)
        {
            float yMinimumBefore, yMaximumBefore;

            CalculateYScale(out yMinimumBefore, out yMaximumBefore);
            valueList[index] = value;

            float graphWidth = graphContainer.sizeDelta.x;
            float graphHeight = graphContainer.sizeDelta.y;

            float yMinimum, yMaximum;
            CalculateYScale(out yMinimum, out yMaximum);

            bool yScaleChanged = yMinimumBefore != yMinimum || yMaximumBefore != yMinimum;

            if (!yScaleChanged)
            {
                //y scale did not changed 
                float xPosition = xSize + index * xSize;
                float yPosition = ((value - yMinimum) / (yMaximum - yMinimum)) * graphHeight;

                //Add data point visual
                string toolTipText = getAxisLabelY(value);
                graphVisualObjectList[index].SetGarphVisualObjectInfo(new Vector2(xPosition, yPosition), xSize, toolTipText);
            }
            else
            {
                //y scale chnaged so update whole graph and y labels
                //cycle through all visible data points
                int xIndex = 0;
                for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++)
                {
                    float xPosition = xSize + xIndex * xSize;
                    float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;

                    // Add data point visual
                    string tooltipText = getAxisLabelY(valueList[i]);
                    graphVisualObjectList[xIndex].SetGarphVisualObjectInfo(new Vector2(xPosition, yPosition), xSize, tooltipText);
                    xIndex++;
                }

                for (int i = 0; i < yLabelList.Count; i++)
                {
                    float normalizedValue = i * 1f / yLabelList.Count;
                    yLabelList[i].GetComponent<TextMeshProUGUI>().text = getAxisLabelY(yMinimum + (normalizedValue * (yMaximum - yMinimum)));
                }
            }
        }

        private void CalculateYScale(out float yMinimum, out float yMaximum)
        {

            // Identify y Min and Max values
            yMaximum = valueList[0];
            yMinimum = valueList[0];

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

            if (startYScaleAtZero)
            {
                yMinimum = 0f; // Start the graph at zero
            }
        }


        /*
         * Interface definition for showing visual for a data point
         * */
        public interface IGraphVisual
        {

            IGraphVisualObject CreateGraphVisualObject(Vector2 graphPosition, float graphPositionWidth, string tooltipText);
            void CleanUp();
        }

        //represents single visual graph object in graph
        public interface IGraphVisualObject
        {
            void SetGarphVisualObjectInfo(Vector2 graphPosition, float graphPositionWidth, string tooltipText);
            void CleanUp();
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

            public void CleanUp()
            {
                throw new NotImplementedException();
            }

            public IGraphVisualObject CreateGraphVisualObject(Vector2 graphPosition, float graphPositionWidth, string tooltipText)
            {
                GameObject barGameObject = CreateBar(graphPosition, graphPositionWidth);

                BarChartVisualObject barChartVisualObject = new BarChartVisualObject(barGameObject, barWidthMultiplier);
                barChartVisualObject.SetGarphVisualObjectInfo(graphPosition, graphPositionWidth, tooltipText);

                return barChartVisualObject;
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
                // Add Button_UI Component which captures UI Mouse Events
                Button_UI barButtonUI = gameObject.AddComponent<Button_UI>();
                return gameObject;
            }

            public class BarChartVisualObject : IGraphVisualObject
            {
                private GameObject barGameObject;
                private float barWidthMultiplier;
                public BarChartVisualObject(GameObject barGameObject, float barWidthMultiplier)
                {
                    this.barGameObject = barGameObject;
                    this.barWidthMultiplier = barWidthMultiplier;
                }

                public void CleanUp()
                {
                    Destroy(barGameObject);
                }

                public void SetGarphVisualObjectInfo(Vector2 graphPosition, float graphPositionWidth, string tooltipText)
                {
                    RectTransform rectTransform = barGameObject.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(graphPosition.x, 0f);
                    rectTransform.sizeDelta = new Vector2(graphPositionWidth * barWidthMultiplier, graphPosition.y);

                    Button_UI barButtonUI = barGameObject.GetComponent<Button_UI>();
                    // Show Tooltip on Mouse Over
                    barButtonUI.MouseOverOnceFunc = () =>
                    {
                        ShowTooltip_Static(tooltipText, graphPosition);
                    };

                    // Hide Tooltip on Mouse Out
                    barButtonUI.MouseOutOnceFunc = () =>
                    {
                        HideTooltip_Static();
                    };
                }

            }
        }


        /*
         * Displays data points as a Line Graph
         * */
        private class LineGraphVisual : IGraphVisual
        {

            private RectTransform graphContainer;
            private Sprite dotSprite;
            private LineGraphVisualObject lastLineGraphVisualObject;
            private Color dotColor;
            private Color dotConnectionColor;

            public LineGraphVisual(RectTransform graphContainer, Sprite dotSprite, Color dotColor, Color dotConnectionColor)
            {
                this.graphContainer = graphContainer;
                this.dotSprite = dotSprite;
                this.dotColor = dotColor;
                this.dotConnectionColor = dotConnectionColor;
                lastLineGraphVisualObject = null;
            }

            public void CleanUp()
            {
                throw new NotImplementedException();
            }

            public IGraphVisualObject CreateGraphVisualObject(Vector2 graphPosition, float graphPositionWidth, string tooltipText)
            {
                List<GameObject> gameObjectList = new List<GameObject>();
                GameObject dotGameObject = CreateDot(graphPosition);

                gameObjectList.Add(dotGameObject);

                GameObject dotConnectionGameObject = null;

                if (lastLineGraphVisualObject != null)
                {
                    dotConnectionGameObject = CreateDotConnection(lastLineGraphVisualObject.GetGraphPosition(), dotGameObject.GetComponent<RectTransform>().anchoredPosition);
                    gameObjectList.Add(dotConnectionGameObject);
                }


                LineGraphVisualObject lineGraphVisualObject = new LineGraphVisualObject(dotGameObject, dotConnectionGameObject, lastLineGraphVisualObject);
                lineGraphVisualObject.SetGarphVisualObjectInfo(graphPosition, graphPositionWidth, tooltipText);
                lastLineGraphVisualObject = lineGraphVisualObject;

                return lineGraphVisualObject;
                //return null;
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
                // Add Button_UI Component which captures UI Mouse Events
                Button_UI dotButtonUI = gameObject.AddComponent<Button_UI>();
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

            public class LineGraphVisualObject : IGraphVisualObject
            {
                public event EventHandler OnChangedGraphVisualObjectInfo;
                private GameObject dotGameObject;
                private GameObject dotConnectiongameObject;
                private LineGraphVisualObject lastVisualObject;
                public LineGraphVisualObject(GameObject dotGameObject, GameObject dotConnectiongameObject, LineGraphVisualObject lastVisualObject)
                {
                    this.dotGameObject = dotGameObject;
                    this.dotConnectiongameObject = dotConnectiongameObject;
                    this.lastVisualObject = lastVisualObject;

                    if (lastVisualObject != null)
                    {
                        lastVisualObject.OnChangedGraphVisualObjectInfo += LastVisualObject_OnChangedGraphVisualObjectInfo;
                    }
                }

                private void LastVisualObject_OnChangedGraphVisualObjectInfo(object sender, EventArgs e)
                {
                    UpdateDotConnection();
                }

                public void CleanUp()
                {
                    Destroy(dotGameObject);
                    Destroy(dotConnectiongameObject);
                }

                public void SetGarphVisualObjectInfo(Vector2 graphPosition, float graphPositionWidth, string tooltipText)
                {
                    RectTransform rectTransform = dotGameObject.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = graphPosition;

                    UpdateDotConnection();

                    Button_UI dotButtonUI = dotGameObject.GetComponent<Button_UI>();
                    // Show Tooltip on Mouse Over
                    dotButtonUI.MouseOverOnceFunc += () =>
                    {
                        ShowTooltip_Static(tooltipText, graphPosition);
                    };

                    // Hide Tooltip on Mouse Out
                    dotButtonUI.MouseOutOnceFunc += () =>
                    {
                        HideTooltip_Static();
                    };
                    if (OnChangedGraphVisualObjectInfo != null) OnChangedGraphVisualObjectInfo(this, EventArgs.Empty);
                }

                public Vector2 GetGraphPosition()
                {
                    RectTransform rectTransform = dotGameObject.GetComponent<RectTransform>();
                    return rectTransform.anchoredPosition;
                }

                private void UpdateDotConnection()
                {
                    if (dotConnectiongameObject != null)
                    {
                        RectTransform dotConnectionRectTransform = dotConnectiongameObject.GetComponent<RectTransform>();
                        Vector2 dir = (lastVisualObject.GetGraphPosition() - GetGraphPosition()).normalized;
                        float distance = Vector2.Distance(GetGraphPosition(), lastVisualObject.GetGraphPosition());
                        dotConnectionRectTransform.sizeDelta = new Vector2(distance, 3f);
                        dotConnectionRectTransform.anchoredPosition = GetGraphPosition() + dir * distance * .5f;
                        dotConnectionRectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir));
                    }
                }
            }
        }
    }
}
