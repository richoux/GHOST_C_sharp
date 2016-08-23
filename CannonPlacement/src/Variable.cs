namespace CannonPlacement
{
  public class Variable : ghost.Variable
  {
    public Variable(string name, 
                    string fullName,
                    double range,
                    int value = -1)
      : this( name, fullName, new ghost.Domain( -1 ), range, value ) 
    { }

    public Variable(string name, 
                    string fullName, 
                    ghost.Domain domain,
                    double range,
                    int value = -1)
      : base( name, fullName, domain, value )
    { 
      Range = range;
    }

    public bool IsSelected() { return IndexDomain >= 0; }

    public double Range { get; private set; }
  }
}
