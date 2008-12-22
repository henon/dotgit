using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotGit.Config
{
  /// <summary>
  /// Enumeration for easy access to the Git configuration file sections. The lower case enum value is used as the section name ( ConfigSections.Core ==> "core" )
  /// </summary>
  public enum ConfigSections
  {
    Core = 0,
    User = 1,
    Remote= 2,
    Branch = 3
  }
}
