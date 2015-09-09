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

namespace Wallin
{
  public class SetVariables : ghost.SetVariables<Variable>
  {
    private List<string>[,] _matrixType;
    private List<int>[,] _matrixIndex;

    public SetVariables( List<Variable> buildings, int row, int column, Point startingTile, Point targetTile )
      : this( buildings, row, column, startingTile, targetTile, new Dictionary<string, string>() )  { }

    public SetVariables( List<Variable> buildings, 
                        int row,
                        int column,
                        Point startingTile,
                        Point targetTile,
                        Dictionary<string, string> failures )
      : base( buildings )      
    {
      Row = row;
      Column = column;
      StartingTile = startingTile;
      TargetTile = targetTile;
      Failures = failures;
      _matrixType = new List<string>[row , column];
      _matrixIndex = new List< int >[row , column];
      for( int r = 0 ; r < row ; ++r )
        for( int c = 0 ; c < column ; ++c )
        {
          _matrixIndex[ r, c ] = new List< int >();
          _matrixType[ r, c ] = new List< string >();
        }
      _matrixType[ startingTile.VerticalPosition, startingTile.HorizontalPosition ].Add( "@s" );
      _matrixType[ targetTile.VerticalPosition, targetTile.HorizontalPosition ].Add( "@t" );

      for( int i = 0 ; i < buildings.Count ; ++i )
      {
        var positionWithoutOutsideScope = PossiblePositions( i );
        positionWithoutOutsideScope.Remove( -1 );
        Variables[ i ].Domain = new ghost.Domain( positionWithoutOutsideScope, -1 );
      }
    }

    public class Point
    {
      public Point( int row, int col )
      {
        VerticalPosition = row;
        HorizontalPosition = col;
      }

      public Point() : this( 0, 0 ) { }

      public int VerticalPosition { get; set; }
      public int HorizontalPosition { get; set; }
    }

    public bool IsSelected( int index )
    {
      if( index >= 0 && index < Variables.Count )
        return Variables[ index ].IsSelected();
      else
        throw new IndexOutOfRangeException("Bad index for IsSelected method");
    }

    public int Length( int index )
    {
      if( index >= 0 && index < Variables.Count )
        return Variables[ index ].Length;
      else
        throw new IndexOutOfRangeException("Bad index for Length method");
    }

    public int Height( int index )
    {
      if( index >= 0 && index < Variables.Count )
        return Variables[ index ].Height;
      else
        throw new IndexOutOfRangeException("Bad index for Height method");
    }

    public int Surface( int index )
    {
      if( index >= 0 && index < Variables.Count )
        return Variables[ index ].Surface();
      else
        throw new IndexOutOfRangeException("Bad index for Surface method");
    }

    public int GapTop( int index )
    {
      if( index >= 0 && index < Variables.Count )
        return Variables[ index ].GapTop;
      else
        throw new IndexOutOfRangeException("Bad index for GapTop method");
    }

    public int GapRight( int index )
    {
      if( index >= 0 && index < Variables.Count )
        return Variables[ index ].GapRight;
      else
        throw new IndexOutOfRangeException("Bad index for GapRight method");
    }

    public int GapBottom( int index )
    {
      if( index >= 0 && index < Variables.Count )
        return Variables[ index ].GapBottom;
      else
        throw new IndexOutOfRangeException("Bad index for GapBottom method");
    }

    public int GapLeft( int index )
    {
      if( index >= 0 && index < Variables.Count )
        return Variables[ index ].GapLeft;
      else
        throw new IndexOutOfRangeException("Bad index for GapLeft method");
    }

    public int DistanceTo( int source, int target )
    {
      return DistanceTo( source, LineToMatrix( target ) );
    }

    public int DistanceToStarting( int source )
    {
      return DistanceTo( source, StartingTile );
    }

    public int DistanceToTarget( int source )
    {
      return DistanceTo( source, TargetTile );
    }

    public int DistanceTo( int source, Point targetPoint )
    {
      Point sourcePoint = LineToMatrix( source );
      if( sourcePoint == null )
        throw new ArgumentException("Cannot compute distance if source is null", "sourcePoint");
      if( targetPoint == null )
        throw new ArgumentException("Cannot compute distance if target is null", "targetPoint");

      return Math.Abs( targetPoint.VerticalPosition - sourcePoint.VerticalPosition )
        + Math.Abs( targetPoint.HorizontalPosition - sourcePoint.HorizontalPosition );
    }

