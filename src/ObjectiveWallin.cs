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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace ghost
{
  public abstract class ObjectiveWallin : Objective< SetBuildings, Building >
  {
    protected ObjectiveWallin( string name ) : base( name ) 
    { 
      SizeWall = Int32.MaxValue;
    }

    public override double PostprocessSatisfaction( SetBuildings variables,
                                                   ref double bestCost,
                                                   List<int> solution,
                                                   double satTimeout )
    {
      var startPostprocess = new Stopwatch();

      bool change = true;
      double cost;
      var wallShape = new WallShape( variables );

      // find all buildings accessible from the starting building and remove all others
      int currentIndex = variables.BuildingsAt( variables.StartingTile )[ 0 ];
      var toVisit = variables.GetBuildingsAround( currentIndex );
      var visited = new List<int>();
      var neighbors = new List<int>();

      visited.Add( currentIndex );

      while( toVisit.Count > 0 )
      {
        currentIndex = toVisit[ 0 ];
        toVisit.Remove( currentIndex );
        neighbors = variables.GetBuildingsAround( currentIndex );

        visited.Add( currentIndex );

        foreach( var n in neighbors )
          if( !visited.Contains( n ) )
            toVisit.Add( n );
      }

      // remove all unreachable buildings from the starting building out of the domain
      for( int i = 0 ; i < variables.GetNumberVariables() ; ++i )
        if( !visited.Contains( i ) )
          variables.SetValue( i, -1 );

      var varSimCost = new double[ variables.GetNumberVariables() ];

      // clean wall from unnecessary buildings.
      do
      {
        for( int i = 0 ; i < variables.GetNumberVariables() ; ++i )
          if( ! variables.IsOnStartingOrTargetTile( i ) )
          {
            change = false;
            if( variables.IsSelected( i ) )
            {
              for( var j = 0 ; j < varSimCost.Count() ; ++j )
                varSimCost[ i ] = 0.0;

              cost = wallShape.PostprocessSimulateCost( i, -1, varSimCost );

              if( cost == 0.0 )
              {
                variables.SetValue( i, -1 );
                change = true;
              }   
            }
          }
      } while( change );

      double objectiveCost = Cost( variables );
      int currentSizeWall = 0;
      for( int i = 0 ; i < variables.GetNumberVariables() ; ++i )
        if( variables.IsSelected( i ) )
          ++currentSizeWall;

      if( objectiveCost < bestCost || ( objectiveCost == bestCost && currentSizeWall < SizeWall ) )
      {
        SizeWall = currentSizeWall;
        bestCost = objectiveCost;
          for( int i = 0 ; i < variables.GetNumberVariables() ; ++i )
          solution[ i ] = variables.GetValue( i );
      }
        
      return startPostprocess.ElapsedMilliseconds;
    }

    public override double PostprocessOptimization( SetBuildings variables,
                                                    ref double bestCost,
                                                    double optTimeout )
    {
      var startPostprocess = new Stopwatch();

      var tabuList = new List<int>( Enumerable.Repeat( 0, variables.GetNumberVariables() ) );

      var buildingSameSize = new Dictionary< int, List<int> >();

      for( int i = 0 ; i < variables.GetNumberVariables() ; ++i )
      {
        if( !buildingSameSize.ContainsKey( variables.Surface( i ) ) )
          buildingSameSize[ variables.Surface( i ) ] = new List<int>();

        buildingSameSize[ variables.Surface( i ) ].Add( i );
      }

      var goodVar = new List<int>();

      bestCost = Cost( variables );
      double currentCost = bestCost;

      long postprocessTimeLimit = (long)Math.Max( 1.0, optTimeout / 100 );

      while( startPostprocess.ElapsedMilliseconds < postprocessTimeLimit && bestCost > 0 )
      {
        goodVar.Clear();

        for( int i = 0 ; i < tabuList.Count ; ++i )
        {
          if( tabuList[ i ] <= 1 )
            tabuList[ i ] = 0;
          else
            --tabuList[ i ];
        }

        for( int i = 0 ; i < variables.GetNumberVariables() ; ++i )
        {
          if( tabuList[ i ] == 0 )
            goodVar.Add ( i );
        }

        if( goodVar.Count == 0 )
          for( int i = 0; i < variables.GetNumberVariables() ; ++i )
            goodVar.Add( i ); 

        int index = HeuristicVariable( goodVar, variables );
        var surface = buildingSameSize[ variables.Surface( index ) ];
        int toSwap = -1;

        foreach( var v in surface )
        {
          if( v != index )
          {
            variables.Swap( v, index );
            currentCost = Cost( variables );
            if( currentCost < bestCost )
            {
              bestCost = currentCost;
              toSwap = v;
            }

            variables.Swap( v, index );
          }

          if( toSwap > -1 )
            variables.Swap( toSwap, index );
        }

        tabuList[ index ] = 2;
      }

      return startPostprocess.ElapsedMilliseconds;
    }


    protected int SizeWall { get; private set; }
  }

  /**********/
  /* GapObj */
  /**********/
  public class GapObjective : ObjectiveWallin
  {
    public GapObjective() : base( "wallinGap" ) { }

    public override double Cost( SetBuildings variables )
    {
      double gaps = 0.0;

      for( int i = 0 ; i < variables.GetNumberVariables() ; ++i )
        gaps += GapSize( i, variables );

      return gaps;
    }

    public override int HeuristicVariable( List<int> indexes, SetBuildings variables )
    {
      var gapList = new List<int>();

      for( int i = 0 ; i < indexes.Count ; ++i )
        gapList.Add( GapSize( indexes[ i ], variables ) );

      int largestGap = gapList.Max();

      var largestGapIndex = new List<int>();
      for( int i = 0 ; i < gapList.Count ; ++i )
        if( gapList[ i ] == largestGap )
          largestGapIndex.Add( i );

      return largestGapIndex.Count == 1 ? largestGapIndex[ 0 ] : largestGapIndex[ Random.Next( 0, largestGapIndex.Count ) ];
    }

    public override int HeuristicValue( List<int> valuesIndex, int variableIndex, SetBuildings variables )
    {
      if( variables.PossibleValues( variableIndex )[ valuesIndex[ 0 ] ] == -1 )
        return valuesIndex[ 0 ];
      else
      {
        double minDist = double.MaxValue;
        double tmp;
        int minIndex = valuesIndex[0];
        foreach( var v in valuesIndex )
        {
          tmp = variables.RealDistanceSTTo( variables.PossibleValues( variableIndex )[ v ] );
          if( minDist > tmp )
          {
            minDist = tmp;
            minIndex = v;
          }
        }

        return minIndex;
      }
    }


    private static int GapSize( int index, SetBuildings variables )
    {
      if( !variables.IsSelected( index ) )
        return 0;

      int gaps = 0;

      var neighbors = variables.GetBuildingsAbove( index );
      gaps += neighbors.Count( x => variables.GapTop( index ) + variables.GapBottom( x ) >= 16 );

      neighbors = variables.GetBuildingsOnRight( index );
      gaps += neighbors.Count( x => variables.GapRight( index ) + variables.GapLeft( x ) >= 16 );

      neighbors = variables.GetBuildingsBelow( index );
      gaps += neighbors.Count( x => variables.GapBottom( index ) + variables.GapTop( x ) >= 16 );

      neighbors = variables.GetBuildingsOnLeft( index );
      gaps += neighbors.Count( x => variables.GapLeft( index ) + variables.GapRight( x ) >= 16 );

      return gaps;
    }
  }
}