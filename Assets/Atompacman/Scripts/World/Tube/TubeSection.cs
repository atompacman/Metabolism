// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// MIT License
// Copyright (c) 2018 Stained Glass Guild
// See file "LICENSE.txt" at project root for complete license
// ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~
// Project: Metabolism
// File: TubeSection.cs
// Creation: 2018-03
// Author: Jérémie Coulombe
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

using JetBrains.Annotations;

using UnityEngine;

namespace Metabolism.World.Tube
{
   [ExecuteInEditMode]
   public sealed class TubeSection : MonoBehaviour
   {
      #region Public fields

      public Transform SliceA;

      public Transform SliceB;

      public bool IsIndependent;

      [Range(3, 100)]
      public int NumSides;
      
      #endregion

      #region Methods

      [UsedImplicitly]
      private void Update()
      {
         Debug.Assert(Utils.IsIdentity(transform));
         
         if (SliceA.hasChanged || SliceB.hasChanged)
         {
            RegenerateMesh();
            if (IsIndependent)
            {
               SliceA.hasChanged = false;
               SliceB.hasChanged = false;
            }
         }
      }

      [UsedImplicitly]
      private void OnValidate()
      {
         RegenerateMesh();
      }

      public void RegenerateMesh()
      {
         float angleRadDelta = 2 * Mathf.PI / NumSides;

         var vertices = new Vector3[NumSides * 2];
         var normals = new Vector3[NumSides * 2];
         var triangles = new int[NumSides * 2 * 3];

         // Create vertices and normals
         for (int i = 0; i < NumSides; ++i)
         {
            float angle = i * angleRadDelta;
            var circleDir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * 0.5f;
            vertices[i*2] = SliceA.TransformPoint(circleDir);
            vertices[i*2+1] = SliceB.TransformPoint(circleDir);
            normals[i*2] = SliceA.TransformDirection(circleDir);
            normals[i*2+1] = SliceB.TransformDirection(circleDir);
         }

         // Create triangles
         for (int i = 0; i < NumSides; ++i)
         {
            triangles[i*2*3    ] = 2 * i + 1;
            triangles[i*2*3 + 1] = 2 * i;
            triangles[i*2*3 + 2] = (2 * i + 2) % (NumSides * 2);
            triangles[i*2*3 + 3] = 2 * i + 1;
            triangles[i*2*3 + 4] = (2 * i + 2) % (NumSides * 2);
            triangles[i*2*3 + 5] = (2 * i + 3) % (NumSides * 2);
         }

         GetComponent<MeshFilter>().mesh = new Mesh
         {
            vertices = vertices,
            normals = normals,
            triangles = triangles
         };
      }
      
      #endregion
   }
}
