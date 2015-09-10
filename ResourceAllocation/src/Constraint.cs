using System;
using System.Collections.Generic;
using System.Linq;

namespace RA
{
  public abstract class Constraint : ghost.Constraint<SetVariables, Variable>
  {
    protected Constraint( SetVariables variables, double resource ) 
        : base( variables ) 
    { 
      Resource = resource;
      Data = new Data();
    }

    protected double Cost( double[] variableCost, string resourceType )
    {
      var costs = new double[ Variables.GetNumberVariables() ];
      for( int i = 0 ; i < Variables.GetNumberVariables() ; ++i )
      {
        switch( resourceType )
        { 
          case "mineral":
            costs[ i ] += Variables.GetValue( i ) * Data.Dataset[ Variables.Name( i ) ].CostMineral;
            break;
          case "gas":
            costs[ i ] += Variables.GetValue( i ) * Data.Dataset[ Variables.Name( i ) ].CostGas;
            break;
          case "supply":
            costs[ i ] += Variables.GetValue( i ) * Data.Dataset[ Variables.Name( i ) ].CostSupply;
            break;
        }
      }

      double total = costs.Sum();

      if( total <= Resource )
        return 0;
      else
      {
        double surplus = total - Resource;
        for( int i = 0 ; i < Variables.GetNumberVariables() ; ++i )
          variableCost[ i ] = costs[ i ] >= surplus ? costs[ i ] : 0;
      }

      return total;
    }

    protected Dictionary<int, double> SimulateCost( int currentVariableIndex,
                                                    Dictionary< int, double[] > variableSimCost,
                                                    string resourceType )
    {
      // for each value in currentVariableIndex's domain, save the constraint cost value.
      var simCosts = new Dictionary<int, double>();

      int backup = Variables.GetValue( currentVariableIndex );

      foreach( var pos in Variables.PossibleValues( currentVariableIndex ) )
      {
        Variables.SetValue( currentVariableIndex, pos );
        simCosts[ pos ] = Cost( variableSimCost[ pos ], resourceType );
      }

      Variables.SetValue( currentVariableIndex, backup );
      return simCosts;
    }


    public Data Data { get; private set;}
    public double Resource { get; private set; }
  }


  /***********/
  /* Mineral */
  /***********/  
  public class MineralConstraint : Constraint
  {
    public MineralConstraint( SetVariables variables, double resource ) : base( variables, resource ) {}

    override public double Cost( double[] variableCost )
    {
      return Cost( variableCost, "mineral" );
    }

    override public Dictionary<int, double> SimulateCost( int currentVariableIndex,
                                                 Dictionary< int, double[] > variableSimCost )
    {
      return SimulateCost( currentVariableIndex, variableSimCost, "mineral" );
    }
  }


  /*******/
  /* Gas */
  /*******/  
  public class GasConstraint : Constraint
  {
    public GasConstraint( SetVariables variables, double resource ) : base( variables, resource ) {}

    override public double Cost( double[] variableCost )
    {
      return Cost( variableCost, "gas" );
    }

    override public Dictionary<int, double> SimulateCost( int currentVariableIndex,
                                                Dictionary< int, double[] > variableSimCost )
    {
      return SimulateCost( currentVariableIndex, variableSimCost, "gas" );
    }
  }


  /**********/
  /* Supply */
  /**********/  
  public class SupplyConstraint : Constraint
  {
    public SupplyConstraint( SetVariables variables, double resource ) : base( variables, resource ) {}

    override public double Cost( double[] variableCost )
    {
      return Cost( variableCost, "supply" );
    }

    override public Dictionary<int, double> SimulateCost( int currentVariableIndex,
                                                Dictionary< int, double[] > variableSimCost )
    {
      return SimulateCost( currentVariableIndex, variableSimCost, "supply" );
    }
  }

}

