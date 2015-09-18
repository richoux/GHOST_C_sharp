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
  /**
   * Objective is the abstract class giving a generic interface of objectives.
   * This class is generic, it needs to know both the Variable and the SetVariables types it will work with.
   */   
  public abstract class Objective<TypeSetVariables, TypeVariable> where TypeSetVariables : SetVariables<TypeVariable> where TypeVariable : Variable
  {
    /**
     * The unique constructor, taking the name of the objective and a boolean telling if the problem is a 
     * permutation problem or not (false by default). 
     */ 
    protected Objective( string name, bool permutation = false )
    {
      Name = name;
      IsPermutation = permutation;
      Random = new Random( Guid.NewGuid().GetHashCode() );
    }

    /**
     * The abstract function to compute the objective function cost, regarding the current variables assignment.
     * GHOST deals with minimization problems only! If you need to find a maximal value, an easy trick is to return 
     * '-cost' instead of 'cost'.
     * @param variables is the set of variables of the problem instance.
     * @return A double, the cost of the objective function.
     */ 
    public abstract double Cost( TypeSetVariables variables );

    /**
     * When several variables have the same heaviest cost among all variables, this function can be used to apply 
     * a heuristic to decide which variable to select and change.
     * The function is virtual. By default, it chooses randomly a variable among variables to change with the same 
     * highest priority.
     * @param indexes is the list of indexes of 'worst variables' to change.
     * @param variables is the set of all variables of the problem instance. Data contained in this set may be 
     * useful to apply some heuristics.
     * @return The index of the variable to change.
     * @see HeuristicValue()
     */ 
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

    /**
     * Similar to HeuristicVariable, but for values: once the worst variable x to change is selected, 
     * Cosntraint.SimulateCost will compute the cost for each values in the domain of x. It may occur that 
     * several values lead to the same locally optimal cost. HeuristicValue is then called to split these values up
     * and return one among others following the written heuristic.
     * @param valuesIndex is the list of indexes of best values in the domain of the variable indexed by variableIndex.
     * @param variableIndex is the index of the worst variable x to change.
     * @param variables is the set of all variables of the problem instance. Data contained in this set may be 
     * useful to apply some heuristics.
     * @return The index in the domain of the best value to assign.
     * @see HeuristicVariable
     */ 
    public virtual int HeuristicValue( List<int> valuesIndex, int variableIndex, TypeSetVariables variables )
    {
      // Returns the INDEX of a possible value in valuesIndex, not the value itself!
      return valuesIndex[ Random.Next( 0, valuesIndex.Count ) ];
    }

    /**
     * After finding a solution satisfying all constraints, PostprocessSatisfaction can be called to 'clean up' 
     * this solution, and correct some weird choices made by the solver.
     * @param variables is the set of variables of the problem instance.
     * @param bestCost is the reference of the best objective cost found so far. It can be improved 
     * by PostprocessSatisfaction.
     * @param solution is the assignment of each variable, i.e., the List of current values.
     * @param satTimeout is the computation timeout set by the user for the solver satisfaction loop.
     * @return Its own running time.
     */ 
    public virtual double PostprocessSatisfaction( TypeSetVariables variables,
                                                  ref double bestCost,
                                                  List<int> solution,
                                                  double satTimeout )
    {
      return -1.0;
    }

    /**
     * After finding a solution minimizing the objective function, PostprocessOptimization can apply a last-chance 
     * optimization to improve the solution quality.
     * @param variables is the set of variables of the problem instance.
     * @param bestCost is the reference of the best objective cost found so far. It can be improved 
     * by PostprocessOptimization.
     * @param optTimeout is the computation timeout set by the user for the solver optimization loop.
     * @return Its own running time.
     */ 
    public virtual double PostprocessOptimization( TypeSetVariables variables,
                                                  ref double bestCost,
                                                  double optTimeout )
    {
      return -1.0;
    }

#if DEBUG
    public virtual void Print() { }
#endif

    public string Name { get; protected set; } /**< The name of the objective. Mostly used for debugging purpose */
    public bool IsPermutation { get; protected set; } /**< A boolean to specify if the problem is a permutation problem or not. 
                                                           Set by default to false by the constructor. 
                                                           @see Objective().*/
    protected Random Random { get; set; } /**< A pseudo-random number generator. It can be used by HeuristicVariable and HeuristicValue. */
  }

  /**
   * NullObjective is an empty objective function, useful if one is only looking to solve a satisfaction problem 
   * (i.e., return the first solution the solver can find among all).
   * The name of such a fake objective will be 'nullObjective'. Cost will always return 0.0.
   */ 
  public class NullObjective<TypeSetVariables, TypeVariable> : Objective< TypeSetVariables, TypeVariable >
    where TypeSetVariables : SetVariables<TypeVariable> where TypeVariable : Variable
  {
    public NullObjective() : base( "nullObjective" ) { }
    public override double Cost( TypeSetVariables variables ) { return 0.0; }
  }
}