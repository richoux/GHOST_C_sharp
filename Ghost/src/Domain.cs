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
  /**
   * Domain is the class implementing variables' domains, ie, the set of possible values a variable can take.
   * In GHOST, such values must be integers.
   * 
   * A domain contains, the list of current possible values of the variable it belongs to, 
   * the initial list of such values (if one wants to reset the domain), an integer representing 
   * values outside the domain scope and a pseudo-random number generator.
   */
  public class Domain : ICloneable
  {
    /**
     * Basic constructor taking the outside-the-scope value (-1 by default).
     */
    public Domain( int outsideScope = -1 )
    {
      OutsideScope = outsideScope;
      Random = new Random( Guid.NewGuid().GetHashCode() );
    }

    /**
     * Constructor taking the outside-the-scope value and a list of integer values, to 
     * make both the initial and current possible variable values. The outside-the-scope value
     * must not belong to this list, or an ArgumentException is raised.
     */
    public Domain( List< int > domain, int outsideScope ): this( outsideScope )
    {
      if( domain.Contains( outsideScope ) )
        throw new ArgumentException("The outside-scope number must not be in domain", "outsideScope");

      CurrentDomain = domain.ConvertAll( v => v );
      InitialDomain = domain.ConvertAll( v => v );
    }

    /**
     * Constructor taking the domain size N and a starting value x, and creating a domain
     * with all values in [x, x + N]. The outside-the-scope value is set to x-1.
     */
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

    /** 
     * Used to know if the Domain object is just an empty shell or a properly 
     * initialized domain. In some cases, it can be convenient to instanciate 
     * a domain object first and to fill it up with values latter.
     */
    public bool IsInitialized()
    {
      return CurrentDomain != null;
    }

    /**
     * Resets the set of current values to the set of initial values. 
     * Allow the recover all values in the domain if we filtered some of them.
     */
    public void ResetToInitial()
    {
      CurrentDomain = InitialDomain.ConvertAll( v => v );
    }
 
    /**
     * Deletes a given value from the set of current domain values.
     * @param value is the value to remove from the domain
     * @return True if and only if the value has been removed.
     */
    public bool RemoveValue( int value )
    {
      return CurrentDomain.Remove( value );
    }

    /**
     * Returns a random value from the domain.
     * @return an integer with a (random) value from the domain.
     */
    public int RandomValue()
    {
      return CurrentDomain[ Random.Next( 0, CurrentDomain.Count ) ];
    }

    /**
     * Get the number of values currently contained by the domain.
     */
    public int GetSize()
    {
      return CurrentDomain.Count;
    }

    /**
     * Get the number of values initially contained by the domain.
     */
    public int GetInitialSize()
    {
      return InitialDomain.Count;
    }

    /**
     * Get the highest value in the domain. 
     */
    public int MaxValue()
    {
      return CurrentDomain.Max();
    }

    /**
     * Get the lowest value in the domain. 
     */
    public int MinValue()
    {
      return CurrentDomain.Min();
    }

    /**
     * Get the highest value in the initial domain. 
     */
    public int MaxInitialValue()
    {
      return InitialDomain.Max();
    }

    /**
     * Get the lowest value in the initial domain. 
     */
    public int MinInitialValue()
    {
      return InitialDomain.Min();
    }

    /**
     * Get the value at the given index
     * @param index is the index of the desired value
     * @return The value at the given index if this one is in the range of the domain, 
     * otherwise the outside-the-scope value.
     */
    public int GetValue( int index )
    {
      if( index >= 0 && index < CurrentDomain.Count )
        return CurrentDomain[ index ];
      else
        return OutsideScope;
    }

    /**
     * Get the index of a given value.
     * @return If the given value is in the domain, it returns its index, and -1 otherwise.
     */ 
    public int IndexOf( int value )
    {
      return CurrentDomain.IndexOf( value );
    }

#if DEBUG
    public virtual void Print() { }
#endif

    protected List< int > CurrentDomain { get; set; } /**< List of integers containing the current values of the domain */
    protected List< int > InitialDomain { get; set; } /**< List of integers containing the initial values of the domain */
    public int OutsideScope { get; protected set; } /**< Value representing all values the domain */
    protected Random Random { get; set; } /**< A random generator only used in RandomValue() */
  }
}