    public double RealDistanceSTTo( int source )
    {
      Point sourcePoint = LineToMatrix( source );
      if( sourcePoint == null )
        throw new ArgumentException("Cannot compute distance if source is null", "sourcePoint");

      double distanceToStarting = Math.Sqrt( Math.Pow( StartingTile.VerticalPosition - sourcePoint.VerticalPosition, 2 ) 
                                            + Math.Pow( StartingTile.HorizontalPosition - sourcePoint.HorizontalPosition, 2 ) );
      double distanceToTarget = Math.Sqrt( Math.Pow( TargetTile.VerticalPosition - sourcePoint.VerticalPosition, 2 ) 
                                          + Math.Pow( TargetTile.HorizontalPosition - sourcePoint.HorizontalPosition, 2 ) );
      return distanceToStarting + distanceToTarget;
    }

    public void Unbuildable( int row, int col )
    {
      if( 0 <= row && row < Row && 0 <= col && col < Column )
        _matrixType[row , col].Add( "###" );

      for( int i = 0 ; i < Variables.Count ; ++i )
      {
        var positionWithoutOutsideScope = PossiblePositions( i );
        positionWithoutOutsideScope.Remove( -1 );
        Variables[ i ].Domain = new ghost.Domain( positionWithoutOutsideScope, -1 );
      }
    }

    public void Unbuildable( List< Point > unbuildables )
    {
      unbuildables.ForEach( x => Unbuildable( x.VerticalPosition, x.HorizontalPosition ) );
    }

    public List<int> BuildingsAt( int row, int col )
    {
      if( 0 <= row && row < Row && 0 <= col && col < Column )
        return _matrixIndex[row , col];
      else
        return null;
    }

    public List<int> BuildingsAt( Point p )
    {
      return p == null ? null : BuildingsAt( p.VerticalPosition, p.HorizontalPosition );
    }

    public List<int> BuildingsAt( int p )
    {
      return p < 0 ? null : BuildingsAt( LineToMatrix( p ) );
    }

    public Point LineToMatrix( int p )
    {
      return p < 0 ? null : new Point( p / Column, p % Column );
    }

    public int MatrixToLine( int row, int col )
    {
      return row * Column + col;
    }

    public int MatrixToLine( Point p )
    {
      return p.VerticalPosition * Column + p.HorizontalPosition;
    }

    public override void ShiftValue( int index ) 
    {
      if( index >= 0 && index < Variables.Count )
      {
        ClearInMatrices( index );
        Variables[ index ].ShiftValue();
        AddInMatrices( index );
      }
    }

    public override void UnshiftValue( int index )
    {
      if( index >= 0 && index < Variables.Count )
      {
        ClearInMatrices( index );
        Variables[ index ].UnshiftValue();
        AddInMatrices( index );
      }
    }

    public void Shift( int index, out int overlaps, out int unbuildables )
    {
      overlaps = 0;
      unbuildables = 0;

      if( index >= 0 && index < Variables.Count && Variables[ index ].IsSelected() )
      {
        Point pos = LineToMatrix( Variables[ index ].GetValue() );
        int row = pos.VerticalPosition;
        int col = pos.HorizontalPosition;

        int rowShift = row + Variables[ index ].Height;
        int colShift = col + Variables[ index ].Length;

        string key;

        for( int x = row ; x < rowShift ; ++x )
        {
          AddInMatrices( x, colShift, Variables[ index ].Name, index ); 

          key = "" + x + "," + colShift;
          if( Failures.ContainsKey( key ) )
          {
            if( !Failures[ key ].Contains( "###" ) )
              ++overlaps;
            else
              ++unbuildables;
          }

          key = "" + x + "," + col;
          if( Failures.ContainsKey( key ) )
          {
            if( !Failures[ key ].Contains( "###" ) )
              --overlaps;
            else
              --unbuildables;
          }

          ClearInMatrices( x, col, Variables[ index ].Name, index );
        }

        Variables[ index ].ShiftValue();
      }
    }


