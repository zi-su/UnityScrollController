﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using DG.Tweening;
namespace UnityScrollController
{
	public class UnityScrollController : MonoBehaviour{
        [SerializeField] ScrollRect scrollRect;
        [SerializeField] VerticalLayoutGroup verticalLayoutGroup;
        [SerializeField] HorizontalLayoutGroup horizontalLayoutGroup;

        RectTransform content;
        RectTransform scrollTrans;
        int index = 0;

        float time = 0.2f;
        bool wait = false;
        
        enum State
        {
            WAIT,
            UPDATE,
            ANIMATE,
        }
        State state = State.WAIT;
        private void Start()
        {
            content = scrollRect.content as RectTransform;
            scrollTrans = scrollRect.transform as RectTransform;
            
        }
        private void Update()
        {
            switch (state)
            {
                case State.WAIT:
                    state = State.UPDATE;
                    break;
                case State.UPDATE:
                    if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        MoveDown();
                    }
                    else if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        MoveUp();
                    }
                    if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        MoveLeft();
                    }
                    if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        MoveRight();
                    }
                    if (Input.GetKeyDown(KeyCode.H))
                    {
                        MoveIndex(4);
                    }
                    if (Input.GetKeyDown(KeyCode.G))
                    {
                        MoveIndex(0);
                    }
                    if (Input.GetKeyDown(KeyCode.J))
                    {
                        MoveIndex(content.childCount - 1);
                    }

                    if (!wait)
                    {
                        if (verticalLayoutGroup != null)
                        {
                            if (GetTop(index) > 0.0f)
                            {
                                content.GetChild(index).GetComponent<IScrollEvent>()?.OnScrollOut();
                                index++;
                                if (index > content.childCount - 1) index = content.childCount - 1;
                                content.GetChild(index).GetComponent<IScrollEvent>()?.OnScrollIn();
                            }
                            else if (Mathf.Abs(GetBottom(index)) > scrollTrans.rect.height)
                            {
                                content.GetChild(index).GetComponent<IScrollEvent>()?.OnScrollOut();
                                index--;
                                if (index < 0) index = 0;
                                content.GetChild(index).GetComponent<IScrollEvent>()?.OnScrollIn();
                            }
                        }
                        else if (horizontalLayoutGroup!= null)
                        {
                            if (GetLeft(index) < 0.0f)
                            {
                                content.GetChild(index).GetComponent<IScrollEvent>()?.OnScrollOut();
                                index++;
                                if (index > content.childCount - 1) index = content.childCount - 1;
                                content.GetChild(index).GetComponent<IScrollEvent>()?.OnScrollIn();
                            }
                            else if (GetRight(index) > scrollTrans.rect.width)
                            {
                                content.GetChild(index).GetComponent<IScrollEvent>()?.OnScrollOut();
                                index--;
                                if (index < 0) index = 0;
                                content.GetChild(index).GetComponent<IScrollEvent>()?.OnScrollIn();
                            }
                        }
                    }
                    break;
                case State.ANIMATE:
                    break;
            }
            Debug.Log(index);
        }
        
        bool OutAreaVertical(int index)
        {
            var child = content.GetChild(index) as RectTransform;
            float top = GetTop(index);
            float bottom = GetBottom(index);
            if(top > 0 || Mathf.Abs(bottom) > scrollTrans.rect.height)
            {
                return true;
            }
            return false;
        }

        bool OutAreaHorizontal(int index)
        {
            float left = GetLeft(index);
            float right = GetRight(index);
            if (left < 0 || right > scrollTrans.rect.width)
            {
                return true;
            }
            return false;
        }

        float GetTop(int index)
        {
            var child = content.GetChild(index) as RectTransform;
            float top = child.localPosition.y + child.rect.height * child.pivot.y + content.localPosition.y;
            return top;
        }

        float GetBottom(int index)
        {
            var child = content.GetChild(index) as RectTransform;
            float bottom = child.localPosition.y - child.rect.height * child.pivot.y + content.localPosition.y;
            return bottom;
        }

        float GetLeft(int index)
        {
            var child = content.GetChild(index) as RectTransform;
            float left = child.localPosition.x - child.rect.width * child.pivot.x + content.localPosition.x;
            return left;
        }

        float GetRight(int index)
        {
            var child = content.GetChild(index) as RectTransform;
            float right = child.localPosition.x + child.rect.width * child.pivot.x + content.localPosition.x;
            return right;
        }

        public void MoveIndex(int index)
        {
            if(horizontalLayoutGroup != null)
            {
                var now = content.GetChild(this.index) as RectTransform;
                var next = content.GetChild(index) as RectTransform;
                now.GetComponent<IScrollEvent>()?.OnScrollOut();
                next.GetComponent<IScrollEvent>()?.OnScrollIn();
                if (OutAreaHorizontal(index))
                {
                    if (index < this.index)
                    {
                        MoveIndexLeft(index);
                    }
                    else
                    {
                        MoveIndexRight(index);
                    }
                }
            }
            else if(verticalLayoutGroup != null)
            {
                var now = content.GetChild(this.index) as RectTransform;
                var next = content.GetChild(index) as RectTransform;
                now.GetComponent<IScrollEvent>()?.OnScrollOut();
                next.GetComponent<IScrollEvent>()?.OnScrollIn();
                if (OutAreaVertical(index))
                {
                    if (index < this.index)
                    {
                        //上端に映るように移動
                        MoveIndexUp(index);
                    }
                    else
                    {
                        //下端に映るように移動
                        MoveIndexDown(index);
                    }
                }
            }
        }

        public void MoveDown()
        {
            var now = content.GetChild(index) as RectTransform;
            {
                now.GetComponent<IScrollEvent>()?.OnScrollOut();
                index++;
                if (index > content.childCount - 1)
                {
                    index = 0;
                }
                content.GetChild(index).GetComponent<IScrollEvent>()?.OnScrollIn();
            }
            if (OutAreaVertical(index))
            {
                //現在位置、次の位置の差分だけ移動
                DiffMoveVertical(now, content.GetChild(index) as RectTransform);
            }
        }

        public void MoveUp()
        {
            var now = content.GetChild(index) as RectTransform;
            {
                now.GetComponent<IScrollEvent>()?.OnScrollOut();
                index--;
                if (index < 0)
                {
                    index = content.childCount - 1;
                }
                content.GetChild(index).GetComponent<IScrollEvent>()?.OnScrollIn();
            }
            if (OutAreaVertical(index))
            {
                //現在位置、次の位置の差分だけ移動
                DiffMoveVertical(now, content.GetChild(index) as RectTransform);
            }
        }

        public void MoveLeft()
        {
            var now = content.GetChild(index) as RectTransform;
            {
                now.GetComponent<IScrollEvent>()?.OnScrollOut();
                index--;
                if (index < 0)
                {
                    index = content.childCount - 1;
                }
                content.GetChild(index).GetComponent<IScrollEvent>()?.OnScrollIn();
            }
            if (OutAreaHorizontal(index))
            {
                //現在位置、次の位置の差分だけ移動
                DiffMoveHorizontal(now, content.GetChild(index) as RectTransform);
            }
        }

        public void MoveRight()
        {
            var now = content.GetChild(index) as RectTransform;
            {
                now.GetComponent<IScrollEvent>()?.OnScrollOut();
                index++;
                if (index > content.childCount - 1)
                {
                    index = 0;
                }
                content.GetChild(index).GetComponent<IScrollEvent>()?.OnScrollIn();
            }
            if (OutAreaHorizontal(index))
            {
                //現在位置、次の位置の差分だけ移動
                DiffMoveHorizontal(now, content.GetChild(index) as RectTransform);
            }
        }


        void MoveIndexLeft(int index)
        {
            //左端に映るように移動
            float left = GetLeft(index) - horizontalLayoutGroup.spacing;
            float n = left / (content.rect.width - scrollTrans.rect.width);
            n = scrollRect.horizontalNormalizedPosition + n;
            n = Mathf.Clamp01(n);
            TweenHorizontal(n);
            this.index = index;
        }

        void MoveIndexRight(int index)
        {
            //右端に映るように移動
            float right = GetRight(index) + horizontalLayoutGroup.spacing;
            right = right - scrollTrans.rect.width;
            float n = right / (content.rect.width - scrollTrans.rect.width);
            n = scrollRect.horizontalNormalizedPosition + (n);
            n = Mathf.Clamp01(n);
            TweenHorizontal(n);
            this.index = index;
        }

        void MoveIndexDown(int index)
        {
            //下端に映るように移動
            float down = GetBottom(index) - verticalLayoutGroup.spacing;
            down = down + scrollTrans.rect.height;
            float n = down / (content.rect.height - scrollTrans.rect.height);
            n = scrollRect.verticalNormalizedPosition + n;
            n = Mathf.Clamp01(n);
            TweenVertical(n);
            this.index = index;
        }

        void MoveIndexUp(int index)
        {
            //上端に映るように移動
            float top = GetTop(index) + verticalLayoutGroup.spacing;
            float n = top / (content.rect.height - scrollTrans.rect.height);
            n = scrollRect.verticalNormalizedPosition + n;
            n = Mathf.Clamp01(n);
            TweenVertical(n);
            this.index = index;
        }

        void DiffMoveVertical(Transform now, Transform next)
        {
            //現在位置、次の位置の差分だけ移動
            float diff = next.localPosition.y - now.localPosition.y;
            float ndiff = diff / (content.rect.height - scrollTrans.rect.height);
            float n = scrollRect.verticalNormalizedPosition + ndiff;
            if (index == 0) n = 1.0f;
            n = Mathf.Clamp01(n);
            TweenVertical(n);
        }

        void DiffMoveHorizontal(Transform now, Transform next)
        {
            //現在位置、次の位置の差分だけ移動
            float diff = next.localPosition.x - now.localPosition.x;
            float ndiff = diff / (content.rect.width - scrollTrans.rect.width);
            float n = scrollRect.horizontalNormalizedPosition + ndiff;
            n = Mathf.Clamp01(n);
            if (index == 0) n = 0.0f;
            TweenHorizontal(n);
        }

        void TweenHorizontal(float n)
        {
            wait = true;
            var t = DOTween.To(() => scrollRect.horizontalNormalizedPosition, x => scrollRect.horizontalNormalizedPosition = x, n, time);
            t.onComplete += () => wait = false;
        }

        void TweenVertical(float n)
        {
            wait = true;
            var t = DOTween.To(() => scrollRect.verticalNormalizedPosition, x => scrollRect.verticalNormalizedPosition = x, n, time);
            t.onComplete += () => wait = false;
        }
	}
}