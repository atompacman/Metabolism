// MIT License
// 
// Copyright (c) 2017 FXGuild
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;

using JetBrains.Annotations;

using UnityEngine;

using Random = UnityEngine.Random;

public class Test : MonoBehaviour
{
   #region Nested types

   private class LineCreator
   {
      #region Nested types

      [Serializable]
      public struct ParamChangeEvent
      {
         public float RelPos;

         public ParamType ParamType;

         public Vector3 NewVal;
      }

      #endregion

      #region Compile-time constants

      private const uint MAX_NUM_SEGS = 1000;
      private const uint MAX_NUM_LEVELS = 4;

      #endregion

      #region Static fields

      public static uint TotalNumSeg;

      #endregion

      #region Private fields

      private readonly GameObject m_Obj;

      private readonly float m_TotalDist;

      private readonly List<GameObject> m_SegObjs;

      private readonly List<LineCreator> m_Children;

      private readonly List<ParamChangeEvent> m_ParamChangeEvents;

      private readonly Dictionary<ParamType, Vector3> m_Params;

      private readonly uint m_Level;

      private Vector3 m_Position;

      private float m_Distance;

      private float m_NextSplitDist;

      private int m_NextEventIdx;

      #endregion

      #region Constructors

      static LineCreator()
      {
         TotalNumSeg = 0;
      }

      public LineCreator(Vector3 a_StartingPos,
                         float a_TotalDistance,
                         uint a_Level,
                         List<ParamChangeEvent> a_ParamChangeEvents)
      {
         m_Obj = new GameObject("Line");
         m_Level = a_Level;
         new GameObject("Segments").transform.parent = m_Obj.transform;
         m_TotalDist = a_TotalDistance;
         m_SegObjs = new List<GameObject>();
         m_Children = new List<LineCreator>();
         m_ParamChangeEvents = a_ParamChangeEvents;
         m_Params = new Dictionary<ParamType, Vector3>();

         m_Position = a_StartingPos;
         m_Distance = 0;
         m_NextSplitDist = -1;
         m_NextEventIdx = 0;

         foreach (ParamType paramType in Enum.GetValues(typeof(ParamType)))
            m_Params[paramType] = Vector3.zero;
      }

      #endregion

      #region Static methods

      private static float SampleNormalDistribution(float a_Mean, float a_StdDev)
      {
         // http://stackoverflow.com/questions/218060/random-gaussian-variables
         float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(1 - Random.value)) *
                               Mathf.Sin(2.0f * Mathf.PI * (1 - Random.value));
         return a_Mean + a_StdDev * randStdNormal;
      }

      #endregion

      #region Methods

      public void Dispose()
      {
         Destroy(m_Obj);
      }

      public void Generate()
      {
         while (m_Distance < m_TotalDist)
         {
            UpdateParams();
            GenerateSegment();

            if (m_NextSplitDist < 0)
               SetNextSplitDistance();

            if (m_Distance >= m_NextSplitDist)
               Split();

            m_Distance += m_Params[ParamType.DISTANCE_INCREMENT].x;

            if (TotalNumSeg > MAX_NUM_SEGS)
            {
               Debug.logger.Log("TOO MANY SEGMENTS");
               return;
            }
         }
      }

      private void UpdateParams()
      {
         if (m_NextEventIdx == m_ParamChangeEvents.Count)
            return;

         var nextEvent = m_ParamChangeEvents[m_NextEventIdx];

         while (nextEvent.RelPos + Mathf.Epsilon >= m_Distance / m_TotalDist)
         {
            m_Params[nextEvent.ParamType] = nextEvent.NewVal;
            ++m_NextEventIdx;
            if (m_NextEventIdx == m_ParamChangeEvents.Count)
               return;
            nextEvent = m_ParamChangeEvents[m_NextEventIdx];
         }
      }

      private void GenerateSegment()
      {
         m_Params[ParamType.DIRECTION] +=
            Vector3.Lerp(Random.insideUnitSphere * m_Params[ParamType.RANDOMNESS].x,
               m_Params[ParamType.GENERAL_DIRECTION],
               m_Params[ParamType.STRAIGHTNESS].x);
         m_Params[ParamType.DIRECTION] = m_Params[ParamType.DIRECTION].normalized;
         m_Params[ParamType.DIAMETER] += m_Params[ParamType.DIAMETER_LINEAR_CHANGE_RATE];

         CreateSegmentObj();
      }

      private void CreateSegmentObj()
      {
         ++TotalNumSeg;

         var cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
         cylinder.transform.parent = m_Obj.transform.Find("Segments").transform;

         var scale = cylinder.transform.localScale;
         scale.x = m_Params[ParamType.DIAMETER].x;
         scale.y = m_Params[ParamType.DISTANCE_INCREMENT].x;
         scale.z = m_Params[ParamType.DIAMETER].x;
         cylinder.transform.localScale = scale;

         var endPos = m_Position +
                      m_Params[ParamType.DISTANCE_INCREMENT].x * m_Params[ParamType.DIRECTION];

         cylinder.transform.position = (m_Position + endPos) / 2f;

         var segDir = endPos - m_Position;
         var rotation = new Quaternion();
         rotation.SetLookRotation(new Vector3(segDir.x, segDir.y, segDir.z));
         cylinder.transform.rotation = rotation;
         cylinder.transform.Rotate(Vector3.left, 90);

         m_Position = endPos;

         m_SegObjs.Add(cylinder);
      }

