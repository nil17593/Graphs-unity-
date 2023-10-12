using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Garuna.ScrollingFeed
{
    public class CommentsPanel : MonoBehaviour
    {
        private Vector3 touchStartPos;
        private Vector3 touchEndPos;

        [SerializeField] private FeedItemUI feedItemUI;
        [SerializeField] private Button commentButton;
        [SerializeField] private GameObject commentPanelPrefab;
        [SerializeField] private RectTransform commentHolder;
        //private void Update()
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        touchStartPos = Input.mousePosition;
        //    }
        //    if (Input.GetMouseButtonUp(0))
        //    {
        //        touchEndPos = Input.mousePosition;
        //        if (touchEndPos.y < touchStartPos.y)
        //        {
        //            transform.GetComponent<RectTransform>().position = new Vector2(0, -1000f);
        //        }
        //    }
        //}

        public void OnPostCommentButtonClick()
        {
            PostComment();
        }

        void PostComment()
        {
            int commentsCount = feedItemUI.GetComments();
            for (int i = 0; i < commentsCount; i++)
            {
                GameObject commentpanel = Instantiate(commentPanelPrefab, commentHolder);
            }
        }
    }
}