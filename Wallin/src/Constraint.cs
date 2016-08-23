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

namespace Wallin
{
  public abstract class Constraint : ghost.Constraint<SetVariables, Variable>
  {
    protected Constraint( SetVariables variables ) : base( variables ) { }

    // Used by WallShape and StartingTargetTiles constraints
    public override Dictionary<int, double> SimulateCost( int currentVariableIndex )
    {
      // for each value in currentVariableIndex's domain, save the constraint cost value.
      var simCosts = new Dictionary<int, double>();

      int backup = Variables.GetValue( currentVariableIndex );
      int previousPos = -42;

      foreach( var pos in Variables.PossibleValues( currentVariableIndex ) )
      {
        if( pos >= 1 && pos == previousPos + 1 )
          Variables.QuickShift( currentVariableIndex );
        else
          Variables.SetValue( currentVariableIndex, pos );

        simCosts[ pos ] = Cost();

        previousPos = pos;
      }

      Variables.SetValue( currentVariableIndex, backup );
      return simCosts;
    }

    public bool IsWall()
    {
      int size = Variables.GetNumberVariables();
      int sizeSelected = 0;
      var neighbors = new Dictionary<int, List<int> >();
      for( int i = 0 ; i < size ; ++i )
        neighbors[ i ] = Variables.GetBuildingsAround( i );

      int indexRandomBuilding = -1;
      for( int i = 0 ; i < size ; ++i )
        if( Variables.IsSelected( i ) )
        {
          indexRandomBuilding = i;
          ++sizeSelected;
        }

      if( indexRandomBuilding == -1 || 
         ( neighbors[ indexRandomBuilding ].Count == 0 && sizeSelected > 1 ) )
        return false;

      var visited = new List<int>();
      var toVisit = neighbors[ indexRandomBuilding ].ConvertAll( x => x );
      visited.Add( indexRandomBuilding );

      while( toVisit.Count > 0 )
      {
        var first = toVisit[ 0 ];
        toVisit.Remove( first );

        foreach( var n in neighbors[ first ] )
          if( !visited.Contains( n ) )
            toVisit.Add( n );

        visited.Add( first );
      }

      return visited.Count == sizeSelected;
    }

    public bool IsWallWithSTTiles()
    {
      var startingBuildings = Variables.BuildingsAt( Variables.StartingTile );
      if( startingBuildings.Count != 1 )
        return false;

      var targetBuildings = Variables.BuildingsAt( Variables.TargetTile );
      if( targetBuildings.Count != 1 )
        return false;

      // if same building on both the starting and target tile
      if( startingBuildings[ 0 ] == targetBuildings[ 0 ] )
        return true;
      else
      {
        var nberCurrent = startingBuildings[ 0 ];
        var nberTarget = targetBuildings[ 0 ];

        List< int > toVisit = Variables.GetBuildingsAround( nberCurrent );
        var visited = new List<int>();
        List< int > neighbors;

        visited.Add( nberCurrent );

        if( toVisit.Contains( nberTarget ) )
          return true;

        while( toVisit.Count > 0 )
        {
          var first = toVisit[ 0 ];
          toVisit.Remove( first );
          neighbors = Variables.GetBuildingsAround( first );

          foreach( var n in neighbors )
          {
            if( n == nberTarget )
              return true;
            if( !visited.Contains( n ) )
              toVisit.Add( n );
          }

          visited.Add( first );
        }
        return false;
      }
    }
  }

  /***********/
  /* Overlap */
  /***********/  
  public class OverLap : Constraint
  {
    public OverLap( SetVariables variables ) : base( variables ) { }

    public override double Cost()
    {
      double conflicts = 0.0;

      foreach( var failures in Variables.Failures )
      {
        int nbConflict = failures.Value.Length - 1;
        if( nbConflict > 0 && !failures.Value.Contains( "###" ) )
          conflicts += nbConflict;
      }

      return conflicts;    
    }

    public override Dictionary<int, double> SimulateCost( int currentVariableIndex )
    {
      var simCosts = new Dictionary<int, double>();

      int backup = Variables.GetValue( currentVariableIndex );
      int previousPos = -42;
      int diff, dummy;

      foreach( var pos in Variables.PossibleValues( currentVariableIndex ) )
      {
        if( pos >= 1 && pos == previousPos + 1 )
        {
          Variables.Shift( currentVariableIndex, out diff, out dummy );
          simCosts[ pos ] = simCosts[ pos - 1 ] + diff;
        }
        else
        { 
          Variables.SetValue( currentVariableIndex, pos );
          simCosts[ pos ] = Cost();
        }

        previousPos = pos;
      }

      Variables.SetValue( currentVariableIndex, backup );

      return simCosts;
    }
  }

  /*************/
  /* Buildable */
  /*************/  
  public class Buildable : Constraint
  {
    public Buildable( SetVariables variables ) : base( variables ) { }

