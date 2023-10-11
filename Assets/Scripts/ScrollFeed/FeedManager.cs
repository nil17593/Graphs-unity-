using UnityEngine;

public class FeedManager : MonoBehaviour
{
    public GameObject feedItemPrefab;
    public Transform feedContainer;
    public FeedItem[] feedItems;

    private void Start()
    {
        // Assuming feedItems is an array of FeedItem Scriptable Objects
        foreach (var feedItem in feedItems)
        {
            var feedItemUI = Instantiate(feedItemPrefab, feedContainer).GetComponent<FeedItemUI>();
            feedItemUI.Initialize(feedItem);
        }
    }
}
