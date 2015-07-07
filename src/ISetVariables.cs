using System.Collections.Generic;

namespace ghost
{
  public interface ISetVariables<out TypeVariable> where TypeVariable : IVariable
  {
    int GetNumberVariables();
    int GetSizeAllDomains();
    int GetIndex( IVariable variable );
    bool IsInSet( IVariable variable );
    void Swap( int index1, int index2 );
    void ResetDomain( int index );
    void ResetAllDomains();
    void ShiftValue( int index ); 
    void UnshiftValue( int index );
    int GetValue( int index );
    void SetValue( int index, int value );
    string Name( int index );
    string FullName( int index );
    Domain Domain( int index );
    List<IVariable> Variables { get; set; }
  }
}

