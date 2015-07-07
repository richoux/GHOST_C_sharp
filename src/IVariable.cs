namespace ghost
{
  public interface IVariable
  {
    void ResetDomain();
    void ShiftValue();
    void UnshiftValue();
    int GetValue();
    void SetValue( int value );
    Domain Domain { get; set; }
    int IndexDomain { get; set; }
  }
}

