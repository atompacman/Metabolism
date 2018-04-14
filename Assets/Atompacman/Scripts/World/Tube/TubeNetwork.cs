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
      }
   }
}
