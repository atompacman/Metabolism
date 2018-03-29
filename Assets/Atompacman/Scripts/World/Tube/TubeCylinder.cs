// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// MIT License
// Copyright (c) 2018 Stained Glass Guild
// See file "LICENSE.txt" at project root for complete license
// ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~
// Project: Metabolism
// File: TubeCylinder.cs
// Creation: 2018-03
// Author: Jérémie Coulombe
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

using JetBrains.Annotations;

using UnityEngine;

namespace Metabolism.World.Tube
{
   public sealed class TubeCylinder : MonoBehaviour
   {
      #region Public fields

      [Range(-1, 1)]
      public float RadiusSlope;

      [Range(3, 100)]
      public int NumSides;

      [Range(-1, 1)]
      public float Skewness;

      #endregion

      #region Methods

      [UsedImplicitly]
      private void Update()
      {
         AssertUniformelyScaled();
      }

      [UsedImplicitly]
      private void OnValidate()
      {
         AssertUniformelyScaled();

         this.EditorCompatibleDestroyAllChildren();

         var cylinderRotation = transform.rotation;
         var cylinderScale = transform.localScale;

         transform.rotation = Quaternion.identity;
         transform.localScale = Vector3.one;

         float angleRadDelta = 2 * Mathf.PI / NumSides;

         for (int i = 0; i < NumSides; ++i)
         {
            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.transform.parent = transform;
            quad.layer = 4;

            float angleRad = i * angleRadDelta;
            float posSkew = Mathf.Min(1, Skewness + 1);
            float negSkew = Mathf.Min(1, -Skewness + 1);
            var direction = new Vector3(posSkew * Mathf.Cos(angleRad), 0,
               negSkew * Mathf.Sin(angleRad));

            quad.transform.position = transform.position + direction * 0.5f;
            quad.transform.forward = new Vector3(negSkew * Mathf.Cos(angleRad), 0,
               posSkew * Mathf.Sin(angleRad));

            float maxSkew = Mathf.Max(posSkew, negSkew);
            float width = maxSkew * angleRadDelta *
                          Mathf.Sqrt(Mathf.Max(maxSkew * Mathf.Sin(angleRadDelta),
                             Mathf.Abs(Mathf.Cos(angleRad))));
            width = angleRadDelta * 0.6f;
            quad.transform.localScale = new Vector3(width, 1, 1);
         }

         transform.rotation = cylinderRotation;
         transform.localScale = cylinderScale;
      }

      private void AssertUniformelyScaled()
      {
         // We make sure that the cylinder is uniformely scaled so that normals are not skewed
         // We use the "Skewness" parameter to alter scale
         Debug.Assert(Mathf.Approximately(transform.localScale.x, transform.localScale.z));
      }

      #endregion
   }
}
