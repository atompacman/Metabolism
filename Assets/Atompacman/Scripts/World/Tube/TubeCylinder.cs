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

      private GameObject CreateRightTriangle(Vector3 a_RightAnglePos,
         Vector3 a_CwPos,
         Vector3 a_CcwPos
                                             )
      {
         var triangle = Instantiate(ResourceRepository.Instance.PrimitiveRightTriangle);
         triangle.layer = LocalGravity.GRAVITATION_COLLIDERS_LAYER;
         triangle.transform.parent = transform;
         triangle.transform.position = a_RightAnglePos;

         var toCcw = a_CcwPos - a_RightAnglePos;
         var toCw = a_CwPos - a_RightAnglePos;
         triangle.transform.localScale = new Vector3(toCcw.magnitude, toCw.magnitude, 1);
         var normal = Vector3.Cross(toCcw, toCw);
         triangle.transform.rotation = Quaternion.LookRotation(normal.normalized, toCw.normalized);

         return triangle;
      }

      [UsedImplicitly]
      private void OnValidate()
      {
         AssertUniformelyScaled();

         // Remove all children to recreate cylinder triangles
         this.EditorCompatibleDestroyAllChildren();

         // Save the global rotation and scale transforms to re-apply them after children creation
         // to make it simpler
         var cylinderRotation = transform.rotation;
         var cylinderScale = transform.localScale;
         transform.rotation = Quaternion.identity;
         transform.localScale = Vector3.one;


         // Create children triangles
         float angleRadDelta = 2 * Mathf.PI / NumSides;
         
         for (int i = 0; i < NumSides; ++i)
         {
            //
            //          e---f
            //         /|\  |\
            //        / | \ | \
            //       /  |  \|  \
            //      a---c---d---b
            //
            float angleA = i * angleRadDelta;
            float angleb = (i + 1) * angleRadDelta;
            var a = new Vector3(Mathf.Cos(angleA), 0, Mathf.Sin(angleA));
            var b = new Vector3(Mathf.Cos(angleb), 0, Mathf.Sin(angleb));
            var ab = b - a;
            var c = a + ab * RadiusSlope * 0.5f;
            var d = b - ab * RadiusSlope * 0.5f;
            var e = a * (1 - RadiusSlope) + Vector3.up;
            var f = b * (1 - RadiusSlope) + Vector3.up;
            
            CreateRightTriangle(c, e, a);
            CreateRightTriangle(c, d, e);
            CreateRightTriangle(f, e, d);
            CreateRightTriangle(d, b, f);

            /*
            var triangleA = CreateTriangle();
            var triangleB = CreateChildTriangle();

            float angleRad = i * angleRadDelta;
            float posSkew = Mathf.Min(1, Skewness + 1);
            float negSkew = Mathf.Min(1, -Skewness + 1);
            var direction = new Vector3(posSkew * Mathf.Cos(angleRad), 0,
               negSkew * Mathf.Sin(angleRad));

            triangleA.transform.position = transform.position + direction * 0.5f;
            triangleA.transform.forward = new Vector3(negSkew * Mathf.Cos(angleRad), 0,
               posSkew * Mathf.Sin(angleRad));

            float maxSkew = Mathf.Max(posSkew, negSkew);
            float width = maxSkew * angleRadDelta *
                          Mathf.Sqrt(Mathf.Max(maxSkew * Mathf.Sin(angleRadDelta),
                             Mathf.Abs(Mathf.Cos(angleRad))));
            width = angleRadDelta * 0.6f;
            triangleA.transform.localScale = new Vector3(width, 1, 1);
            
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
            */
         }
         
         // Apply original transforms at the end
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
