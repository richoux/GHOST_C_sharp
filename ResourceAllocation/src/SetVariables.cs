using System;
using System.Collections.Generic;

namespace RA
{
  public class SetVariables : ghost.SetVariables<Variable>
  {
    public SetVariables( List<Variable> variables ) : base( variables ) { }
  }
}

