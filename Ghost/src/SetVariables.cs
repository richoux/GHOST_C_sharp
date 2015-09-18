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
   * SetVariables is the class grouping all variables we need to instance a given problem.
   * This class is generic, it needs to know the Variable type it will work with.
   * 
   * Sometimes, it can be convenient to share information among variables. This is the purpose of SetVariables. 
   */
  public class SetVariables<TypeVariable> where TypeVariable : Variable
  {

    /**
     * The unique constructor taking a list of variables in input. 
     */
    public SetVariables( List<TypeVariable> variables )
    {
      Variables = variables;
    }

    /**
     * Set each variable of the set to a random value, calling Domain.RandomValue(). 
     */
    public void RandomInitialization()
    {
      foreach( Variable v in Variables )
      {
        v.SetValue( v.Domain.RandomValue() );
      }
    }

    /**
     * Returns the number of variables in the set. 
     */
    public int GetNumberVariables()
    {
      return Variables.Count;
    }

    /**
     * Returns the sum of all variables domain size. Notice this does NOT return the 
     * sum of variables initial domain size.
     */
    public int GetSizeAllDomains()
    {
      int sum = 0;

      for( int i = 0 ; i < Variables.Count ; ++i )
        sum += Variables[ i ].Domain.GetSize();

      return sum;
    }

    /**
     * Given a variable object, returns its index in the set.
     * @return The index of the searched variable, or -1 if the given variable is not in the set.
     */ 
    public int GetIndex( TypeVariable variable )
    {
      return Variables.IndexOf( variable );
    }

    /**
     * Tests if the given variable is in the set. 
     */
    public bool IsInSet( TypeVariable variable )
    {
      return Variables.Contains( variable );
    }

    /**
     * Swap values of two variables.
     * @param index1 is the index of the first variable.
     * @param index2 is the index of the second variable.
     */ 
    public void Swap( int index1, int index2 )
    {
      var tmp = Variables[ index1 ].GetValue();
      Variables[ index1 ].SetValue( Variables[ index2 ].GetValue() );
      Variables[ index2 ].SetValue( tmp );
    }

    /**
     * Reset the domain of a variable to its initial domain.
     * @param index is the index of the considered variable. If the index is 
     * out of range, the function does nothing.
     */ 
    public virtual void ResetDomain( int index )
    {
      if( index >= 0 && index < Variables.Count )
        Variables[ index ].ResetDomain();
    }

    /**
     * Reset the domain of each variable to their initial domain.
     */ 
    public virtual void ResetAllDomains()
    {
      Variables.ForEach( v => v.ResetDomain() );
    }

    /**
     * Calls Variable.ShiftValue() on a given variable.
     * @param index is the index of the considered variable. If the index is 
     * out of range, the function does nothing.
     */ 
    public virtual void ShiftValue( int index ) 
    {
      if( index >= 0 && index < Variables.Count )
        Variables[ index ].ShiftValue();
    }

    /**
     * Calls Variable.UnshiftValue() on a given variable.
     * @param index is the index of the considered variable. If the index is 
     * out of range, the function does nothing.
     */ 
    public virtual void UnshiftValue( int index )
    {
      if( index >= 0 && index < Variables.Count )
        Variables[ index ].UnshiftValue();
    }
      
    /**
     * Returns the value of a given variable.
     * @param index is the index of the considered variable. If the index is 
     * out of range, an IndexOutOfRangeException is raised.
     */ 
    public virtual int GetValue( int index )
    {
      if( index >= 0 && index < Variables.Count )
        return Variables[ index ].GetValue();
      else
        throw new IndexOutOfRangeException("Bad index for GetValue method");
    }

    /**
     * Set the value of a given variable.
     * @param index is the index of the considered variable. If the index is 
     * out of range, the function does nothing.
     * @param value is the new value to assign.
     * @see Variable.SetValue()
     */ 
    public virtual void SetValue( int index, int value )
    {
      if( index >= 0 && index < Variables.Count )
        Variables[ index ].SetValue( value );
    }

    /**
     * Returns the list of possible values of a given variable.
     * @param index is the index of the considered variable. If the index is 
     * out of range, an IndexOutOfRangeException is raised.
     * @see Variable.PossibleValues()
     */ 
    public virtual List<int> PossibleValues( int index )
    {
      if( index >= 0 && index < Variables.Count )
        return Variables[ index ].PossibleValues();
      else
        throw new IndexOutOfRangeException("Bad index for PossibleValues method");
    }

    /**
     * Returns the name of a given variable.
     * @param index is the index of the considered variable. If the index is 
     * out of range, an IndexOutOfRangeException is raised.
     * @see Variable.Name()
     */ 
    public string Name( int index )
    {
      if( index >= 0 && index < Variables.Count )
        return Variables[ index ].Name;
      else
        throw new IndexOutOfRangeException("Bad index for Name property");
    }

    /**
     * Returns the long name of a given variable.
     * @param index is the index of the considered variable. If the index is 
     * out of range, an IndexOutOfRangeException is raised.
     * @see Variable.FullName()
     */ 
    public string FullName( int index )
    {
      if( index >= 0 && index < Variables.Count )
        return Variables[ index ].FullName;
      else
        throw new IndexOutOfRangeException("Bad index for FullName property");
    }

    /**
     * Returns the domain of a given variable. Notice the difference with SetVariables.PossibleValues: 
     * here a Domain object is returned. 
     * @param index is the index of the considered variable. If the index is 
     * out of range, an IndexOutOfRangeException is raised.
     * @see Variable.Domain()
     */ 
    public Domain Domain( int index )
    {
      if( index >= 0 && index < Variables.Count )
        return Variables[ index ].Domain;
      else
        throw new IndexOutOfRangeException("Bad index for Domain property");
    }

//#if DEBUG
    public virtual void Print() { }
//#endif

    protected List<TypeVariable> Variables { get; set; } /**< The set of variables, implemented as a List */
  }
}

