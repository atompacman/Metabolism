// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// MIT License
// Copyright (c) 2018 Stained Glass Guild
// See file "LICENSE.txt" at project root for complete license
// ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~
// Project: Metabolism
// File: TubeNetwork.cs
// Creation: 2018-03
// Author: Jérémie Coulombe
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

using JetBrains.Annotations;

using UnityEngine;

namespace Metabolism.World.Tube
{
   public sealed class TubeNetwork : MonoBehaviour
   {
      private static void Recursive(int a_Lvl)
      {
         
      }

      [UsedImplicitly]
      private void Start()
      {
         var segment = new GameObject {name = "Segment"};
         segment.AddComponent<TubeSegment>();
         segment.transform.parent = transform;

         Recursive(4);

         for (int i = 0; i < 10; ++i)
         {
            var cylinderObj = new GameObject { name = "Cylinder" };
            var cylinder = cylinderObj.AddComponent<TubeCylinder>();
            cylinder.transform.parent = segment.transform;

            cylinder.NumSides = 40;
            cylinder.Skewness = 2;

            cylinder.transform.position = Vector3.forward * i * 100;
            cylinder.transform.localScale = new Vector3(20, 100, 20);
            cylinder.transform.rotation = Quaternion.Euler(-62, 110, 6);
         }
      }
   }
}
