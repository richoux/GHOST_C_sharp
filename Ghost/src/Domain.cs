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
using System.Linq;

namespace ghost
{
  public class Domain : ICloneable
  {
    public Domain( int outsideScope = -1 )
    {
      OutsideScope = outsideScope;
      Random = new Random( Guid.NewGuid().GetHashCode() );
    }

    public Domain( List< int > domain, int outsideScope ): this( outsideScope )
    {
      if( domain.Contains( outsideScope ) )
        throw new ArgumentException("The outside-scope number must not be in domain", "outsideScope");

      CurrentDomain = domain.ConvertAll( v => v );
      InitialDomain = domain.ConvertAll( v => v );
    }

    public Domain( int size, int startValue ) : this( startValue -1 )
    { 
      List<int> list = Enumerable.Range( startValue, size ).ToList();
      CurrentDomain = list.ConvertAll( v => v );
      InitialDomain = list.ConvertAll( v => v );
    }

    #region ICloneable implementation
    public object Clone()
    {
      if( !IsInitialized() )
        return new Domain( OutsideScope );
      else
      {
        var d = new Domain( InitialDomain.ConvertAll( v => v ), OutsideScope );
        d.CurrentDomain = CurrentDomain.ConvertAll( v => v );
        return d;
      }
    }
    #endregion

    // IsInitialized is useful to know if the Domain object
    // is just an empty shell or a properly initialized domain.
    // In some cases, it can be convenient to instanciate a domain
    // object first and to fill it up with values latter.
    public bool IsInitialized()
    {
      return CurrentDomain != null;
    }

    public void ResetToInitial()
    {
      CurrentDomain = InitialDomain.ConvertAll( v => v );
    }
 
    public bool RemoveValue( int value )
    {
      return CurrentDomain.Remove( value );
    }

    public int RandomValue()
    {
      return CurrentDomain[ Random.Next( 0, CurrentDomain.Count ) ];
    }

    public int GetSize()
    {
      return CurrentDomain.Count;
    }

    public int GetInitialSize()
    {
      return InitialDomain.Count;
    }

    public int MaxValue()
    {
      return CurrentDomain.Max();
    }

    public int MinValue()
    {
      return CurrentDomain.Min();
    }

    public int MaxInitialValue()
    {
      return InitialDomain.Max();
    }

    public int MinInitialValue()
    {
      return InitialDomain.Min();
    }

    public int GetValue( int index )
    {
      if( index >= 0 && index < CurrentDomain.Count )
        return CurrentDomain[ index ];
      else
        return OutsideScope;
    }

    public int IndexOf( int value )
    {
      return CurrentDomain.IndexOf( value );
    }

#if DEBUG
    public virtual void Print() { }
#endif

    protected List< int > CurrentDomain { get; set; }
    protected List< int > InitialDomain { get; set; }
    public int OutsideScope { get; protected set; }
    protected Random Random { get; set; }
  }
}