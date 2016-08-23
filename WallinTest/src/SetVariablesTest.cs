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

namespace WallinTest
{
  [TestFixture]
  public class SetVariablesTest
  {
    public static Variable academy = new Variable( "A", "Terran_Academy", 3, 2, 0, 3, 7, 8, Race.Terran, 2 );
    public static Variable barracks = new Variable( "B", "Terran_Barracks", 4, 3, 8, 7, 15, 16, Race.Terran, 1 );

    public static List<Variable> list = new List<Variable> { new Variable( "X", "xxx", 1, 1, 0, 0, 0, 0, Race.Unknown, 0, 0 ), 
                                                             (Variable)academy.Clone(), 
                                                             (Variable)barracks.Clone(), 
                                                             (Variable)academy.Clone() };
    public static SetVariables setBuildings = new SetVariables( list, 3, 4, new SetVariables.Point( 2, 3 ), new SetVariables.Point( 0, 0 ) );

    [Test]
    public void IsSelectedTest()
    {
      setBuildings.SetValue( 1, 0 );
      Assert.That( setBuildings.IsSelected( 0 ), Is.False );
      Assert.That( setBuildings.IsSelected( 1 ), Is.True );
      Assert.That( setBuildings.IsSelected( 2 ), Is.False );
      setBuildings.SetValue( 1, -1 );
    }

    [TestCase( -1 )]
    [TestCase( 1024 )]
    public void IsSelectedFailTest( int index )
    {
      Assert.Throws<IndexOutOfRangeException>(()=>setBuildings.IsSelected( index ));
    }

    [TestCase( 0, 0, 0 )]
    [TestCase( 1, 0, 1 )]
    [TestCase( 3, 0, 3 )]
    [TestCase( 4, 1, 0 )]
    [TestCase( 5, 1, 1 )]
    [TestCase( 9, 2, 1 )]
    [TestCase( 11, 2, 3 )]
    public void LineToMatrixTest( int p, int r, int c )
    {
      SetVariables.Point point = setBuildings.LineToMatrix( p );
      Assert.That( point.HorizontalPosition, Is.EqualTo( c ) );
      Assert.That( point.VerticalPosition, Is.EqualTo( r ) );
    }

    [TestCase( 0, 0, 0 )]
    [TestCase( 1, 0, 1 )]
    [TestCase( 3, 0, 3 )]
    [TestCase( 4, 1, 0 )]
    [TestCase( 5, 1, 1 )]
    [TestCase( 9, 2, 1 )]
    [TestCase( 11, 2, 3 )]
    public void MatrixToLineTest( int p, int r, int c )
    {
      int val = setBuildings.MatrixToLine( new SetVariables.Point( r, c ) );
      Assert.That( val, Is.EqualTo( p ) );
    }

    [TestCase( 0, 0 )]
    [TestCase( 2, 2 )]
    [TestCase( 4, 1 )]
    [TestCase( 6, 3 )]
    public void DistanceToTargetTest( int source, int distance )
    {
      Assert.That( setBuildings.DistanceToTarget( source ), Is.EqualTo( distance ) );
    }

    [TestCase( -1, 0 )]
    [TestCase( 0, -1 )]
    public void DistanceToFailTest( int source, int target )
    {
      Assert.Throws<ArgumentException>(()=>setBuildings.DistanceTo( source, target ));
    }
      
    [Test]
    public void UnbuildableTest()
    {
      var set = new SetVariables( list, 10, 10, new SetVariables.Point( 5, 5 ), new SetVariables.Point( 0, 0 ) );
      set.Unbuildable( 4, 4 );

      for( int i = 0 ; i < 10 ; ++i )
        for( int j = 0 ; j < 10 ; ++j )
          if( i == 4 && j == 4 )
            Assert.That( set.Domain( 0 ).IndexOf( i * 10 + j ), Is.EqualTo( -1 ) );
          else
            Assert.That( set.Domain( 0 ).IndexOf( i * 10 + j ), Is.Not.EqualTo( -1 ) );

      var listPoint = new List<SetVariables.Point> { new SetVariables.Point( 3, 9 ), new SetVariables.Point( 8, 7 ), new SetVariables.Point( 1, 1 )};
      set.Unbuildable( listPoint );
      for( int i = 0 ; i < 10 ; ++i )
        for( int j = 0 ; j < 10 ; ++j )
          if( ( i == 4 && j == 4 )
             ||
             ( i == 3 && j == 9 )
             ||
             ( i == 8 && j == 7 )
             ||
             ( i == 1 && j == 1 ) )
            Assert.That( set.Domain( 0 ).IndexOf( i * 10 + j ), Is.EqualTo( -1 ) );
          else
            Assert.That( set.Domain( 0 ).IndexOf( i * 10 + j ), Is.Not.EqualTo( -1 ) );
#if DEBUG
      set.Print();
#endif
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(14)]
    [TestCase(25)]
    public void FitAtTest( int pos )
    {
      var set = new SetVariables( list, 5, 10, new SetVariables.Point( 4, 5 ), new SetVariables.Point( 0, 0 ) );
      Assert.That( set.FitAt( 1, pos ), Is.True );
    }

