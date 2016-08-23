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
using Wallin;

namespace WallinTest
{
  [TestFixture]
  public class VariableTest
  {
    public static Variable building = new Variable("A",
                                                   "Terran_Academy",
                                                   new ghost.Domain( 16 * 12, 0 ),
                                                   3,
                                                   2,
                                                   0,
                                                   3,
                                                   7,
                                                   8,
                                                   Race.Terran,
                                                   2);

    [Test]
    public void SurfaceTest()
    {
      Assert.That( building.Surface(), Is.EqualTo( 6 ) );
    }

    [Test]
    public void IsSelectedTest()
    {
      building.SetValue( -1 );
      Assert.That( building.IsSelected(), Is.False );
      building.SetValue( 0 );
      Assert.That( building.IsSelected(), Is.True );
      building.SetValue( 42 );
      Assert.That( building.IsSelected(), Is.True );
      // value outside the scope of the domain 16*12
      building.SetValue( 1024 );
      Assert.That( building.IsSelected(), Is.False );
    }

    [Test]
    public void RaceStringTest()
    {
      Assert.That( building.RaceString(), Is.EqualTo( "Terran" ) );
    }
  }
}

