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

using JetBrains.Annotations;

using UnityEngine;

namespace Metabolism.World.Tube
{
   [ExecuteInEditMode]
   public sealed class TubeSegment : MonoBehaviour
   {
      #region Public fields

      public Transform SliceA;

      public Transform SliceB;

      [SerializeField, UsedImplicitly]
      private Transform Sections;

      [SerializeField, UsedImplicitly]
      private Transform SubSlices;

      [SerializeField, UsedImplicitly]
      private Material Material;

      [Range(3, 100)]
      public int NumSides;

      [Range(1, 100)]
      public int NumSections;

      #endregion

      #region Methods

      [UsedImplicitly]
      private void Update()
      {
         Debug.Assert(Utils.IsIdentity(transform));
         Debug.Assert(Utils.IsIdentity(Sections));

         if (SliceA.hasChanged || SliceB.hasChanged)
         {
            RegenerateSections();
            SliceA.hasChanged = false;
            SliceB.hasChanged = false;
         }
      }

      [UsedImplicitly]
      private void LateUpdate()
      {
         foreach (Transform slice in SubSlices)
         {
            slice.hasChanged = false;
         }
      }

      [UsedImplicitly]
      private void OnValidate()
      {
         RegenerateSections();
      }

      private void RegenerateSections()
      {
         foreach (Transform section in Sections)
         {
            Utils.EditorCompatibleDestroy(this, section.gameObject);
         }
         foreach (Transform slice in SubSlices)
         {
            Utils.EditorCompatibleDestroy(this, slice.gameObject);
         }

         // Create slices
         var slices = new Transform[NumSections + 1];
         for (int i = 0; i <= NumSections; ++i)
         {
            var slice = new GameObject("Slice " + i).transform;
            slice.parent = SubSlices;
            float lerpFactor = i / (float)NumSections;
            slice.position = Vector3.Lerp(SliceA.position, SliceB.position, lerpFactor);
            slice.rotation = Quaternion.Lerp(SliceA.rotation, SliceB.rotation, lerpFactor);
            slice.localScale = Vector3.Lerp(SliceA.localScale, SliceB.localScale, lerpFactor);
            slices[i] = slice;
         }

         // Create sections
         for (int i = 0; i < NumSections; ++i)
         {
            var section = new GameObject("Section " + i).AddComponent<TubeSection>();
            section.transform.parent = Sections;
            section.NumSides = NumSides;
            section.IsIndependent = false;
            section.SliceA = slices[i];
            section.SliceB = slices[i+1];
            section.gameObject.AddComponent<MeshFilter>();
            section.gameObject.AddComponent<MeshRenderer>().material = Material;
            section.RegenerateMesh();
            var collider = section.gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = section.GetComponent<MeshFilter>().sharedMesh;
            section.gameObject.layer = LocalGravity.GRAVITATION_COLLIDERS_LAYER;
         }
      }

      #endregion
   }
}