    [TestCase(38)]
    [TestCase(42)]
    public void DontFitAtTest( int pos )
    {
      var set = new SetVariables( list, 5, 10, new SetVariables.Point( 4, 5 ), new SetVariables.Point( 0, 0 ) );
      Assert.That( set.FitAt( 1, pos ), Is.False );
    }
      
    // Is tested by the following:
    // * SetValue
    // * PossiblePositions
    // * AddInMatrices (both)
    // * ClearInMatrices (both)
    // * ShiftValue
    // * UnshiftValue
    // * Shift (QuickShift is similar)
    // * all GetAround
    // * IsOnStartingOrTargetTile
    // * CountAround
    [Test]
    public void MatrixIndexTests()
    {
      var set = new SetVariables( list, 5, 10, new SetVariables.Point( 4, 5 ), new SetVariables.Point( 0, 0 ) );
      set.SetValue( 1, 14 );

      for( int i = 0 ; i < 5 ; ++i )
        for( int j = 0 ; j < 10 ; ++j )
          if( ( i == 1 && j == 4 )
              ||
              ( i == 1 && j == 5 )
              ||
              ( i == 1 && j == 6 )
              ||
              ( i == 2 && j == 4 )
              ||
              ( i == 2 && j == 5 )
              ||
              ( i == 2 && j == 6 ) )
          {
            var l = set.BuildingsAt( i, j );
            Assert.That( l.Count, Is.EqualTo( 1 ) );
            Assert.That( l[ 0 ], Is.EqualTo( 1 ) );
          }
          else
          {
            var l = set.BuildingsAt( i, j );
            Assert.That( l.Count, Is.Zero );
          }

      set.ShiftValue( 1 );

      for( int i = 0 ; i < 5 ; ++i )
        for( int j = 0 ; j < 10 ; ++j )
          if( ( i == 1 && j == 7 )
             ||
             ( i == 1 && j == 5 )
             ||
             ( i == 1 && j == 6 )
             ||
             ( i == 2 && j == 7 )
             ||
             ( i == 2 && j == 5 )
             ||
             ( i == 2 && j == 6 ) )
          {
            var l = set.BuildingsAt( i, j );
            Assert.That( l.Count, Is.EqualTo( 1 ) );
            Assert.That( l[ 0 ], Is.EqualTo( 1 ) );
          }
          else
          {
            var l = set.BuildingsAt( i, j );
            Assert.That( l.Count, Is.Zero );
          }

      set.UnshiftValue( 1 );

      for( int i = 0 ; i < 5 ; ++i )
        for( int j = 0 ; j < 10 ; ++j )
          if( ( i == 1 && j == 4 )
             ||
             ( i == 1 && j == 5 )
             ||
             ( i == 1 && j == 6 )
             ||
             ( i == 2 && j == 4 )
             ||
             ( i == 2 && j == 5 )
             ||
             ( i == 2 && j == 6 ) )
          {
            var l = set.BuildingsAt( i, j );
            Assert.That( l.Count, Is.EqualTo( 1 ) );
            Assert.That( l[ 0 ], Is.EqualTo( 1 ) );
          }
          else
          {
            var l = set.BuildingsAt( i, j );
            Assert.That( l.Count, Is.Zero );
          }

      set.SetValue( 1, 27 );

      for( int i = 0 ; i < 5 ; ++i )
        for( int j = 0 ; j < 10 ; ++j )
          if( ( i == 3 && j == 7 )
             ||
             ( i == 3 && j == 8 )
             ||
             ( i == 3 && j == 9 )
             ||
             ( i == 2 && j == 7 )
             ||
             ( i == 2 && j == 8 )
             ||
             ( i == 2 && j == 9 ) )
          {
            var l = set.BuildingsAt( i, j );
            Assert.That( l.Count, Is.EqualTo( 1 ) );
            Assert.That( l[ 0 ], Is.EqualTo( 1 ));
          }
          else
          {
            var l = set.BuildingsAt( i, j );
            Assert.That( l.Count, Is.Zero );
          }

      set.ShiftValue( 1 );

      for( int i = 0 ; i < 5 ; ++i )
        for( int j = 0 ; j < 10 ; ++j )
          if( ( i == 3 && j == 0 )
             ||
             ( i == 3 && j == 1 )
             ||
             ( i == 3 && j == 2 )
             ||
             ( i == 4 && j == 0 )
             ||
             ( i == 4 && j == 1 )
             ||
             ( i == 4 && j == 2 ) )
          {
            var l = set.BuildingsAt( i, j );
            Assert.That( l.Count, Is.EqualTo( 1 ) );
            Assert.That( l[ 0 ], Is.EqualTo( 1 ) );
          }
          else
          {
            var l = set.BuildingsAt( i, j );
            Assert.That( l.Count, Is.Zero );
          }

      set.UnshiftValue( 1 );

      for( int i = 0 ; i < 5 ; ++i )
        for( int j = 0 ; j < 10 ; ++j )
          if( ( i == 3 && j == 7 )
             ||
             ( i == 3 && j == 8 )
             ||
             ( i == 3 && j == 9 )
             ||
             ( i == 2 && j == 7 )
             ||
             ( i == 2 && j == 8 )
             ||
             ( i == 2 && j == 9 ) )
          {
            var l = set.BuildingsAt( i, j );
            Assert.That( l.Count, Is.EqualTo( 1 ) );
            Assert.That( l[ 0 ], Is.EqualTo( 1 ) );
          }
          else
          {
            var l = set.BuildingsAt( i, j );
            Assert.That( l.Count, Is.Zero );
          }

      set.ShiftValue( 1 );
      set.SetValue( 2, 22 );
      set.SetValue( 3, 37 );

      int overlaps;
      int unbuild;

      set.Shift( 2, out overlaps, out unbuild );
      Assert.That( overlaps, Is.EqualTo( -2 ) );
      Assert.That( unbuild, Is.Zero );

      Assert.That( set.GetBuildingsAround( 2 ).Contains( 1 ), Is.True );
      Assert.That( set.GetBuildingsAround( 2 ).Contains( 3 ), Is.True );
      Assert.That( set.GetBuildingsAround( 1 ).Contains( 2 ), Is.True );
      Assert.That( set.GetBuildingsAround( 3 ).Contains( 2 ), Is.True );
      Assert.That( set.GetBuildingsAround( 2 ).Count, Is.EqualTo( 2 ) );
      Assert.That( set.GetBuildingsAround( 3 ).Count, Is.EqualTo( 1 ) );
      Assert.That( set.GetBuildingsAround( 1 ).Count, Is.EqualTo( 1 ) );

      set.Shift( 2, out overlaps, out unbuild );
      Assert.That( overlaps, Is.EqualTo( 2 ) );
      Assert.That( unbuild, Is.Zero );

      var listSmallBuildings = new List<Variable> { new Variable( "X", "xxx", 1, 1, 0, 0, 0, 0, Race.Unknown, 0, 0 ), 
        new Variable( "X", "xxx", 1, 1, 0, 0, 0, 0, Race.Unknown, 0, 0 ),
        new Variable( "X", "xxx", 1, 1, 0, 0, 0, 0, Race.Unknown, 0, 0 ),
        new Variable( "X", "xxx", 1, 1, 0, 0, 0, 0, Race.Unknown, 0, 0 ),
        new Variable( "X", "xxx", 1, 1, 0, 0, 0, 0, Race.Unknown, 0, 0 ) };

      set = new SetVariables( listSmallBuildings, 5, 10, new SetVariables.Point( 4, 5 ), new SetVariables.Point( 0, 0 ) );
      set.SetValue( 0, 25 );
      set.SetValue( 1, 26 );
      set.SetValue( 2, 35 );
      set.SetValue( 3, 24 );
      set.SetValue( 4, 15 );

      Assert.That( set.GetBuildingsOnRight( 0 ).Contains( 1 ), Is.True );
      Assert.That( set.GetBuildingsBelow( 0 ).Contains( 2 ), Is.True );
      Assert.That( set.GetBuildingsOnLeft( 0 ).Contains( 3 ), Is.True );
      Assert.That( set.GetBuildingsAbove( 0 ).Contains( 4 ), Is.True );
      Assert.That( set.GetBuildingsOnRight( 0 ).Count, Is.EqualTo( 1 ) );
      Assert.That( set.GetBuildingsBelow( 0 ).Count, Is.EqualTo( 1 ) );
      Assert.That( set.GetBuildingsOnLeft( 0 ).Count, Is.EqualTo( 1 ) );
      Assert.That( set.GetBuildingsAbove( 0 ).Count, Is.EqualTo( 1 ) );
      Assert.That( set.CountAround( 0 ), Is.EqualTo( 4 ) );

      Assert.That( set.GetBuildingsOnLeft( 1 ).Contains( 0 ), Is.True );
      Assert.That( set.GetBuildingsOnLeft( 1 ).Contains( 2 ), Is.True );
      Assert.That( set.GetBuildingsOnLeft( 1 ).Contains( 4 ), Is.True );
      Assert.That( set.GetBuildingsOnRight( 1 ).Count, Is.Zero );
      Assert.That( set.GetBuildingsBelow( 1 ).Count, Is.Zero );
      Assert.That( set.GetBuildingsOnLeft( 1 ).Count, Is.EqualTo( 3 ) );
      Assert.That( set.GetBuildingsAbove( 1 ).Count, Is.Zero );
      Assert.That( set.CountAround( 1 ), Is.EqualTo( 3 ) );

      Assert.That( set.GetBuildingsOnRight( 2 ).Contains( 1 ), Is.True );
      Assert.That( set.GetBuildingsOnLeft( 2 ).Contains( 3 ), Is.True );
      Assert.That( set.GetBuildingsAbove( 2 ).Contains( 0 ), Is.True );
      Assert.That( set.GetBuildingsOnRight( 2 ).Count, Is.EqualTo( 1 ) );
      Assert.That( set.GetBuildingsBelow( 2 ).Count, Is.Zero );
      Assert.That( set.GetBuildingsOnLeft( 2 ).Count, Is.EqualTo( 1 ) );
      Assert.That( set.GetBuildingsAbove( 2 ).Count, Is.EqualTo( 1 ) );
      Assert.That( set.CountAround( 2 ), Is.EqualTo( 3 ) );

      Assert.That( set.GetBuildingsOnRight( 3 ).Contains( 0 ), Is.True );
      Assert.That( set.GetBuildingsOnRight( 3 ).Contains( 2 ), Is.True );
      Assert.That( set.GetBuildingsOnRight( 3 ).Contains( 4 ), Is.True );
      Assert.That( set.GetBuildingsOnRight( 3 ).Count, Is.EqualTo( 3 ) );
      Assert.That( set.GetBuildingsBelow( 3 ).Count, Is.Zero );
      Assert.That( set.GetBuildingsOnLeft( 3 ).Count, Is.Zero );
      Assert.That( set.GetBuildingsAbove( 3 ).Count, Is.Zero );
      Assert.That( set.CountAround( 3 ), Is.EqualTo( 3 ) );

      Assert.That( set.GetBuildingsOnRight( 4 ).Contains( 1 ), Is.True );
      Assert.That( set.GetBuildingsBelow( 4 ).Contains( 0 ), Is.True );
      Assert.That( set.GetBuildingsOnLeft( 4 ).Contains( 3 ), Is.True );
      Assert.That( set.GetBuildingsOnRight( 4 ).Count, Is.EqualTo( 1 ) );
      Assert.That( set.GetBuildingsBelow( 4 ).Count, Is.EqualTo( 1 ) );
      Assert.That( set.GetBuildingsOnLeft( 4 ).Count, Is.EqualTo( 1 ) );
      Assert.That( set.GetBuildingsAbove( 4 ).Count, Is.Zero );
      Assert.That( set.CountAround( 4 ), Is.EqualTo( 3 ) );

      Assert.That( set.IsOnStartingOrTargetTile( 0 ), Is.False );
      set.SetValue( 0, 0 );
      Assert.That( set.IsOnStartingOrTargetTile( 0 ), Is.True );
      set.SetValue( 0, 1 );
      Assert.That( set.IsOnStartingOrTargetTile( 0 ), Is.False );
      set.SetValue( 0, 45 );
      Assert.That( set.IsOnStartingOrTargetTile( 0 ), Is.True );
    }
  }
}