    public void QuickShift( int index )
    {
      if( index >= 0 
         && index < Variables.Count
         && Variables[ index ].IsSelected() )
      {
        Point position = LineToMatrix( Variables[ index ].GetValue() );
        int row = position.VerticalPosition;
        int column = position.HorizontalPosition;

        int rowShift = row + Variables[ index ].Height;
        int columnShift = column + Variables[ index ].Length;

        for( int x = row ; x < rowShift ; ++x )
        {
          AddInMatrices( x, columnShift, Variables[ index ].Name, index ); 
          ClearInMatrices( x, column, Variables[ index ].Name, index );
        }

        Variables[ index ].ShiftValue();
      }
    }

    public bool FitAt( int index, int value )
    {
      if( value < 0 )
        return true;

      var p = LineToMatrix( value );
      var vertPos = Math.Max( 0, p.VerticalPosition );
      var horiPos = Math.Max( 0, p.HorizontalPosition );
      return vertPos + Variables[ index ].Height <= Row
             &&
             horiPos + Variables[ index ].Length <= Column;
    }

    public override void SetValue( int index, int value )
    {
      if( index >= 0 && index < Variables.Count && FitAt( index, value ) )
      {
        if( Variables[ index ].IsSelected() )
          ClearInMatrices( index );

        Variables[ index ].SetValue( value );

        if( value != Domain( index ).OutsideScope )
          AddInMatrices( index );
      }
    }

    private void AddInMatrices( int building )
    {
      //Console.WriteLine( "Call AddInMatrices({0}) with position {1}", building, Variables[ building ].GetValue() );
      if( Variables[ building ].IsSelected() )
      {
        Point pos = LineToMatrix( Variables[ building ].GetValue() );
        int row = pos.VerticalPosition;
        int col = pos.HorizontalPosition;

        for( int x = row; x < row + Variables[ building ].Height; ++x )
          for( int y = col; y < col + Variables[ building ].Length; ++y )
            AddInMatrices(x, y, Variables[ building ].Name, building );
      }
    }

    private void AddInMatrices( int row, int col, string buildingShort, int buildingIndex )
    {
      var temp = _matrixType[ row, col ].FirstOrDefault( s => s.Contains( "@" ) );

      bool fail = ! ( _matrixType[ row, col ].Count == 0 
                     || ( temp != null && temp.Length <= 3) );

      _matrixType[ row, col ].Add( buildingShort );
      _matrixIndex[ row, col ].Add( buildingIndex );
      if( fail )
      {
        var key = "" + row + "," + col;

        if( !Failures.ContainsKey( key ) )
          Failures.Add( key, string.Join( "", _matrixType[ row, col ].ToArray() ) );
        else
          Failures[ key ] += buildingShort;
      }
    }

    private void ClearInMatrices( int building )
    {
      //Console.WriteLine( "Call ClearInMatrices({0}) with position {1}", building, Variables[ building ].GetValue() );
      if( Variables[ building ].IsSelected() )
      {
        Point pos = LineToMatrix( Variables[ building ].GetValue() );
        int row = pos.VerticalPosition;
        int col = pos.HorizontalPosition;

        for( int x = row; x < row + Variables[ building ].Height; ++x )
          for( int y = col; y < col + Variables[ building ].Length; ++y )
            ClearInMatrices(x, y, Variables[ building ].Name, building );
      }
    }

    private void ClearInMatrices( int row, int col, string buildingShort, int buildingIndex )
    {
      if( _matrixIndex[row , col].Contains( buildingIndex ) )
      {
        _matrixType[row , col].Remove( buildingShort );
        _matrixIndex[row , col].Remove( buildingIndex );

        var key = "" + row + "," + col;

        if( Failures.ContainsKey( key ) )
        {
          if( _matrixType[ row, col ].Count < 2
              || _matrixType[ row, col ].Contains( "###" )
              || ( _matrixType[ row, col ].Count == 1 && _matrixType[ row, col ][ 0 ].Contains( "@" ) ) )
            Failures.Remove( key );
          else
            Failures[ key ] = string.Join( "", _matrixType[ row, col ].ToArray() );
        }
      }
    }

    public List< int > GetBuildingsAround ( int index )
    {
      var allIndexesList = Enumerable.Range( 0, Variables.Count ).ToList();
      return GetBuildingsAround( index, allIndexesList );
    }

