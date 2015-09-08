/*
 * GHOST (General meta-Heuristic Optimization Solving Tool) is a C# library 
 * designed to solve combinatorial satisfaction and optimization problems within 
 * some tenth of milliseconds. It has been originally designed to handle 
 * StarCraft: Brood War-related problems. 
 * 
 * GHOST is a framework aiming to easily model and implement satisfaction and optimization
 * problems. It contains a meta-heuristic solver aiming to solve any kind of these problems 
 * represented by a CSP/COP. It is a generalization of the C++ Wall-in project (https://github.com/richoux/Wall-in) 
 * and a C# adaptation and improvement of the GHOST's C++ version (https://github.com/richoux/GHOST).
 * Please visit https://github.com/richoux/GHOST_C_sharp for further information.
 * 
 * Copyright (C) 2015 Florian Richoux
 *
 * This file is part of GHOST.
 * GHOST is free software: you can redistribute it and/or 
 * modify it under the terms of the GNU General Public License as published 
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.

 * GHOST is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.

 * You should have received a copy of the GNU General Public License
 * along with GHOST. If not, see http://www.gnu.org/licenses/.
 */

using System.Collections.Generic;
using NUnit.Framework;
using Wallin;
using System;
using System.Linq;

namespace GhostTest
{
  [TestFixture]
  public class ConstraintWallinTest
  {
    public static List<Building> list = new List<Building> { 
      new Building( "a",
                    "aa",
                    2,
                    2,
                    0,
                    0,
                    0,
                    0,
                    Race.Unknown,
                    0,
                    0 ), 

      new Building( "b",
                    "bb",
                    2,
                    2,
                    0,
                    0,
                    0,
                    0,
                    Race.Unknown,
                    0,
                    0 ),

      new Building( "c",
                    "cc",
                    2,
                    2,
                    0,
                    0,
                    0,
                    0,
                    Race.Unknown,
                    0,
                    0 )
    }; 

    public static SetBuildings setBuildings = new SetBuildings( list, 
                                                                5, 
                                                                5, 
                                                                new SetBuildings.Point( 2, 3 ), 
                                                                new SetBuildings.Point( 0, 0 ) );

    [Test]
    public void SimulateCostBaseTest()
    {
      setBuildings.SetValue( 0, -1 );
      setBuildings.SetValue( 1, 0 );
      setBuildings.SetValue( 2, 8 );
#if DEBUG
      setBuildings.Print();
#endif
      var varSimCost = new Dictionary<int, double[] >();
      for( int i = 0 ; i < setBuildings.Domain( 1 ).MaxValue() + 1 ; ++i )
      {
        varSimCost[ i ] = new double[ setBuildings.GetNumberVariables() ];
        for( var j = 0 ; j < setBuildings.GetNumberVariables() ; ++j )
          varSimCost[ i ][ j ] = -1;
      }

      var noHoles = new WallShape( setBuildings );
      var vecCosts = noHoles.SimulateCost( 1, varSimCost );

      foreach( var tuple in vecCosts )
        if( tuple.Key == 0 || tuple.Key == 7 || tuple.Key == 8 || tuple.Key == 12 || tuple.Key == 13 )
        {
          Assert.AreEqual( 2.0, tuple.Value );
        }
        else if( tuple.Key == 1 || tuple.Key == 6 || tuple.Key == 11 || tuple.Key == 16 || tuple.Key == 17 || tuple.Key == 18 )
        {
          Assert.AreEqual( 0.0, tuple.Value );
        }
        else
        {
          Assert.AreEqual( 4.0, tuple.Value );
        }
    }
  }
}

