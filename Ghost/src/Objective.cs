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
using System;

namespace ghost
{
  public abstract class Objective<TypeSetVariables, TypeVariable> where TypeSetVariables : SetVariables<TypeVariable> where TypeVariable : Variable
  {
    protected Objective( string name, bool permutation = false )
    {
      Name = name;
      IsPermutation = permutation;
      Random = new Random( Guid.NewGuid().GetHashCode() );
    }

    public abstract double Cost( TypeSetVariables variables );

    public virtual int HeuristicVariable( List<int> indexes, TypeSetVariables variables ) 
    {
      return indexes[ Random.Next( 0, indexes.Count ) ];
    }

//    public virtual int HeuristicValue( Dictionary<int, double> globalCostForEachValue, 
//                                       ref double bestEstimatedCost, 
//                                       ref int bestValue )
//    {
//      return Random.Next( 0, globalCostForEachValue.Count );
//    }

    public virtual int HeuristicValue( List<int> valuesIndex, int variableIndex, TypeSetVariables variables )
    {
      // Returns the INDEX of a possible value in valuesIndex, not the value itself!
      return valuesIndex[ Random.Next( 0, valuesIndex.Count ) ];
    }

    public virtual double PostprocessSatisfaction( TypeSetVariables variables,
                                                  ref double bestCost,
                                                  List<int> solution,
                                                  double satTimeout )
    {
      return -1.0;
    }

    public virtual double PostprocessOptimization( TypeSetVariables variables,
                                                  ref double bestCost,
                                                  double optTimeout )
    {
      return -1.0;
    }

#if DEBUG
    public virtual void Print() { }
#endif

    public string Name { get; protected set; }
    public bool IsPermutation { get; protected set; }
    protected Random Random { get; set; }
  }

  public class NullObjective<TypeSetVariables, TypeVariable> : Objective< TypeSetVariables, TypeVariable >
    where TypeSetVariables : SetVariables<TypeVariable> where TypeVariable : Variable
  {
    public NullObjective() : base( "nullObjective" ) { }
    public override double Cost( TypeSetVariables variables ) { return 0.0; }
  }
}