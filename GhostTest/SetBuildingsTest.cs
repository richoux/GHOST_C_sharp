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
using ghost;
using System;

namespace GhostTest
{
  [TestFixture]
  public class SetBuildingsTest
  {
    public static Building academy = new Building( "A", "Terran_Academy", 3, 2, 0, 3, 7, 8, Race.Terran, 2 );
    public static Building barracks = new Building( "B", "Terran_Barracks", 4, 3, 8, 7, 15, 16, Race.Terran, 1 );

    public static List<Building> list = new List<Building> { new Building( "X", "xxx", 1, 1, 0, 0, 0, 0, Race.Unknown, 0, 0 ), 
                                                             (Building)academy.Clone(), 
                                                             (Building)barracks.Clone(), 
                                                             (Building)academy.Clone() };
    public static SetBuildings setBuildings = new SetBuildings( list, 3, 4, new SetBuildings.Point( 2, 3 ), new SetBuildings.Point( 0, 0 ) );

    [Test]
    public void IsSelectedTest()
    {
      setBuildings.SetValue( 1, 0 );
      Assert.IsFalse( setBuildings.IsSelected( 0 ) );
      Assert.IsTrue( setBuildings.IsSelected( 1 ) );
      Assert.IsFalse( setBuildings.IsSelected( 2 ) );
      setBuildings.SetValue( 1, -1 );
    }

    [TestCase( -1 )]
    [TestCase( 1024 )]
    [ExpectedException(typeof(IndexOutOfRangeException))]
    public void IsSelectedFailTest( int index )
    {
      setBuildings.IsSelected( index );
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
      SetBuildings.Point point = setBuildings.LineToMatrix( p );
      Assert.AreEqual( c, point.HorizontalPosition );
      Assert.AreEqual( r, point.VerticalPosition );
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
      int val = setBuildings.MatrixToLine( new SetBuildings.Point( r, c ) );
      Assert.AreEqual( p, val );
    }

    [TestCase( 0, 0 )]
    [TestCase( 2, 2 )]
    [TestCase( 4, 1 )]
    [TestCase( 6, 3 )]
    public void DistanceToTargetTest( int source, int distance )
    {
      Assert.AreEqual( distance, setBuildings.DistanceToTarget( source ) );
    }

    [TestCase( -1, 0 )]
    [TestCase( 0, -1 )]
    [ExpectedException(typeof(ArgumentException))]
    public void DistanceToFailTest( int source, int target )
    {
      setBuildings.DistanceTo( source, target );
    }
      
    [Test]
    public void UnbuildableTest()
    {
      var set = new SetBuildings( list, 10, 10, new SetBuildings.Point( 5, 5 ), new SetBuildings.Point( 0, 0 ) );
      set.Unbuildable( 4, 4 );

      for( int i = 0 ; i < 10 ; ++i )
        for( int j = 0 ; j < 10 ; ++j )
          if( i == 4 && j == 4 )
            Assert.AreEqual( -1, set.Domain( 0 ).IndexOf( i * 10 + j ) );
          else
            Assert.AreNotEqual( -1, set.Domain( 0 ).IndexOf( i * 10 + j ) );

      var listPoint = new List<SetBuildings.Point> { new SetBuildings.Point( 3, 9 ), new SetBuildings.Point( 8, 7 ), new SetBuildings.Point( 1, 1 )};
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
            Assert.AreEqual( -1, set.Domain( 0 ).IndexOf( i * 10 + j ) );
          else
            Assert.AreNotEqual( -1, set.Domain( 0 ).IndexOf( i * 10 + j ) );
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
      var set = new SetBuildings( list, 5, 10, new SetBuildings.Point( 4, 5 ), new SetBuildings.Point( 0, 0 ) );
      Assert.IsTrue( set.FitAt( 1, pos ) );
    }

