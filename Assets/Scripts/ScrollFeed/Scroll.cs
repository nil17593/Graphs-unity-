using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scroll : MonoBehaviour
{
    public GameObject feedItemPrefab;
    public int initialItemCount = 10;
    public float itemHeight = 100f;

    private ScrollRect scrollRect;
    private RectTransform content;
    private List<GameObject> feedItems = new List<GameObject>();
    private int currentIndex = 0;

    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        content = scrollRect.content;

        InitializeFeed();
    }

    private void InitializeFeed()
    {
        for (int i = 0; i < initialItemCount; i++)
        {
            GameObject feedItem = Instantiate(feedItemPrefab, content);
            feedItem.transform.localPosition = new Vector3(0, -i * itemHeight, 0);
            feedItems.Add(feedItem);
        }
    }

    private void Update()
    {
        if (scrollRect.normalizedPosition.y < 0.1f && currentIndex < feedItems.Count - 1)
        {
            LoadMoreItems();
        }
    }

    private void LoadMoreItems()
    {
        int itemsToLoad = 10; // Adjust the number of new items to load as needed

        for (int i = 0; i < itemsToLoad; i++)
        {
            currentIndex++;
            if (currentIndex < feedItems.Count)
            {
                feedItems[currentIndex].SetActive(true);
            }
            else
            {
                GameObject feedItem = Instantiate(feedItemPrefab, content);
                feedItem.transform.localPosition = new Vector3(0, -currentIndex * itemHeight, 0);
                feedItems.Add(feedItem);
            }
        }
    }
}