    public List< int > GetBuildingsAround ( int index, List< int > indexVariablesList )
    {
      var myNeighbors = new List<int>();

      if( Variables[ index ].IsSelected() )
      {
        Point coordinates = LineToMatrix( Variables[ index ].GetValue() );

        int top = coordinates.VerticalPosition;
        int right = coordinates.HorizontalPosition + Variables[ index ].Length - 1;
        int bottom = coordinates.VerticalPosition + Variables[ index ].Height - 1;
        int left = coordinates.HorizontalPosition;

        foreach( var other in indexVariablesList )
        {
          if( other != index && Variables[ other ].IsSelected() )
          {
            Point xyOther = LineToMatrix( Variables[ other ].GetValue() );
            int otherTop = xyOther.VerticalPosition;
            int otherRight = xyOther.HorizontalPosition + Variables[ other ].Length - 1;
            int otherBottom = xyOther.VerticalPosition + Variables[ other ].Height - 1;
            int otherLeft = xyOther.HorizontalPosition;

            if(  ( top == otherBottom + 1 && ( otherRight >= left && otherLeft <= right ) )
            || ( right == otherLeft - 1 && ( otherBottom >= top - 1 && otherTop <= bottom + 1 ) )
            || ( bottom == otherTop - 1 && ( otherRight >= left && otherLeft <= right ) )
            || ( left == otherRight + 1 && ( otherBottom >= top - 1 && otherTop <= bottom + 1 ) ) )
            {
              myNeighbors.Add( other );
            }
          }
        }
      }
      return myNeighbors;
    }

    public List< int > GetBuildingsAbove( int index )
    {
      var myNeighbors = new List<int>();

      if( Variables[ index ].IsSelected() )
      {
        Point coordinates = LineToMatrix( Variables[ index ].GetValue() );

        int top = coordinates.VerticalPosition;
        int right = coordinates.HorizontalPosition + Variables[ index ].Length - 1;
        int left = coordinates.HorizontalPosition;

        for( int i = 0 ; i < Variables.Count ; ++i )
        {
          if( i != index && Variables[ i ].IsSelected() )
          {
            Point xyOther = LineToMatrix( Variables[ i ].GetValue() );
            int otherRight = xyOther.HorizontalPosition + Variables[ i ].Length - 1;
            int otherBottom = xyOther.VerticalPosition + Variables[ i ].Height - 1;
            int otherLeft = xyOther.HorizontalPosition;

            if( top == otherBottom + 1 && otherRight >= left && otherLeft <= right )
              myNeighbors.Add( i );
          }
        }
      }
      return myNeighbors;
    }

    public List< int > GetBuildingsOnRight( int index )
    {
      var myNeighbors = new List<int>();

      if( Variables[ index ].IsSelected() )
      {
        Point coordinates = LineToMatrix( Variables[ index ].GetValue() );

        int top = coordinates.VerticalPosition;
        int right = coordinates.HorizontalPosition + Variables[ index ].Length - 1;
        int bottom = coordinates.VerticalPosition + Variables[ index ].Height - 1;

        for( int i = 0 ; i < Variables.Count ; ++i )
        {
          if( i != index && Variables[ i ].IsSelected() )
          {
            Point xyOther = LineToMatrix( Variables[ i ].GetValue() );
            int otherTop = xyOther.VerticalPosition;
            int otherBottom = xyOther.VerticalPosition + Variables[ i ].Height - 1;
            int otherLeft = xyOther.HorizontalPosition;

            if( right == otherLeft - 1 && otherBottom >= top - 1 && otherTop <= bottom + 1 )
              myNeighbors.Add( i );
          }
        }
      }
      return myNeighbors;
    }

    public List< int > GetBuildingsBelow( int index )
    {
      var myNeighbors = new List<int>();

      if( Variables[ index ].IsSelected() )
      {
        Point coordinates = LineToMatrix( Variables[ index ].GetValue() );

        int right = coordinates.HorizontalPosition + Variables[ index ].Length - 1;
        int bottom = coordinates.VerticalPosition + Variables[ index ].Height - 1;
        int left = coordinates.HorizontalPosition;

        for( int i = 0 ; i < Variables.Count ; ++i )
        {
          if( i != index && Variables[ i ].IsSelected() )
          {
            Point xyOther = LineToMatrix( Variables[ i ].GetValue() );
            int otherTop = xyOther.VerticalPosition;
            int otherRight = xyOther.HorizontalPosition + Variables[ i ].Length - 1;
            int otherLeft = xyOther.HorizontalPosition;

            if( bottom == otherTop - 1 && otherRight >= left && otherLeft <= right )
              myNeighbors.Add( i );
          }
        }
      }
      return myNeighbors;
    }

