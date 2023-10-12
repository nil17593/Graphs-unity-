using UnityEngine;

namespace Garuna.ScrollingFeed
{
    public class FeedManager : MonoBehaviour
    {
        [SerializeField] private FeedItemUI feedItemPrefab;
        [SerializeField] private RectTransform feedContainer;
        [SerializeField] private Sprite[] feedItems;
        [SerializeField] private GameObject commentsPanel;
        [SerializeField] private int numberOfImages = 10; // The initial number of images.
        [SerializeField] private int imagesToLoad = 5;    // The number of images to load when reaching the second last image.

        public static FeedManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            InitializeImages();
        }

        void InitializeImages()
        {
            for (int i = 0; i < numberOfImages; i++)
            {
                FeedItemUI newFeed = Instantiate(feedItemPrefab, feedContainer);
                int randomSpriteIndex = Random.Range(0, feedItems.Length);
                newFeed.Initialize(feedItems[randomSpriteIndex]);
            }
        }

        public void LoadMoreImages()
        {
            for (int i = 0; i < imagesToLoad; i++)
            {
                FeedItemUI newFeed = Instantiate(feedItemPrefab, feedContainer);
                int randomSpriteIndex = Random.Range(0, feedItems.Length);
                newFeed.Initialize(feedItems[randomSpriteIndex]);
            }
        }
    }
}
