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
using ghost;

namespace Wallin
{
  public enum Race { Terran, Protoss, Zerg, Unknown };

  public class Building : Variable, ICloneable
  {
    public Building( string name, string fullName, Domain domain, int value = -1 ) 
      : this( name, fullName, domain, 0, 0, 0, 0, 0, 0, Race.Unknown, 0, value )
    { }

    public Building(string name, 
                    string fullName, 
                    int length,
                    int height,
                    int gapTop,
                    int gapRight,
                    int gapBottom,
                    int gapLeft,
                    Race race,
                    int treedepth,
                    int value = -1)
      : this( name,
              fullName,
              new Domain( -1 ),
              length,
              height,
              gapTop,
              gapRight,
              gapBottom,
              gapLeft,
              race,
              treedepth,
              value ) { }


    public Building(string name, 
                    string fullName, 
                    Domain domain,
                    int length,
                    int height,
                    int gapTop,
                    int gapRight,
                    int gapBottom,
                    int gapLeft,
                    Race race,
                    int treedepth,
                    int value = -1)
      : base( name, fullName, domain, value )
    { 
      Length = length;
      Height = height;
      GapTop = gapTop;
      GapRight = gapRight;
      GapBottom = gapBottom;
      GapLeft = gapLeft;
      Race = race;
      Treedepth = treedepth;
    }

    #region ICloneable implementation
    public object Clone()
    {
      return new Building( Name, FullName, (Domain)Domain.Clone(), Length, Height, GapTop, GapRight, GapBottom, GapLeft, Race, Treedepth, GetValue() );
    }
    #endregion

    public int Surface() { return Height * Length; }
    public bool IsSelected() { return IndexDomain >= 0; }
    public string RaceString()	
    { 
      return Race.ToString();
    }
    
    public int Length { get; private set; }
    public int Height { get; private set; }
    public int GapTop { get; private set; }
    public int GapRight { get; private set; }
    public int GapBottom { get; private set; }
    public int GapLeft { get; private set; }
    public Race Race { get; private set; }
    public int Treedepth { get; private set; }
  }
}
