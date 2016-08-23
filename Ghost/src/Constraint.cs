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

namespace ghost
{
  /**
   * Constraint is the abstract class giving a generic interface of constraints.
   * This class is generic, it needs to know both the Variable and the SetVariables types it will work with.
   */  
  public abstract class Constraint<TypeSetVariables, TypeVariable> where TypeSetVariables : SetVariables<TypeVariable> where TypeVariable : Variable
  {
    /**
     * The unique constructor, taking a SetVariables in input.
     */ 
    protected Constraint( TypeSetVariables variables )
    {
      Variables = variables;
    }

    /**
     * Abstract function computing the cost of the constraint.
     * @return A double, the cost of the constraint, ie, the value indicating how much the constraint is violated. The smaller the better. Zero means the constraint is satisfied.
     */ 
    public abstract double Cost();

    /**
     * CostAndUpdateVarCost is a function calling Cost() then UpdateProjectedCost().
     * @return The value returned by Cost(), ie, a double representing the cost of the constraint.
     */ 
    public double CostAndUpdateVarCost()
    {
      var cost = Cost();
      UpdateProjectedCost( cost );
      return cost;
    }

    /**
     * Virtual function calling AddProjectedCostToAll on the set of variables.
     * @param A double representing the current cost of the constraint.
     */
    public virtual void UpdateProjectedCost( double cost )
    {
      Variables.AddProjectedCostToAll( cost );
    }

    /**
     * An abstract function computing the cost of the constraint, regarding the current assignment of each variable.
     * @param variableCost is an array of doubles. Values contained in this array when this function is called are not used. 
     * The purpose of this array is to save the cost of each variable, while computing the constraint cost. 
     * @return A double, the cost of the constraint.
     */ 
    //public abstract double Cost( double[] variableCost );

    /**
     * This function is called by the solver when it looks for a new value for a selected variable 
     * (a variable the solver heuristics choose to modify in priority). Given this variable, SimulateCost 
     * must try all values in the variable domain, computes the new constraint cost and saves 
     * the couple (value, cost) into a Dictionary.
     * @param currentVariableIndex is the index of variable to change the value.
     * @return A Dictionary<int, double> containing the cost for each value in the domain of the variable at 
     * the index currentVariableIndex.
     */ 
    public virtual Dictionary<int, double> SimulateCost( int currentVariableIndex )
    {
      // for each value in currentVariableIndex's domain, save the constraint cost value.
      var simCosts = new Dictionary<int, double>();

      int backup = Variables.GetValue( currentVariableIndex );

      foreach( var pos in Variables.PossibleValues( currentVariableIndex ) )
      {
        Variables.SetValue( currentVariableIndex, pos );
        simCosts[ pos ] = Cost();
      }

      Variables.SetValue( currentVariableIndex, backup );
      return simCosts;
    }


#if DEBUG
    public virtual void Print() { }
#endif

    public TypeSetVariables Variables { get; protected set; } /**< The set of all variables of the problem instance. */
  }
}
