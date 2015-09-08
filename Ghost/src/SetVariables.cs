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
  public class SetVariables<TypeVariable> where TypeVariable : Variable
  {
    public SetVariables( List<TypeVariable> variables )
    {
      Variables = variables;
    }

    public int GetNumberVariables()
    {
      return Variables.Count;
    }

    public int GetSizeAllDomains()
    {
      int sum = 0;

      for( int i = 0 ; i < Variables.Count ; ++i )
        sum += Variables[ i ].Domain.GetSize();

      return sum;
    }

    public int GetIndex( TypeVariable variable )
    {
      return Variables.IndexOf( variable );
    }

    public bool IsInSet( TypeVariable variable )
    {
      return Variables.Contains( variable );
    }

    public void Swap( int index1, int index2 )
    {
      var tmp = Variables[ index1 ].GetValue();
      Variables[ index1 ].SetValue( Variables[ index2 ].GetValue() );
      Variables[ index2 ].SetValue( tmp );
    }

    public virtual void ResetDomain( int index )
    {
      if( index >= 0 && index < Variables.Count )
        Variables[ index ].ResetDomain();
    }

    public virtual void ResetAllDomains()
    {
      Variables.ForEach( v => v.ResetDomain() );
    }

    public virtual void ShiftValue( int index ) 
    {
      if( index >= 0 && index < Variables.Count )
        Variables[ index ].ShiftValue();
    }

    public virtual void UnshiftValue( int index )
    {
      if( index >= 0 && index < Variables.Count )
        Variables[ index ].UnshiftValue();
    }
      
    public virtual int GetValue( int index )
    {
      if( index >= 0 && index < Variables.Count )
        return Variables[ index ].GetValue();
      else
        throw new IndexOutOfRangeException("Bad index for GetValue method");
    }

    public virtual void SetValue( int index, int value )
    {
      if( index >= 0 && index < Variables.Count )
        Variables[ index ].SetValue( value );
    }

    public virtual List<int> PossibleValues( int index )
    {
      if( index >= 0 && index < Variables.Count )
        return Variables[ index ].PossibleValues();
      else
        throw new IndexOutOfRangeException("Bad index for PossibleValues method");
    }
          
    public string Name( int index )
    {
      if( index >= 0 && index < Variables.Count )
        return Variables[ index ].Name;
      else
        throw new IndexOutOfRangeException("Bad index for Name property");
    }

    public string FullName( int index )
    {
      if( index >= 0 && index < Variables.Count )
        return Variables[ index ].FullName;
      else
        throw new IndexOutOfRangeException("Bad index for FullName property");
    }

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

    protected List<TypeVariable> Variables { get; set; }
  }
}

