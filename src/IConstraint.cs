using System.Collections.Generic;

namespace ghost
{
  public interface IConstraint<out TypeSetVariables> where TypeSetVariables : ISetVariables<IVariable>
  {
    double Cost( List<double> variableCost );
    Dictionary<int, double> SimulateCost( int currentVariableIndex,
                                          Dictionary< int, List<double> > variableSimCost,
                                         Objective< TypeSetVariables, IVariable > objective );
    Dictionary<int, double> SimulateCost( int currentVariableIndex,
                                         Dictionary< int, List<double> > variableSimCost );
  }
}

