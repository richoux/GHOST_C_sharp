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
using ghost;
using System;

namespace GhostTest
{
  public class VariableWrapper : Variable
  {
    public VariableWrapper( string name, string fullName ) : base( name, fullName ) { }
    public VariableWrapper( string name, string fullName, Domain domain, int value ) : base( name, fullName, domain, value ) { }
    public int GetIndexDomain() { return IndexDomain; }
  }

  [TestFixture]
  public class VariableTest
  {
    public static VariableWrapper variable = new VariableWrapper("b", "building", new Domain( 3, 0 ), -1 );

    [Test]
    public void ConstructorsTest()
    {
      var v1 = new VariableWrapper( "a", "bb" );
      var v2 = new VariableWrapper( "a", "bb" );
      var v3 = new VariableWrapper( "a", "bb" );
      Assert.AreEqual( "a", v1.Name );
      Assert.AreEqual( "bb", v1.FullName );
      Assert.AreEqual( -1, v1.GetValue() );

      v1 = new VariableWrapper( "a", "bb", new Domain( 5, 0 ), 4 );
      v2 = new VariableWrapper( "a", "bb", new Domain( 20, -4 ), -5 );
      v3 = new VariableWrapper( "a", "bb", new Domain( 10, 1 ), -2 );
      Assert.AreEqual( 4, v1.GetValue() );
      Assert.AreEqual( -5, v2.GetValue() );
      Assert.AreEqual( 0, v3.GetValue() );
    }

    [Test]
    public void SetGetValueTest()
    {
      // default index should be -1
      Assert.AreEqual( variable.Domain.OutsideScope, variable.GetValue() );
      variable.SetValue( 0 );
      Assert.AreEqual( 0, variable.GetValue() );
      variable.SetValue( 1 );
      Assert.AreEqual( 1, variable.GetValue() );
      variable.SetValue( 2 );
      Assert.AreEqual( 2, variable.GetValue() );
      variable.SetValue( -1 );
      Assert.AreEqual( variable.Domain.OutsideScope, variable.GetValue() );
    }

    [Test]
    public void ShiftValueTest()
    {
      variable.SetValue( -1 );

      Assert.AreEqual( variable.Domain.OutsideScope, variable.GetValue() );
      variable.ShiftValue();
      Assert.AreEqual( variable.Domain.OutsideScope, variable.GetValue() );
      variable.SetValue( 1 );
      Assert.AreEqual( 1, variable.GetValue() );
      variable.ShiftValue();
      Assert.AreEqual( 2, variable.GetValue() );
      variable.ShiftValue();
      Assert.AreEqual( 0, variable.GetValue() );
      variable.ShiftValue();
      Assert.AreEqual( 1, variable.GetValue() );
    }

    [Test]
    public void UnshiftValueTest()
    {
      variable.SetValue( -1 );

      Assert.AreEqual( variable.Domain.OutsideScope, variable.GetValue() );
      variable.UnshiftValue();
      Assert.AreEqual( variable.Domain.OutsideScope, variable.GetValue() );
      variable.SetValue( 1 );
      Assert.AreEqual( 1, variable.GetValue() );
      variable.UnshiftValue();
      Assert.AreEqual( 0, variable.GetValue() );
      variable.UnshiftValue();
      Assert.AreEqual( 2, variable.GetValue() );
      variable.UnshiftValue();
      Assert.AreEqual( 1, variable.GetValue() );
    }

    [Test]
    public void DomainTest()
    {
      var v = new VariableWrapper( "x", "XX" );
      Assert.AreEqual( -42, v.GetIndexDomain() );
      v.Domain = new Domain( -5 );
      Assert.AreEqual( -5, v.GetIndexDomain() );
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void DomainFailTest()
    {
      var v = new VariableWrapper( "x", "XX" );
      v.Domain = null;
    }

  }
}

