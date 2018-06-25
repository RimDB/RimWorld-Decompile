﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Verse
{
	[HasDebugOutput]
	public static class GraphicDatabase
	{
		private static Dictionary<GraphicRequest, Graphic> allGraphics = new Dictionary<GraphicRequest, Graphic>();

		public static Graphic Get<T>(string path) where T : Graphic, new()
		{
			GraphicRequest req = new GraphicRequest(typeof(T), path, ShaderDatabase.Cutout, Vector2.one, Color.white, Color.white, null, 0, null);
			return GraphicDatabase.GetInner<T>(req);
		}

		public static Graphic Get<T>(string path, Shader shader) where T : Graphic, new()
		{
			GraphicRequest req = new GraphicRequest(typeof(T), path, shader, Vector2.one, Color.white, Color.white, null, 0, null);
			return GraphicDatabase.GetInner<T>(req);
		}

		public static Graphic Get<T>(string path, Shader shader, Vector2 drawSize, Color color) where T : Graphic, new()
		{
			GraphicRequest req = new GraphicRequest(typeof(T), path, shader, drawSize, color, Color.white, null, 0, null);
			return GraphicDatabase.GetInner<T>(req);
		}

		public static Graphic Get<T>(string path, Shader shader, Vector2 drawSize, Color color, int renderQueue) where T : Graphic, new()
		{
			GraphicRequest req = new GraphicRequest(typeof(T), path, shader, drawSize, color, Color.white, null, renderQueue, null);
			return GraphicDatabase.GetInner<T>(req);
		}

		public static Graphic Get<T>(string path, Shader shader, Vector2 drawSize, Color color, Color colorTwo) where T : Graphic, new()
		{
			GraphicRequest req = new GraphicRequest(typeof(T), path, shader, drawSize, color, colorTwo, null, 0, null);
			return GraphicDatabase.GetInner<T>(req);
		}

		public static Graphic Get<T>(string path, Shader shader, Vector2 drawSize, Color color, Color colorTwo, GraphicData data) where T : Graphic, new()
		{
			GraphicRequest req = new GraphicRequest(typeof(T), path, shader, drawSize, color, colorTwo, data, 0, null);
			return GraphicDatabase.GetInner<T>(req);
		}

		public static Graphic Get(Type graphicClass, string path, Shader shader, Vector2 drawSize, Color color, Color colorTwo)
		{
			return GraphicDatabase.Get(graphicClass, path, shader, drawSize, color, colorTwo, null, null);
		}

		public static Graphic Get(Type graphicClass, string path, Shader shader, Vector2 drawSize, Color color, Color colorTwo, GraphicData data, List<ShaderParameter> shaderParameters)
		{
			GraphicRequest graphicRequest = new GraphicRequest(graphicClass, path, shader, drawSize, color, colorTwo, data, 0, shaderParameters);
			Graphic result;
			if (graphicRequest.graphicClass == typeof(Graphic_Single))
			{
				result = GraphicDatabase.GetInner<Graphic_Single>(graphicRequest);
			}
			else if (graphicRequest.graphicClass == typeof(Graphic_Terrain))
			{
				result = GraphicDatabase.GetInner<Graphic_Terrain>(graphicRequest);
			}
			else if (graphicRequest.graphicClass == typeof(Graphic_Multi))
			{
				result = GraphicDatabase.GetInner<Graphic_Multi>(graphicRequest);
			}
			else if (graphicRequest.graphicClass == typeof(Graphic_Mote))
			{
				result = GraphicDatabase.GetInner<Graphic_Mote>(graphicRequest);
			}
			else if (graphicRequest.graphicClass == typeof(Graphic_Random))
			{
				result = GraphicDatabase.GetInner<Graphic_Random>(graphicRequest);
			}
			else if (graphicRequest.graphicClass == typeof(Graphic_Flicker))
			{
				result = GraphicDatabase.GetInner<Graphic_Flicker>(graphicRequest);
			}
			else if (graphicRequest.graphicClass == typeof(Graphic_Appearances))
			{
				result = GraphicDatabase.GetInner<Graphic_Appearances>(graphicRequest);
			}
			else if (graphicRequest.graphicClass == typeof(Graphic_StackCount))
			{
				result = GraphicDatabase.GetInner<Graphic_StackCount>(graphicRequest);
			}
			else
			{
				try
				{
					return (Graphic)GenGeneric.InvokeStaticGenericMethod(typeof(GraphicDatabase), graphicRequest.graphicClass, "GetInner", new object[]
					{
						graphicRequest
					});
				}
				catch (Exception ex)
				{
					Log.Error(string.Concat(new object[]
					{
						"Exception getting ",
						graphicClass,
						" at ",
						path,
						": ",
						ex.ToString()
					}), false);
				}
				result = BaseContent.BadGraphic;
			}
			return result;
		}

		private static T GetInner<T>(GraphicRequest req) where T : Graphic, new()
		{
			Graphic graphic;
			if (!GraphicDatabase.allGraphics.TryGetValue(req, out graphic))
			{
				graphic = Activator.CreateInstance<T>();
				graphic.Init(req);
				GraphicDatabase.allGraphics.Add(req, graphic);
			}
			return (T)((object)graphic);
		}

		public static void Clear()
		{
			GraphicDatabase.allGraphics.Clear();
		}

		[Category("System")]
		[DebugOutput]
		public static void AllGraphicsLoaded()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("There are " + GraphicDatabase.allGraphics.Count + " graphics loaded.");
			int num = 0;
			foreach (Graphic graphic in GraphicDatabase.allGraphics.Values)
			{
				stringBuilder.AppendLine(num + " - " + graphic.ToString());
				if (num % 50 == 49)
				{
					Log.Message(stringBuilder.ToString(), false);
					stringBuilder = new StringBuilder();
				}
				num++;
			}
			Log.Message(stringBuilder.ToString(), false);
		}

		// Note: this type is marked as 'beforefieldinit'.
		static GraphicDatabase()
		{
		}
	}
}
