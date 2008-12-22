using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotGit.Config
{
  /// <summary>
  /// Enumeration used for easy access to Git configuration file keys. The lower case value in the enum is used as the key in the config file. (e.g. IgnoreCase => ignorecase)
  /// </summary>
  public enum ConfigKeys
  {
    RepositoryFormatVersion,
    IgnoreCase,
    Bare,
    LogAllRefUpdates,
    SymLinks,
    FileMode,

    Name,
    Email,

    Url,
    Ref
  }
}
