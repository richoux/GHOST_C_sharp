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
using System.Collections.Generic;

namespace ghost
{
  /**
   * Variable is the abstract class giving a generic interface of variables. 
   * A variable is characterised by a name, a long name (called full name), a Domain and its current value (actually, an iterator on its Domain).
   */
  public abstract class Variable
  {
    /**
     * Constructor where the domain will be instanciated by default.
     */
    protected Variable( string name, string fullName ) : this( name, fullName, null, -1 ) { }

    /**
     * Regular constructor. The domain is cloned or instanciated if null. 
     * The domain iterator is assigned to the index of value in Domain, 
     * or to -42 if the Domain has not been initialized.
     * @see Domain.IsInitialized()
     */
    protected Variable( string name, string fullName, Domain domain, int value )
    {
      Name = name;
      FullName = fullName;
      Domain = domain == null ? new Domain() : (Domain)domain.Clone();
      // if value is not in domain, domain.IndexOf( value ) returns -1
      ProjectedCost = 0.0;
      IndexDomain = Domain.IsInitialized() ? Domain.IndexOf( value ) : -42;
    }
            
    /**
     * Reset the variable's domain to its initial values.
     * @see Domain.ResetToInitial()
     */
    public void ResetDomain()
    {
      Domain.ResetToInitial();
    }
      
    /**
     * Set the current value to the next value in the domain, 
     * or to the first one if we reach the domain upper bound.
     */
    public void ShiftValue() 
    {
      if( IndexDomain >= 0 )
        IndexDomain = IndexDomain < Domain.GetSize() - 1 ? IndexDomain + 1 : 0;
    }
      
    /**
     * Set the current value to the previous value in the domain, 
     * or to the last one if we reach the domain lower bound.
     */
    public void UnshiftValue()
    {
      if( IndexDomain >= 0 )
        IndexDomain = IndexDomain > 0 ? IndexDomain - 1 : Domain.GetSize() - 1;
    }

    /**
     * To knwo the current variable value.
     * @return An integer corresponding to the variable's current value.
     */
    public int GetValue()
    {
      return Domain.GetValue( IndexDomain );
    }

    /**
     * Set the current value.
     */
    public void SetValue( int value )
    {
      IndexDomain = Domain.IndexOf( value );
    }

    /**
     * To know what values are in the current domain.
     * @return a List<int> of values belonging to the variable domain.
     */
    public List<int> PossibleValues()
    {
      var possibleValues = new List<int>();

      for( int i = 0 ; i < Domain.GetSize() ; ++i )
        possibleValues.Add( Domain.GetValue( i ) );

      return possibleValues;
    }

    public string Name { get; protected set; } /**< Name is the short name of the variable. For instance, 'b'. */
    public string FullName { get; protected set; } /**< FullName is the long name of the variable. For instance, 'building'. */

    private Domain _domain;
    public Domain Domain /** The Domain, ie, the list of possible variable values */
    { 
      get { return _domain; }  
      set
      { 
        if( value == null )
          throw new ArgumentException("Variable cannot take a null domain", "value");
  
        IndexDomain = value.OutsideScope;
        _domain = value; 
      }
    }

    public double ProjectedCost { get; set; }

#if DEBUG
    public virtual void Print() { }
#endif

    protected int IndexDomain { get; set; } /**< A variable does not contain its current value directly, 
                                                 but the domain index of its current value. 
                                                 This acts like on iterator on the domain */
  }
}
