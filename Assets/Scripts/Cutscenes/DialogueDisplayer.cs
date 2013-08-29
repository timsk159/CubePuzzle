using UnityEngine;
using System.Collections;

public class DialogueDisplayer : MonoBehaviour 
{
	public int charsPerSecond;

	UILabel mLabel;
	string mText;
	int mOffset = 0;

	void Start()
	{
		mLabel = GetComponent<UILabel>();
		mLabel.supportEncoding = false;
		mLabel.symbolStyle = UIFont.SymbolStyle.None;
		mText = mLabel.font.WrapText(mLabel.text, mLabel.lineWidth / mLabel.cachedTransform.localScale.x, mLabel.maxLineCount, false, UIFont.SymbolStyle.None);
	}

	public IEnumerator DisplayText(string text)
	{
		while(mOffset < mText.Length)
		{
			// Periods and end-of-line characters should pause for a longer time.
			float delay = (float)charsPerSecond / 100f;

			char c = mText[mOffset];
			if(c == '.' || c == '\n' || c == '!' || c == '?')
			{
				delay *= 2f;
			}
			mLabel.text = mText.Substring(0, ++mOffset);
			yield return new WaitForSeconds(delay);
		}
	}

	public IEnumerator DisplayText(string[] textLines)
	{
				Debug.Log ("Asked to display: " + textLines.Length + " Lines of text");
		foreach(var text in textLines)
		{
			while(mOffset < text.Length)
			{
				charsPerSecond = Mathf.Max(1, charsPerSecond);

				// Periods and end-of-line characters should pause for a longer time.
				float delay = (float)charsPerSecond / 100f;

				char c = text[mOffset];
				if(c == '.' || c == '\n' || c == '!' || c == '?')
				{
					delay *= 2f;
				}

				mLabel.text = text.Substring(0, ++mOffset);
				yield return new WaitForSeconds(delay);
			}
			mLabel.text = "";
			mOffset = 0;
		}
	}
}
