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
      Assert.That( d.OutsideScope, Is.EqualTo( -1 ) );
      Assert.That( d.GetRandomObject(), Is.Not.Null );
      Assert.That( d.GetCurrentDomain(), Is.Null );
      Assert.That( d.GetInitialDomain(), Is.Null );

      d = new DomainWrapper( 51 );
      Assert.That( d.OutsideScope, Is.EqualTo( 51 ) );
      Assert.That( d.GetRandomObject(), Is.Not.Null );
      Assert.That( d.GetCurrentDomain(), Is.Null );
      Assert.That( d.GetInitialDomain(), Is.Null );

      var list = new List<int> { 1, 2, 3 };
      d = new DomainWrapper( list, 0 );
      Assert.That( d.OutsideScope, Is.EqualTo( 0 ) );
      Assert.That( d.GetRandomObject(), Is.Not.Null );
      Assert.That( d.GetSize(), Is.EqualTo( 3 ) );
      Assert.That( d.GetInitialDomain().Count, Is.EqualTo( 3 ) );

      list.Remove( 2 );

      Assert.That( d.GetSize(), Is.EqualTo( 3 ) );
      Assert.That( d.GetInitialDomain().Count, Is.EqualTo( 3 ) );
      for( int i = 0 ; i < d.GetSize() ; ++i )
        Assert.That( d.GetInitialDomain()[ i ], Is.EqualTo( d.GetValue( i ) ) );

      d = new DomainWrapper( 10, -9 );
      for( int i = 0 ; i < 10 ; ++i )
      {
        Assert.That( d.GetValue( i ), Is.EqualTo( i - 9 ) );
        Assert.That( d.GetInitialDomain()[ i ], Is.EqualTo( i - 9 ) );
      }
      Assert.That( d.OutsideScope, Is.EqualTo( -10 ) );
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void ConstructorFailTest()
    {
      var list = new List<int> { 1, 2, 3 };
      new DomainWrapper( list, 3 );
    }

    // 1 is domain's OutsideValue 
    [TestCase(-1, Result=1)]
    [TestCase(0, Result=2)]
    [TestCase(1, Result=3)]
    [TestCase(2, Result=4)]
    [TestCase(3, Result=1)]
    public int GetValueTest( int index )
    {
      return domain.GetValue( index );
    }

    [Test]
    public void ResetToInitialTest()
    {
      domain.SetCurrentDomain( new List<int>{ 1, 2, 3 } );
      domain.SetInitialDomain( new List<int>{ 4, 5 } );
      Assert.That( domain.GetSize(), Is.Not.EqualTo( domain.GetInitialSize() ) );

      domain.ResetToInitial();

      Assert.That( domain.GetSize(), Is.EqualTo( domain.GetInitialSize() ) );
      for( int i = 0 ; i < domain.GetSize() ; ++i )
        Assert.That( domain.GetValue( i ), Is.EqualTo( domain.GetInitialDomain()[ i ] ) );
    }

    [Test]
    public void RemoveValueTest()
    {
      domain.SetCurrentDomain( new List<int>{ 1, 2, 3 } );
      for( int i = 0 ; i < domain.GetSize() ; ++i )
        Assert.That( domain.GetValue( i ), Is.EqualTo( i + 1 ) );

      domain.RemoveValue( 2 );
      Assert.That( domain.GetSize(), Is.EqualTo( 2 ) );
      Assert.That( domain.GetValue( 0 ), Is.EqualTo( 1 ) );
      Assert.That( domain.GetValue( 1 ), Is.EqualTo( 3 ) );
    }

    [TestCase(2, Result=0)]
    [TestCase(10, Result=4)]
    [TestCase(123, Result=-1)]
    public int IndexOfTest( int value )
    {
      domain.SetCurrentDomain( new List<int>{ 2, 4, 5, 6, 10, 42 } );
      return domain.IndexOf( value );
    }

    [Test]
    public void CloneTest()
    {
      var d1 = new DomainWrapper();
      var d2 = (Domain)d1.Clone();

      Assert.That( d1.IsInitialized(), Is.False );
      Assert.That( d2.IsInitialized(), Is.False );

      d1.SetCurrentDomain( new List<int>{ 1, 2, 3 } );
      d1.SetInitialDomain( new List<int>{ 1, 2, 3, 4 } );

      Assert.That( d1.IsInitialized(), Is.True );
      Assert.That( d2.IsInitialized(), Is.False );

      d2 = (Domain)d1.Clone();

      Assert.That( d2.IsInitialized(), Is.True );
      Assert.That( d2.GetSize(), Is.EqualTo( 3 ) );
    }
  }
}

