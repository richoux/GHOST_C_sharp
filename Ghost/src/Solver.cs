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
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace ghost
{
  public class Solver<TypeVariable, TypeSetVariables, TypeConstraint> 
        where TypeVariable : Variable
        where TypeSetVariables : SetVariables<TypeVariable>
        where TypeConstraint : Constraint<TypeSetVariables, TypeVariable>
  {
    public Solver( TypeSetVariables vecVariables, 
                   List< TypeConstraint > vecConstraints,
                   Objective<TypeSetVariables, TypeVariable> obj = null )
      : this( vecVariables, vecConstraints, obj, 0 )
    { }

    public Solver( TypeSetVariables vecVariables, 
                   List< TypeConstraint > vecConstraints,
                   Objective<TypeSetVariables, TypeVariable> obj,
                   int loops )
    {
      _variables = vecVariables;
      _constraints = vecConstraints;
      _numberVariables = _variables.GetNumberVariables();
      _variablesCost = new double[ _numberVariables ];
      _loops = loops;
      _tabuList = new List<int>( _numberVariables );
      _bestSolution = new List<int>( _numberVariables );
      _variables.ResetAllDomains();

      if( obj == null )
      {
        _objective = new NullObjective<TypeSetVariables, TypeVariable>();
        _isNullObjective = true;
      }
      else
      {
        _objective = obj;
        _isNullObjective = false;
      }
    }
    
    private TypeSetVariables     _variables;
    private int                  _numberVariables;
    private List<TypeConstraint> _constraints;
    private Objective<TypeSetVariables, TypeVariable> _objective;
    private double[]             _variablesCost;	
    private int                  _loops;		
    private List<int>            _tabuList;	
    private double               _bestCost;	
    private List<int>            _bestSolution;	
    private bool                 _isNullObjective;

    public double solve( double satTimeout, double optTimeout = 0, bool doRandomInitialization = true )
    {
      if( optTimeout.Equals(0) )
	      optTimeout = satTimeout * 10;

      int tabuLength = _numberVariables - 1;

      var start = new Stopwatch();
      Stopwatch startTour;
      start.Start();

      double timerPostProcessOpt = 0;
      
      var	constraintsCost = new Dictionary<int, double>[ _constraints.Count ];
      var	globalCostForEachValue = new Dictionary<int, double>();
      var	allVariablesCostForEachValue = new Dictionary<int, double[] >();

      _bestCost = double.MaxValue;
      double beforePostProc = double.MaxValue;
      double bestGlobalCost = double.MaxValue;
      double globalCost;

      var worstVariables = new List<int>();
      double worstVariableCost;
      int worstVariableIndex;

      var bestSimulatedGlobalCostsForValueSelection = new List<int>();
      double bestSimulatedGlobalCost;
      int bestSimulatedGlobalCostIndex;

      var bestVariablesCostList = new double[ _numberVariables ];

      int tour = 0;
      int iterations = 0;

      do // optimization loop
      {
        startTour = new Stopwatch();
        startTour.Start();
	      ++tour;
      	globalCost = double.MaxValue;
        for( var i = 0 ; i < _variablesCost.Count() ; ++i )
          _variablesCost[ i ] = 0.0;
        _tabuList.AddRange( Enumerable.Repeat( 0, _numberVariables ) );
        
	      do // solving loop 
      	{
      	  ++iterations;
	  
          if( globalCost.Equals( double.MaxValue ) )
      	  {
            _variables.ResetAllDomains();

            if( doRandomInitialization )
              _variables.RandomInitialization();
            
            double cost = 0.0;

            foreach( var c in _constraints )
      	      cost += c.Cost( _variablesCost );
            
            if( !cost.Equals( double.MaxValue ) )
      	      globalCost = cost;
      	    else
	          {  
              _variables.ResetAllDomains();
	            continue;
            }
          }

          // make sure there is at least one untabu variable
	        bool freeVariables = false;

	        // Update tabu list
          for( int i = 0 ; i < _tabuList.Count ; ++i )
          {
	          if( _tabuList[i] <= 1 )
	          {
	            _tabuList[i] = 0;
	            if( !freeVariables )
		            freeVariables = true;      
	          }
	          else
	            --_tabuList[i];
          }

	        // Here, we look at neighbor configurations with the lowest cost.
	        worstVariables.Clear();
	        worstVariableCost = 0;

          for( int i = 0; i < _variablesCost.Count(); ++i )
	        {
	          if( !freeVariables || _tabuList[i] == 0 )
	          {
	            if( worstVariableCost < _variablesCost[i] )
	            {
		            worstVariableCost = _variablesCost[i];
		            worstVariables.Clear();
		            worstVariables.Add( i );
	            }
              else if( worstVariableCost.Equals( _variablesCost[i] ) )
                worstVariables.Add( i );	  
	          }
	        }

	        // can apply some heuristics here, according to the objective function
          worstVariableIndex = _objective.HeuristicVariable( worstVariables, _variables );
      
	        // variable simulated costs
          for( var i = 0 ; i < bestVariablesCostList.Count() ; ++i )
            bestVariablesCostList[i] = 0.0;

          allVariablesCostForEachValue.Clear();
          foreach( var i in _variables.PossibleValues( worstVariableIndex ) )
            allVariablesCostForEachValue[ i ] = new double[ _numberVariables ];

	        for( int i = 0 ; i < _constraints.Count ; ++i )
            constraintsCost[i] = _constraints[i].SimulateCost( worstVariableIndex, allVariablesCostForEachValue );

          bestSimulatedGlobalCostsForValueSelection.Clear();
          bestSimulatedGlobalCost = double.MaxValue;

          foreach( var j in _variables.PossibleValues( worstVariableIndex ) )
          {
            // sum all numbers in the List constraintsCost[i][j] and put it into listGlobalCost[j]
            globalCostForEachValue[ j ] = 0;
            for( int i = 0 ; i < _constraints.Count ; ++i )
              globalCostForEachValue[ j ] += constraintsCost[ i ][ j ];

           // replace all negative numbers by the max value for double
            if( globalCostForEachValue[ j ] < 0 )
              globalCostForEachValue[ j ] = double.MaxValue;
            else 
            {
              if( globalCostForEachValue[ j ] < bestSimulatedGlobalCost )
              {
                bestSimulatedGlobalCost = globalCostForEachValue[ j ];
                bestSimulatedGlobalCostsForValueSelection.Clear();
                bestSimulatedGlobalCostsForValueSelection.Add( _variables.PossibleValues( worstVariableIndex ).IndexOf( j ) );
              }
              else if( bestSimulatedGlobalCost.Equals( globalCostForEachValue[ j ] ) )
                bestSimulatedGlobalCostsForValueSelection.Add( _variables.PossibleValues( worstVariableIndex ).IndexOf( j ) );
            }
          }

	        // look for the first smallest cost, according to objective heuristic
          bestSimulatedGlobalCostIndex = _objective.HeuristicValue( bestSimulatedGlobalCostsForValueSelection, worstVariableIndex, _variables );
          bestVariablesCostList = allVariablesCostForEachValue[ _variables.PossibleValues( worstVariableIndex )[ bestSimulatedGlobalCostIndex ] ];

          if( bestSimulatedGlobalCost < globalCost )
	        {
            globalCost = bestSimulatedGlobalCost;

	          if( _objective.IsPermutation )
              permut( worstVariableIndex, bestSimulatedGlobalCostIndex );
	          else
              move( worstVariableIndex, bestSimulatedGlobalCostIndex );

#if DEBUG
            Console.WriteLine("#######");
            var dummy = new double[_variables.GetNumberVariables()];
            for(var i = 0 ; i < _constraints.Count ; ++i )
              Console.WriteLine("_constraints[{0}]:{1}", i, _constraints[ i ].Cost( dummy ) );          
            
            Console.WriteLine("tour:{2}, iter:{3}, globalCost:{0}, bestGlobalCost:{1}, var:{4}, bestPosIndex:{5}, bestPos:{6}",
                              globalCost,bestGlobalCost,tour,iterations,worstVariableIndex,bestSimulatedGlobalCostIndex,_variables.PossibleValues( worstVariableIndex )[ bestSimulatedGlobalCostIndex ]);
            for( var i = 0 ; i < _variables.GetNumberVariables() ; ++i )
              Console.WriteLine( "var[{0}] = {1}", i, _variables.GetValue( i ) );
            _variables.Print();
#endif

	          if( globalCost < bestGlobalCost )
	          {
	            bestGlobalCost = globalCost;
              _bestSolution.Clear();
              for( int i = 0 ; i < _numberVariables ; ++i )
                _bestSolution.Add( _variables.GetValue( i ) );
            }
	    
	          _variablesCost = bestVariablesCostList;
	        }
	        else // local minima
	          _tabuList[ worstVariableIndex ] = tabuLength;
        } while( !globalCost.Equals( 0 ) && startTour.ElapsedMilliseconds < satTimeout && start.ElapsedMilliseconds < optTimeout );

	      if( globalCost.Equals( 0 ) )
        {
          // compute the objective value
          var objectiveCost = _objective.Cost( _variables );
          if( _bestCost > objectiveCost )
          {
            _bestCost = objectiveCost;
            _bestSolution.Clear();
            for( int i = 0 ; i < _numberVariables ; ++i )
              _bestSolution.Add( _variables.GetValue( i ) );
          }

          // eventually try to improve it via an ad-hoc post-process
	        _objective.PostprocessSatisfaction( _variables, ref _bestCost, _bestSolution, satTimeout );
        }

      } while( ( ( !_isNullObjective || _loops == 0 )  && ( start.ElapsedMilliseconds < optTimeout ) )
              || ( _isNullObjective && start.ElapsedMilliseconds < satTimeout * _loops ) );

      for( int i = 0 ; i < _numberVariables ; ++i )
        _variables.SetValue( i, _bestSolution[i] );

      beforePostProc = _bestCost;
      
      if( bestGlobalCost.Equals( 0 ) )
	      timerPostProcessOpt = _objective.PostprocessOptimization( _variables, ref _bestCost, optTimeout );

//#if DEBUG
      Console.WriteLine( "############" );

      for( var i = 0 ; i < _variables.GetNumberVariables() ; ++i )
        Console.WriteLine( "var[{0}] = {1}", i, _variables.GetValue( i ) );
      _variables.Print();

      if( _isNullObjective )
	      Console.WriteLine( "SATISFACTION run" );
      else
	      Console.WriteLine( "OPTIMIZATION run with objective " + _objective.Name );
      
      Console.WriteLine("Elapsed time: " + start.ElapsedMilliseconds
                      + "\nGlobal cost: " + bestGlobalCost
                      + "\nNumber of tours: " + tour
                      + "\nNumber of iterations: " + iterations );

      if( !_isNullObjective )
      {
	      Console.WriteLine( "Optimization cost: " + _bestCost
                         + "\nOpt Cost BEFORE post-processing: " + beforePostProc );
      }
      
      if( timerPostProcessOpt > 0 )
	      Console.WriteLine( "Post-processing time: " + timerPostProcessOpt ); 

      Console.WriteLine();
//#endif
      
      return _isNullObjective ? _bestCost : bestGlobalCost;
    }

    private void move( int variableId, int newValueIndex )
    {
      _variables.SetValue( variableId, _variables.PossibleValues( variableId )[ newValueIndex ] );
    }

    private void permut( int variableId, int newValueIndex )
    {
      int newValue = _variables.PossibleValues( variableId )[ newValueIndex ];

      if( _variables.GetValue( variableId ) == newValue )
        return;
    
      for( int i = 0 ; i < _numberVariables ; ++i )
        if( _variables.GetValue( i ) == newValue )
        {
          _variables.Swap( variableId, i );
          return;
        }
    }
  }
}