    [TestCase(38)]
    [TestCase(42)]
    public void DontFitAtTest( int pos )
    {
      var set = new SetBuildings( list, 5, 10, new SetBuildings.Point( 4, 5 ), new SetBuildings.Point( 0, 0 ) );
      Assert.IsFalse( set.FitAt( 1, pos ) );
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
      var set = new SetBuildings( list, 5, 10, new SetBuildings.Point( 4, 5 ), new SetBuildings.Point( 0, 0 ) );
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
            Assert.AreEqual( 1, l.Count );
            Assert.AreEqual( 1, l[ 0 ] );
          }
          else
          {
            var l = set.BuildingsAt( i, j );
            Assert.AreEqual( 0, l.Count );
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
            Assert.AreEqual( 1, l.Count );
            Assert.AreEqual( 1, l[ 0 ] );
          }
          else
          {
            var l = set.BuildingsAt( i, j );
            Assert.AreEqual( 0, l.Count );
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
            Assert.AreEqual( 1, l.Count );
            Assert.AreEqual( 1, l[ 0 ] );
          }
          else
          {
            var l = set.BuildingsAt( i, j );
            Assert.AreEqual( 0, l.Count );
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
            Assert.AreEqual( 1, l.Count );
            Assert.AreEqual( 1, l[ 0 ] );
          }
          else
          {
            var l = set.BuildingsAt( i, j );
            Assert.AreEqual( 0, l.Count );
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
            Assert.AreEqual( 1, l.Count );
            Assert.AreEqual( 1, l[ 0 ] );
          }
          else
          {
            var l = set.BuildingsAt( i, j );
            Assert.AreEqual( 0, l.Count );
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
            Assert.AreEqual( 1, l.Count );
            Assert.AreEqual( 1, l[ 0 ] );
          }
          else
          {
            var l = set.BuildingsAt( i, j );
            Assert.AreEqual( 0, l.Count );
          }

      set.ShiftValue( 1 );
      set.SetValue( 2, 22 );
      set.SetValue( 3, 37 );

      int overlaps;
      int unbuild;

      set.Shift( 2, out overlaps, out unbuild );
      Assert.AreEqual( -2, overlaps );
      Assert.AreEqual( 0, unbuild );

      Assert.IsTrue( set.GetBuildingsAround( 2 ).Contains( 1 ) );
      Assert.IsTrue( set.GetBuildingsAround( 2 ).Contains( 3 ) );
      Assert.IsTrue( set.GetBuildingsAround( 1 ).Contains( 2 ) );
      Assert.IsTrue( set.GetBuildingsAround( 3 ).Contains( 2 ) );
      Assert.AreEqual( 2, set.GetBuildingsAround( 2 ).Count );
      Assert.AreEqual( 1, set.GetBuildingsAround( 3 ).Count );
      Assert.AreEqual( 1, set.GetBuildingsAround( 1 ).Count );

      set.Shift( 2, out overlaps, out unbuild );
      Assert.AreEqual( 2, overlaps );
      Assert.AreEqual( 0, unbuild );

      var listSmallBuildings = new List<Building> { new Building( "X", "xxx", 1, 1, 0, 0, 0, 0, Race.Unknown, 0, 0 ), 
        new Building( "X", "xxx", 1, 1, 0, 0, 0, 0, Race.Unknown, 0, 0 ),
        new Building( "X", "xxx", 1, 1, 0, 0, 0, 0, Race.Unknown, 0, 0 ),
        new Building( "X", "xxx", 1, 1, 0, 0, 0, 0, Race.Unknown, 0, 0 ),
        new Building( "X", "xxx", 1, 1, 0, 0, 0, 0, Race.Unknown, 0, 0 ) };

      set = new SetBuildings( listSmallBuildings, 5, 10, new SetBuildings.Point( 4, 5 ), new SetBuildings.Point( 0, 0 ) );
      set.SetValue( 0, 25 );
      set.SetValue( 1, 26 );
      set.SetValue( 2, 35 );
      set.SetValue( 3, 24 );
      set.SetValue( 4, 15 );

      Assert.IsTrue( set.GetBuildingsOnRight( 0 ).Contains( 1 ) );
      Assert.IsTrue( set.GetBuildingsBelow( 0 ).Contains( 2 ) );
      Assert.IsTrue( set.GetBuildingsOnLeft( 0 ).Contains( 3 ) );
      Assert.IsTrue( set.GetBuildingsAbove( 0 ).Contains( 4 ) );
      Assert.AreEqual( 1, set.GetBuildingsOnRight( 0 ).Count );
      Assert.AreEqual( 1, set.GetBuildingsBelow( 0 ).Count );
      Assert.AreEqual( 1, set.GetBuildingsOnLeft( 0 ).Count );
      Assert.AreEqual( 1, set.GetBuildingsAbove( 0 ).Count );
      Assert.AreEqual( 4, set.CountAround( 0 ) );