      private void Split()
      {
         if (m_Level == MAX_NUM_LEVELS)
         {
            Debug.logger.Log("MAX LEVEL REACHED");
            return;
         }
         /*
         var dir2D = Random.insideUnitCircle;
         var dir = new Vector3(dir2D.x, 0, dir2D.y);
         */
         
         float mean = m_Params[ParamType.SPLIT_ANGLE_AVERAGE].x;
         float stdDev = m_Params[ParamType.SPLIT_ANGLE_STD_DEV].x;
         float phi = Random.value * 2 * Mathf.PI;
         float theta = SampleNormalDistribution(mean, stdDev) * Mathf.Deg2Rad;
         var dir = new Vector3(
            Mathf.Sin(theta) * Mathf.Cos(phi),
            Mathf.Cos(theta),
            Mathf.Sin(theta) * Mathf.Sin(phi));

         //var mat = Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(dir), Vector3.one);
         //dir = mat * m_Params[ParamType.DIRECTION];

         //var rotation = Quaternion.LookRotation(m_Params[ParamType.DIRECTION], dir);

         var paramChanges = new List<ParamChangeEvent>();

         float distFactor = m_Params[ParamType.SPLIT_CHILD_TOTAL_DIST_FACTOR].x;

         foreach (var paramAndVal in m_Params)
         {
            var paramType = paramAndVal.Key;
            var val = paramAndVal.Value;

            switch (paramType)
            {
            case ParamType.DIRECTION:
               val = dir;
               break;
            case ParamType.GENERAL_DIRECTION:
               val = dir;
               break;
            case ParamType.SPLIT_DISTANCE_AVERAGE:
               val = m_Params[ParamType.SPLIT_DISTANCE_AVERAGE] * distFactor;
               break;
            default:
               break;
            }

            paramChanges.Add(new ParamChangeEvent
            {
               RelPos = 0,
               ParamType = paramAndVal.Key,
               NewVal = val
            });
         }
         var childLine = new LineCreator(m_Position, m_TotalDist * distFactor, m_Level + 1, paramChanges);
         childLine.m_Obj.transform.parent = m_Obj.transform;
         childLine.Generate();
         m_Children.Add(childLine);
         m_NextSplitDist = -1;
      }

      private void SetNextSplitDistance()
      {
         float mean = m_Params[ParamType.SPLIT_DISTANCE_AVERAGE].x;
         float stdDev = m_Params[ParamType.SPLIT_DISTANCE_STD_DEV].x;
         m_NextSplitDist = m_Distance + SampleNormalDistribution(mean, stdDev);
      }

      #endregion
   }

   #endregion

   #region Nested types

   public enum ParamType
   {
      // Vector3
      DIRECTION,
      // float
      DISTANCE_INCREMENT,
      // Vector3
      GENERAL_DIRECTION,
      // float[0,inf]
      DIAMETER,
      // float[-inf,inf]
      DIAMETER_LINEAR_CHANGE_RATE,
      // float[0,inf]
      RANDOMNESS,
      // float[0,1]
      STRAIGHTNESS,

      //// Splitting ////

      // float[0,inf]
      SPLIT_DISTANCE_AVERAGE,
      // float[0,inf]
      SPLIT_DISTANCE_STD_DEV,
      // float[0,180]
      SPLIT_ANGLE_AVERAGE,
      // float[0, inf]
      SPLIT_ANGLE_STD_DEV,

      // float[0, inf]
      SPLIT_CHILD_TOTAL_DIST_FACTOR
   }

   #endregion

   #region Compile-time constants

   private const string INSTRUCTIONS_FILE_PATH =
      "B:\\Dev\\MVS\\Metabolism\\Assets\\Atompacman\\test.txt";

   #endregion

   #region Private fields

   private LineCreator m_LineCreator;

   #endregion

   #region Methods

   [UsedImplicitly]
   private void Start()
   {
      m_LineCreator = null;
   }

   [UsedImplicitly]
   private void Update()
   {
      if (!Input.GetKeyDown(KeyCode.G) && m_LineCreator != null)
         return;

      if (m_LineCreator != null)
         m_LineCreator.Dispose();

      LineCreator.TotalNumSeg = 0;

      var paramChanges = new List<LineCreator.ParamChangeEvent>();

      foreach (string line in File.ReadAllLines(INSTRUCTIONS_FILE_PATH))
      {
         var tokens = line.Split('|');
         Debug.Assert(tokens.Length == 5);
         for (int i = 0; i < tokens.Length; i++)
            tokens[i] = tokens[i].Trim();
         paramChanges.Add(new LineCreator.ParamChangeEvent
         {
            RelPos = float.Parse(tokens[0]),
            ParamType = (ParamType) Enum.Parse(typeof(ParamType), tokens[1]),
            NewVal = new Vector3(float.Parse(tokens[2]),
               float.Parse(tokens[3]),
               float.Parse(tokens[4]))
         });
      }

      m_LineCreator = new LineCreator(Vector3.zero, 10, 0, paramChanges);
      m_LineCreator.Generate();
   }

   #endregion
}
