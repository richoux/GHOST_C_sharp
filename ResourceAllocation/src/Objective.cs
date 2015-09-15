using System;
using System.Linq;
using System.Collections.Generic;

namespace RA
{
  public abstract class Objective : ghost.Objective< SetVariables, Variable >
  {
    protected Objective( string name, int mineral, int gas, double supply ) 
      : base( name ) 
    { 
      Data = new Data();
      Mineral = mineral;
      Gas = gas;
      Supply = supply;
    }

    public Data Data { get; private set; }
    public int Mineral { get; private set; }
    public int Gas { get; private set; }
    public double Supply { get; private set; }
  }


  /**********/
  /* MaxDPS */
  /**********/  
  public class MaxDPS : Objective
  {
    public MaxDPS( string name, int mineral, int gas, double supply ) : base( name, mineral, gas, supply ) { }

    override public double Cost( SetVariables variables )
    {
      var costs = new double[ variables.GetNumberVariables() ];

      for( int i = 0 ; i < variables.GetNumberVariables() ; ++i )
        if( Data.Dataset[ variables.Name( i ) ].GroundAttack > 0 )
          costs[ i ] = variables.GetValue( i ) * ( (double)Data.Dataset[ variables.Name( i ) ].GroundAttack / Data.Dataset[ variables.Name( i ) ].GroundCooldown );

      var total = costs.Sum();
      return total > 0 ? -total : double.MaxValue;
    }

    override public int HeuristicValue( List<int> valuesIndex, int variableIndex, SetVariables variables )
    {
      double minValue = double.MaxValue;
      int minIndex = -1;
      double tmp;

      int backup = variables.GetValue( variableIndex );

      for( int i = 0 ; i < valuesIndex.Count ; ++i )
      {
        variables.SetValue( variableIndex, variables.Domain( variableIndex ).GetValue( valuesIndex[ i ] ) );
        tmp = Cost( variables );
        if( minValue > tmp )
        {
          minValue = tmp;
          minIndex = i;
        }
      }

      // a priori not necessary, but just in case...
      variables.SetValue( variableIndex, backup );

      // Returns the INDEX of a possible value in valuesIndex, not the value itself!
      return minIndex == -1 ? valuesIndex[ Random.Next( 0, valuesIndex.Count ) ] : valuesIndex[ minIndex ];
    }

  }
}

