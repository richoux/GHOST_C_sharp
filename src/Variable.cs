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
  public abstract class Variable
  {
    protected Variable( string name, string fullName ) : this( name, fullName, null, -1 ) { }
    protected Variable( string name, string fullName, Domain domain, int value )
    {
      Name = name;
      FullName = fullName;
      Domain = domain == null ? new Domain() : (Domain)domain.Clone();
      // if value is not in domain, domain.IndexOf( value ) returns -1
      IndexDomain = Domain.IsInitialized() ? Domain.IndexOf( value ) : -42;
    }
            
    public void ResetDomain()
    {
      Domain.ResetToInitial();
    }
      
    public void ShiftValue() 
    {
      if( IndexDomain >= 0 )
        IndexDomain = IndexDomain < Domain.GetSize() - 1 ? IndexDomain + 1 : 0;
    }
      
    public void UnshiftValue()
    {
      if( IndexDomain >= 0 )
        IndexDomain = IndexDomain > 0 ? IndexDomain - 1 : Domain.GetSize() - 1;
    }

    public int GetValue()
    {
      return Domain.GetValue( IndexDomain );
    }

    public void SetValue( int value )
    {
      IndexDomain = Domain.IndexOf( value );
    }

    public List<int> PossibleValues()
    {
      var possibleValues = new List<int>();

      for( int i = 0 ; i < Domain.GetSize() ; ++i )
        possibleValues.Add( Domain.GetValue( i ) );

      return possibleValues;
    }

    public string Name { get; protected set; }
    public string FullName { get; protected set; }

    private Domain _domain;
    public Domain Domain
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

#if DEBUG
    public virtual void Print() { }
#endif

    protected int IndexDomain { get; set; }
  }
}
