// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// MIT License
// Copyright (c) 2018 Stained Glass Guild
// See file "LICENSE.txt" at project root for complete license
// ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~
// Project: Metabolism
// File: TubeSegment.cs
// Creation: 2018-03
// Author: Jérémie Coulombe
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

using System.Collections;

using JetBrains.Annotations;

using UnityEngine;

namespace Metabolism.World.Tube
{
   public sealed class TubeSegment : MonoBehaviour
   {
      #region Public fields

      [Range(0, 1)]
      public float InitialRadius;

      [Range(0, 1)]
      public float FinalRadius;

      [Range(-1, 1)]
      public float InitialSkewness;

      [Range(-1, 1)]
      public float FinalSkewness;

      [Range(3, 100)]
      public int NumSides;

      [Range(1, 100)]
      public int NumCylinders;

      #endregion

      #region Methods

      [UsedImplicitly]
      private void OnValidate()
      {
         this.EditorCompatibleDestroyAllChildren();

         float cylinderLength = 1 / (float) NumCylinders;

         for (int i = 0; i < NumCylinders; ++i)
         {
            float progression = i * cylinderLength;

            var cylinder = new GameObject("Cylinder").AddComponent<TubeCylinder>();
            cylinder.transform.parent = transform;

            float radius = Mathf.Lerp(InitialRadius, FinalRadius, progression);
            cylinder.transform.localScale = new Vector3(radius, radius, cylinderLength);

            cylinder.transform.position = Vector3.forward * (progression - 0.5f);

            cylinder.NumSides = NumSides;
            cylinder.Skewness = Mathf.Lerp(InitialSkewness, FinalSkewness, progression);
         }

         //Instantiate(ResourceRepository.Instance.PrimitiveRightTriangle);
      }

      [UsedImplicitly]
      private void Update()
      {
         // We make sure that the object is uniformely scaled so that normals are not skewed
         Debug.Assert(Utils.IsUniformelyScaled(transform));
      }

      #endregion
   }
}