    public override double Cost()
    {
      // count number of buildings misplaced on unbuildable tiles (denoted by ###)
      double conflicts = 0.0;

      foreach( var failures in Variables.Failures )
      {
        if( failures.Value.Contains( "###" ) )
          conflicts += failures.Value.Length - 3;
      }

      return conflicts;    
    }

    public override Dictionary<int, double> SimulateCost( int currentVariableIndex )
    {
      var simCosts = new Dictionary<int, double>();

      int backup = Variables.GetValue( currentVariableIndex );
      int previousPos = -42;
      int diff, dummy;

      foreach( var pos in Variables.PossibleValues( currentVariableIndex ) )
      {
       if( pos >= 1 && pos == previousPos + 1 )
        {
          Variables.Shift( currentVariableIndex, out dummy, out diff );
          simCosts[ pos ] = simCosts[ pos - 1 ] + diff;
        }
        else
        { 
          Variables.SetValue( currentVariableIndex, pos );
          simCosts[ pos ] = Cost();
        }

        previousPos = pos;
      }

      Variables.SetValue( currentVariableIndex, backup );

      return simCosts;
    }
  }

  /*************/
  /* WallShape */
  /*************/  
  public class WallShape : Constraint
  {
    public WallShape( SetVariables variables ) : base( variables ) { }

    public override double Cost()
    {
      // cost = |buildings with one neighbor| - 1 + |buildings with no neighbors|
      double conflicts = 0.0;

      if( !IsWall() )
      {
        int nberNeighbors;
        var oneNeighborBuildings = new List<int>();

        int widthNeeded = Math.Abs( Variables.TargetTile.HorizontalPosition - Variables.StartingTile.HorizontalPosition );
        int heightNeeded = Math.Abs( Variables.TargetTile.VerticalPosition - Variables.StartingTile.VerticalPosition );
        int width = 0;
        int height = 0;
        var notSelected = new List<int>();


        for( int i = 0 ; i < Variables.GetNumberVariables() ; ++i )
        {
          if( Variables.IsSelected( i ) )
          {
            if( !Variables.IsOnStartingOrTargetTile( i ) )
            {
              width += Variables.Length( i );
              height += Variables.Height( i );

              // if we don't have a wall, penalise all buildings on the domain
              // except those on starting and target tile
              ++conflicts;

              nberNeighbors = Variables.CountAround( i );

              if( nberNeighbors == 0 || nberNeighbors > 2 ) // to change with Protoss and pylons
                ++conflicts;
              else
              {
                if( nberNeighbors == 1 )
                  oneNeighborBuildings.Add( i );
              }
            }
          }
          else
            notSelected.Add( i );
        }
          
        //penalise unselected buildings if we need them
        if( width < widthNeeded || height < heightNeeded )
          foreach( var i in notSelected )
            conflicts += 2;

        if( oneNeighborBuildings.Count > 2 ) // for latter: pylons can be alone, or have 1 neighbor only
        {
          foreach( var index in oneNeighborBuildings )
            if( !Variables.IsOnStartingOrTargetTile( index ) )
              ++conflicts;
        }
      }

      return conflicts;    
    }

    public double PostprocessSimulateCost( int currentVariableIndex, int newPosition )
    {
      int backup = Variables.GetValue( currentVariableIndex );
      Variables.SetValue( currentVariableIndex, newPosition );

      double simCost = Cost();

      Variables.SetValue( currentVariableIndex, backup );

      return simCost;
    }
  }

  /***********************/
  /* StartingTargetTiles */
  /***********************/  
  public class StartingTargetTiles : Constraint
  {
    public StartingTargetTiles( SetVariables variables ) : base( variables ) { }

    public override double Cost()
    {
      // no building on one of these two tiles: cost of the tile = 6
      // a building with no or with 2 or more neighbors: cost of the tile = 3
      // two or more buildings on one of these tile: increasing penalties.
      double conflicts = 0.0;

      var startingBuildings = Variables.BuildingsAt( Variables.StartingTile );
      var targetBuildings = Variables.BuildingsAt( Variables.TargetTile );

      int neighbors;

      // if same building on both the starting and target tile
      if( startingBuildings.Count == 1 && targetBuildings.Count == 1 && startingBuildings[0] == targetBuildings[0] )
        return 0.0;

      if( startingBuildings.Count == 0 )
      {
        // penalize buildings not placed on the domain
        for( int i = 0 ; i < Variables.GetNumberVariables() ; ++i )
          if( !Variables.IsSelected( i ) )
            conflicts += 2;
      }
      else
      {
        foreach( var index in startingBuildings )
        {
          neighbors = Variables.CountAround( index );

          if( neighbors != 1 )
            conflicts += 2;
        }
      }

      if( targetBuildings.Count == 0 )
      {      
        // penalize buildings not placed on the domain
        for( int i = 0 ; i < Variables.GetNumberVariables() ; ++i )
          if( !Variables.IsSelected( i ) )
            conflicts += 2;
      }
      else
      {
        foreach( var index in targetBuildings )
        {
          neighbors = Variables.CountAround( index );

          if( neighbors != 1 )
            conflicts += 2;
        }
      }

      return conflicts;
    }
  }
}

