// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// MIT License
// Copyright (c) 2018 Stained Glass Guild
// See file "LICENSE.txt" at project root for complete license
// ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~
// Project: Metabolism
// File: ResourceRepository.cs
// Creation: 2018-03
// Author: Jérémie Coulombe
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

using JetBrains.Annotations;

using UnityEngine;

namespace Metabolism
{
   public sealed class ResourceRepository : MonoBehaviour
   {
      #region Static fields

      public static ResourceRepository Instance;

      #endregion

      #region Public fields

      public GameObject PrimitiveRightTriangle;

      #endregion

      #region Static methods

      private static Mesh CreatePrimitiveTriangleMesh()
      {
         return new Mesh
         {
            vertices = new[]
            {
               new Vector3(-0.5f, -0.5f, 0),
               new Vector3(-0.5f, 0.5f, 0),
               new Vector3(0.5f, -0.5f, 0)
            },
            triangles = new[]
            {
               0,
               1,
               2
            },
            normals = new[]
            {
               new Vector3(0, 0, -1),
               new Vector3(0, 0, -1),
               new Vector3(0, 0, -1)
            }
         };
      }

      #endregion

      #region Methods

      [UsedImplicitly]
      private void Awake()
      {
         Instance = this;

         // Since it's complicated to save a prefab with a procedurally generated mesh, we 
         // regenerate it at game startup.
         PrimitiveRightTriangle.GetComponent<MeshFilter>().mesh = CreatePrimitiveTriangleMesh();
      }

      #endregion
   }
}
