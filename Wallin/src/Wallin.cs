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
using System;

namespace Wallin
{
  public class TestWallin
  {
    static List< Variable > MakeTerranBuildings( ghost.Domain domain )
    {
      return new List< Variable > {
        new Variable( "A", "Terran_Academy", domain, 3, 2, 0, 3, 7, 8, Race.Terran, 2 ),
        new Variable( "A", "Terran_Academy", domain, 3, 2, 0, 3, 7, 8, Race.Terran, 2 ),
        new Variable( "B", "Terran_Barracks", domain, 4, 3, 8, 7, 15, 16, Race.Terran, 1 ),
        new Variable( "B", "Terran_Barracks", domain, 4, 3, 8, 7, 15, 16, Race.Terran, 1 ),
        new Variable( "U", "Terran_Bunker", domain, 3, 2, 8, 15, 15, 16, Race.Terran, 2 ),
        new Variable( "U", "Terran_Bunker", domain, 3, 2, 8, 15, 15, 16, Race.Terran, 2 ),
        new Variable( "F", "Terran_Factory", domain, 4, 3, 8, 7, 7, 8, Race.Terran, 2 ),
        new Variable( "F", "Terran_Factory", domain, 4, 3, 8, 7, 7, 8, Race.Terran, 2 ),
        new Variable( "S", "Terran_Supply_Depot", domain, 3, 2, 10, 9, 5, 10, Race.Terran, 0 ),
        new Variable( "S", "Terran_Supply_Depot", domain, 3, 2, 10, 9, 5, 10, Race.Terran, 0 )
      };
    }

    static void Main( string[] args )
    {
      var unbuildables = new List< SetVariables.Point > {
        new SetVariables.Point( 7, 12 ), 
        new SetVariables.Point( 7, 13 ), 
        new SetVariables.Point( 7, 14 ), 
        new SetVariables.Point( 7, 15 ), 
        new SetVariables.Point( 8, 10 ), 
        new SetVariables.Point( 8, 11 ), 
        new SetVariables.Point( 8, 12 ), 
        new SetVariables.Point( 8, 13 ), 
        new SetVariables.Point( 8, 14 ), 
        new SetVariables.Point( 8, 15 ), 
        new SetVariables.Point( 9, 10 ), 
        new SetVariables.Point( 9, 11 ), 
        new SetVariables.Point( 9, 12 ), 
        new SetVariables.Point( 9, 13 ), 
        new SetVariables.Point( 9, 14 ), 
        new SetVariables.Point( 9, 15 ), 
        new SetVariables.Point( 10, 8 ), 
        new SetVariables.Point( 10, 9 ), 
        new SetVariables.Point( 10, 10 ), 
        new SetVariables.Point( 10, 11 ), 
        new SetVariables.Point( 10, 12 ), 
        new SetVariables.Point( 10, 13 ), 
        new SetVariables.Point( 10, 14 ), 
        new SetVariables.Point( 10, 15 ), 
        new SetVariables.Point( 11, 8 ), 
        new SetVariables.Point( 11, 9 ), 
        new SetVariables.Point( 11, 10 ), 
        new SetVariables.Point( 11, 11 ), 
        new SetVariables.Point( 11, 12 ), 
        new SetVariables.Point( 11, 13 ), 
        new SetVariables.Point( 11, 14 ), 
        new SetVariables.Point( 11, 15 ) 
      };

      var domain = new ghost.Domain( 16 * 12, 0 );

      var listBuildings = MakeTerranBuildings( domain );

      var setBuildings = new SetVariables( listBuildings,
                                           12,
                                           16,
                                           new SetVariables.Point( 11, 7 ),
                                           new SetVariables.Point( 6, 15 ) );

      setBuildings.Unbuildable( unbuildables );

      var constraints = new List< Constraint > {
        new OverLap( setBuildings ),
        new Buildable( setBuildings ),
        new WallShape( setBuildings ),
        new StartingTargetTiles( setBuildings )
      };

      var objective = new GapObjective();

      var solver = new ghost.Solver< Variable, SetVariables, Constraint >( setBuildings, constraints, objective );

      Console.WriteLine( "Start solving trivial test" );
      solver.solve( 20, 150 );


        // 3.4 GHz, mono 20,150 7 tours 44 iterations
        //            C++      12 tours 400 iteratons
    }
  }
}

