﻿using System;
using System.Xml;
using Verse;

namespace RimWorld
{
	public class WeatherCommonalityRecord
	{
		public WeatherDef weather;

		public float commonality;

		public WeatherCommonalityRecord()
		{
		}

		public void LoadDataFromXmlCustom(XmlNode xmlRoot)
		{
			DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "weather", xmlRoot.Name);
			this.commonality = (float)ParseHelper.FromString(xmlRoot.FirstChild.Value, typeof(float));
		}
	}
}
