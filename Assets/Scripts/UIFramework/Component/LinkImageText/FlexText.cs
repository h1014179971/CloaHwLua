using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(LinkImageText))]
	[DisallowMultipleComponent]
	public class FlexText : MonoBehaviour
	{
		const string upfoldSuffix = "...<a href=FlexText>[点击展开]</a>";
		const string foldSuffix = "<a href=FlexText>[点击缩回]</a>";

		public int summaryLine = 3;
		int charsOfSummaryLine;
		int summaryCharCount;

		LinkImageText _text;
		string contentStr;
		string summaryStr;
        string cacheStr;

		private void Awake()
		{
			_text = GetComponent<LinkImageText>();
            //UpdateParam();
		}

        private IEnumerator Start()
        {
            yield return null;
            yield return null;
            yield return null;
            if (cacheStr != null)
            {
                SetText(cacheStr);
                cacheStr = null;
            }
        }

        void UpdateParam()
        {
            int charsPerLine = Mathf.FloorToInt(_text.rectTransform.sizeDelta.x / _text.fontSize);
            charsOfSummaryLine = summaryLine * charsPerLine;
            summaryCharCount = charsPerLine * (summaryLine - 1) + charsPerLine / 3;

        }

		void OnEnable()
		{
			_text.onHrefClick.AddListener(OnHrefClick);
		}

		void OnDisable()
		{
			_text.onHrefClick.RemoveListener(OnHrefClick);
		}

		private void OnHrefClick(string hrefName)
		{
			Debug.Log("点击了 " + hrefName);
            if (contentStr.Length > _text.text.Length)
			{
				// 展开
				_text.text = contentStr + foldSuffix;
			}
			else
			{
				if (summaryStr.Length < contentStr.Length)
				{
					// 缩回
					_text.text = summaryStr + upfoldSuffix;
				}
			}
		}

		public void SetText(string content)
		{
            UpdateParam();
            cacheStr = contentStr = content;
			if (content.Length > charsOfSummaryLine)
			{
				summaryStr = contentStr.Substring(0, summaryCharCount);
				_text.text = summaryStr + upfoldSuffix;
			}
			else
			{
				summaryStr = contentStr;
				_text.text = contentStr;
			}
		}
	}
}
