using System;
using System.Linq;

namespace RA
{
  public abstract class Objective : ghost.Objective< SetVariables, Variable >
  {
    protected Objective( string name ) 
      : base( name ) 
    { 
      Data = new Data();
    }

    public Data Data { get; private set;}
  }


  /**********/
  /* MaxDPS */
  /**********/  
  public class MaxDPS : Objective
  {
    public MaxDPS( string name ) : base( name ) { }

    override public double Cost( SetVariables variables )
    {
      var costs = new double[ variables.GetNumberVariables() ];

      for( int i = 0 ; i < variables.GetNumberVariables() ; ++i )
        costs[ i ] += variables.GetValue( i ) * ( Data.Dataset[ variables.Name( i ) ].GroundAttack / Data.Dataset[ variables.Name( i ) ].GroundCooldown );

      return 1 / costs.Sum();
    }
  }
}