    public List< int > GetBuildingsOnLeft( int index )
    {
      var myNeighbors = new List<int>();

      if( Variables[ index ].IsSelected() )
      {
        Point coordinates = LineToMatrix( Variables[ index ].GetValue() );

        int top = coordinates.VerticalPosition;
        int bottom = coordinates.VerticalPosition + Variables[ index ].Height - 1;
        int left = coordinates.HorizontalPosition;

        for( int i = 0 ; i < Variables.Count ; ++i )
        {
          if( i != index && Variables[ i ].IsSelected() )
         {
            Point xyOther = LineToMatrix( Variables[ i ].GetValue() );
            int otherTop = xyOther.VerticalPosition;
            int otherRight = xyOther.HorizontalPosition + Variables[ i ].Length - 1;
            int otherBottom = xyOther.VerticalPosition + Variables[ i ].Height - 1;

            if( left == otherRight + 1 && otherBottom >= top - 1 && otherTop <= bottom + 1 )
              myNeighbors.Add( i );
          }
        }
      }
      return myNeighbors;
    }

    public bool IsOnStartingOrTargetTile( int index )
    {
      var startingBuildings = BuildingsAt( StartingTile );
      var targetBuildings = BuildingsAt( TargetTile );

      return startingBuildings.Contains( index ) || targetBuildings.Contains( index );
    }

    public int CountAround( int index )
    {
      return Variables[ index ].IsSelected() ? GetBuildingsAround( index ).Count : 0;
    }

    private List<int> PossiblePositions( int index )
    {
      var possiblePositions = new List<int>();

      possiblePositions.Add( -1 );

      for( int row = 0; row <= Row - Variables[ index ].Height; ++row )
        for( int col = 0; col <= Column - Variables[ index ].Length; ++col )
          if( !_matrixType[row , col].Contains( "###" ) && !_matrixType[ row , col + Variables[ index ].Length - 1 ].Contains( "###" ) )
            possiblePositions.Add( MatrixToLine( row, col ) );
  
      return possiblePositions;
    }
      
//#if DEBUG
    public override void Print()
    {
      Console.WriteLine( "Row: {0}, Column: {1}\nMatrix Index:", Row, Column );
      string bar = "";

      for( int i = 0 ; i < Column ; ++i )
        bar += "---------";

      for( int i = 0 ; i < Row ; ++i )
      {
        Console.WriteLine( "|{0}| ", bar );
        Console.Write("|");
        for( int j = 0 ; j < Column ; ++j )
          if( _matrixIndex[ i, j ].Count == 0 )
            Console.Write( "        |" );
          else
          {
            string buildingsString = "";
            foreach( var elem in _matrixIndex[i , j] )
              buildingsString += elem + " ";

            Console.Write( String.Format( " {0,6} |", buildingsString ) );
          }
        Console.WriteLine();
      }
      Console.WriteLine( "|{0}|", bar );
      Console.WriteLine( "Matrix Type:" );
      for( int i = 0 ; i < Row ; ++i )
      {
        Console.WriteLine( "|{0}| ", bar );
        Console.Write("|");
        for( int j = 0 ; j < Column ; ++j )
          if( _matrixType[ i, j ].Count == 0 )
            Console.Write( "        |" );
          else
          {
            string buildingsString = "";
            foreach( var elem in _matrixType[i , j] )
            {
              buildingsString += elem;
              if( !elem.Contains( '#' ) && !elem.Contains( '@' ) )
                buildingsString += " ";
            }
            Console.Write( String.Format( " {0,6} |", buildingsString ) );
          }
        Console.WriteLine();
      }
      Console.WriteLine( "|{0}|", bar );
      Console.Write( "Failures: " );
      string fails = "";
      foreach( var f in Failures )
        fails += "[" + f.Key + "]:" + f.Value + ", ";
      Console.WriteLine( fails );
    }
//#endif

    public int Column { get; private set; }
    public int Row { get; private set; }
    public Point StartingTile { get; private set; }
    public Point TargetTile { get; private set; }
    public Dictionary<string, string> Failures { get; private set; }
  }
}
