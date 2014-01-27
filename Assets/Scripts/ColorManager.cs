using UnityEngine;
using System.Collections;
using System;

public enum ColorBlindMode
{
	None
};

//Created as a central place for converting the Colour enum into a Colour object
//Also handy for colour blind corrections
public class ColorManager
{
	public static ColorBlindMode currentColorBlindMode;

	public static int[] cachedColourValues = (int[])Enum.GetValues(typeof(Colour));

	//Return a bright pink if something went wrong.
	public static Color errorColor = new Color(1, 0.24f, 0.83f);

	public static Color GetObjectRealColor(Colour objColour)
	{
		switch(objColour)
		{
			case Colour.Red:
				return ConvertRed();
			case Colour.Green:
				return ConvertGreen();
			case Colour.Blue:
				return ConvertBlue();
			case Colour.None:
				return Color.white;
		}
		return errorColor;
	}

	static Color ConvertRed()
	{
		switch(currentColorBlindMode)
		{
			case ColorBlindMode.None:
				return new Color(0.7f, 0, 0);
		}
		return errorColor;
	}

	static Color ConvertGreen()
	{
		switch(currentColorBlindMode)
		{
			case ColorBlindMode.None:
				return new Color(0, 0.7f, 0);
		}
		return errorColor;
	}

	static Color ConvertBlue()
	{
		switch(currentColorBlindMode)
		{
			case ColorBlindMode.None:
				return new Color(0, 0, 0.7f);

		}
		return errorColor;
	}
}