      Assert.IsTrue( set.GetBuildingsOnLeft( 1 ).Contains( 0 ) );
      Assert.IsTrue( set.GetBuildingsOnLeft( 1 ).Contains( 2 ) );
      Assert.IsTrue( set.GetBuildingsOnLeft( 1 ).Contains( 4 ) );
      Assert.AreEqual( 0, set.GetBuildingsOnRight( 1 ).Count );
      Assert.AreEqual( 0, set.GetBuildingsBelow( 1 ).Count );
      Assert.AreEqual( 3, set.GetBuildingsOnLeft( 1 ).Count );
      Assert.AreEqual( 0, set.GetBuildingsAbove( 1 ).Count );
      Assert.AreEqual( 3, set.CountAround( 1 ) );

      Assert.IsTrue( set.GetBuildingsOnRight( 2 ).Contains( 1 ) );
      Assert.IsTrue( set.GetBuildingsOnLeft( 2 ).Contains( 3 ) );
      Assert.IsTrue( set.GetBuildingsAbove( 2 ).Contains( 0 ) );
      Assert.AreEqual( 1, set.GetBuildingsOnRight( 2 ).Count );
      Assert.AreEqual( 0, set.GetBuildingsBelow( 2 ).Count );
      Assert.AreEqual( 1, set.GetBuildingsOnLeft( 2 ).Count );
      Assert.AreEqual( 1, set.GetBuildingsAbove( 2 ).Count );
      Assert.AreEqual( 3, set.CountAround( 2 ) );

      Assert.IsTrue( set.GetBuildingsOnRight( 3 ).Contains( 0 ) );
      Assert.IsTrue( set.GetBuildingsOnRight( 3 ).Contains( 2 ) );
      Assert.IsTrue( set.GetBuildingsOnRight( 3 ).Contains( 4 ) );
      Assert.AreEqual( 3, set.GetBuildingsOnRight( 3 ).Count );
      Assert.AreEqual( 0, set.GetBuildingsBelow( 3 ).Count );
      Assert.AreEqual( 0, set.GetBuildingsOnLeft( 3 ).Count );
      Assert.AreEqual( 0, set.GetBuildingsAbove( 3 ).Count );
      Assert.AreEqual( 3, set.CountAround( 3 ) );

      Assert.IsTrue( set.GetBuildingsOnRight( 4 ).Contains( 1 ) );
      Assert.IsTrue( set.GetBuildingsBelow( 4 ).Contains( 0 ) );
      Assert.IsTrue( set.GetBuildingsOnLeft( 4 ).Contains( 3 ) );
      Assert.AreEqual( 1, set.GetBuildingsOnRight( 4 ).Count );
      Assert.AreEqual( 1, set.GetBuildingsBelow( 4 ).Count );
      Assert.AreEqual( 1, set.GetBuildingsOnLeft( 4 ).Count );
      Assert.AreEqual( 0, set.GetBuildingsAbove( 4 ).Count );
      Assert.AreEqual( 3, set.CountAround( 4 ) );

      Assert.IsFalse( set.IsOnStartingOrTargetTile( 0 ) );
      set.SetValue( 0, 0 );
      Assert.IsTrue( set.IsOnStartingOrTargetTile( 0 ) );
      set.SetValue( 0, 1 );
      Assert.IsFalse( set.IsOnStartingOrTargetTile( 0 ) );
      set.SetValue( 0, 45 );
      Assert.IsTrue( set.IsOnStartingOrTargetTile( 0 ) );

//      set.SetValue( 0, 25 );
//      Assert.IsFalse( set.IsNeighborOfSTTBuildings( 0 ) );
//      set.SetValue( 0, 11 );
//      Assert.IsTrue( set.IsNeighborOfSTTBuildings( 0 ) );
//      set.SetValue( 0, 44 );
//      Assert.IsTrue( set.IsNeighborOfSTTBuildings( 0 ) );
    }
  }
}

