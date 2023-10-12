using UnityEngine;

public class FeedManager : MonoBehaviour
{
    public FeedItemUI feedItemPrefab;
    public Transform feedContainer;
    public Sprite[] feedItems;

    private void Start()
    {
        // Assuming feedItems is an array of FeedItem Scriptable Objects
        for (int i =0;i<10;i++)
        {
            int randomSpriteIndex = Random.Range(0, feedItems.Length);
            FeedItemUI feedItemUI = Instantiate(feedItemPrefab, feedContainer);
            feedItemUI.Initialize(feedItems[randomSpriteIndex]);
        }
    }
}
