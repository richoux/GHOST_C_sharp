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

using NUnit.Framework;
using System;
using System.Collections.Generic;
using ghost;

namespace GhostTest
{
  public class DomainWrapper : Domain
  {
    public DomainWrapper( int outsideScope = -1 ) : base( outsideScope ) { }
    public DomainWrapper( List< int > domain, int outsideScope ) : base( domain, outsideScope ) { }
    public DomainWrapper( int size, int startValue ) : base( size, startValue ) { }

    public Random GetRandomObject()
    {
      return Random;
    }

    public List<int> GetCurrentDomain()
    {
      return CurrentDomain;
    }

    public List<int> GetInitialDomain()
    {
      return InitialDomain;
    }

    public void SetCurrentDomain( List<int> domain )
    {
      CurrentDomain = domain;
    }

    public void SetInitialDomain( List<int> domain )
    {
      InitialDomain = domain;
    }
  }

  [TestFixture]
  public class DomainTest
  {
    public static DomainWrapper domain = new DomainWrapper( 3, 2 );

    [Test]
    public void ConstructorsTest()
    {
      var d = new DomainWrapper();
      Assert.AreEqual( -1, d.OutsideScope );
      Assert.IsNotNull( d.GetRandomObject() );
      Assert.IsNull( d.GetCurrentDomain() );
      Assert.IsNull( d.GetInitialDomain() );

      d = new DomainWrapper( 51 );
      Assert.AreEqual( 51, d.OutsideScope );
      Assert.IsNotNull( d.GetRandomObject() );
      Assert.IsNull( d.GetCurrentDomain() );
      Assert.IsNull( d.GetInitialDomain() );

      var list = new List<int> { 1, 2, 3 };
      d = new DomainWrapper( list, 0 );
      Assert.AreEqual( 0, d.OutsideScope );
      Assert.IsNotNull( d.GetRandomObject() );
      Assert.AreEqual( 3, d.GetSize() );
      Assert.AreEqual( 3, d.GetInitialDomain().Count );

      list.Remove( 2 );

      Assert.AreEqual( 3, d.GetSize() );
      Assert.AreEqual( 3, d.GetInitialDomain().Count );
      for( int i = 0 ; i < d.GetSize() ; ++i )
        Assert.AreEqual( d.GetValue( i ), d.GetInitialDomain()[ i ] );

      d = new DomainWrapper( 10, -9 );
      for( int i = 0 ; i < 10 ; ++i )
      {
        Assert.AreEqual( i - 9, d.GetValue( i ) );
        Assert.AreEqual( i - 9, d.GetInitialDomain()[ i ] );
      }
      Assert.AreEqual( -10, d.OutsideScope );
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void ConstructorFailTest()
    {
      var list = new List<int> { 1, 2, 3 };
      new DomainWrapper( list, 3 );
    }

    // 1 is domain's OutsideValue 
    [TestCase(1, -1)]
    [TestCase(2, 0)]
    [TestCase(3, 1)]
    [TestCase(4, 2)]
    [TestCase(1, 3)]
    public void GetValueTest( int expected, int index )
    {
      Assert.AreEqual( expected, domain.GetValue( index ) );
    }

    [Test]
    public void ResetToInitialTest()
    {
      domain.SetCurrentDomain( new List<int>{ 1, 2, 3 } );
      domain.SetInitialDomain( new List<int>{ 4, 5 } );
      Assert.AreNotEqual( domain.GetSize(), domain.GetInitialSize() );

      domain.ResetToInitial();

      Assert.AreEqual( domain.GetSize(), domain.GetInitialSize() );
      for( int i = 0 ; i < domain.GetSize() ; ++i )
        Assert.AreEqual( domain.GetValue( i ), domain.GetInitialDomain()[ i ] );
    }

    [Test]
    public void RemoveValueTest()
    {
      domain.SetCurrentDomain( new List<int>{ 1, 2, 3 } );
      for( int i = 0 ; i < domain.GetSize() ; ++i )
        Assert.AreEqual( i + 1, domain.GetValue( i ) );

      domain.RemoveValue( 2 );
      Assert.AreEqual( 2, domain.GetSize() );
      Assert.AreEqual( 1, domain.GetValue( 0 ) );
      Assert.AreEqual( 3, domain.GetValue( 1 ) );
    }

    [TestCase(0, 2)]
    [TestCase(4, 10)]
    [TestCase(-1, 123)]
    public void IndexOfTest( int result, int value )
    {
      domain.SetCurrentDomain( new List<int>{ 2, 4, 5, 6, 10, 42 } );
      Assert.AreEqual( result, domain.IndexOf( value ) );
    }

    [Test]
    public void CloneTest()
    {
      var d1 = new DomainWrapper();
      var d2 = (Domain)d1.Clone();

      Assert.IsFalse( d1.IsInitialized() );
      Assert.IsFalse( d2.IsInitialized() );

      d1.SetCurrentDomain( new List<int>{ 1, 2, 3 } );
      d1.SetInitialDomain( new List<int>{ 1, 2, 3, 4 } );

      Assert.IsTrue( d1.IsInitialized() );
      Assert.IsFalse( d2.IsInitialized() );

      d2 = (Domain)d1.Clone();

      Assert.IsTrue( d2.IsInitialized() );
      Assert.AreEqual( 3, d2.GetSize() );
    }
  }
}

