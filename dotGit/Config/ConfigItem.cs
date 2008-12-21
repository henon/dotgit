using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotGit.Config
{
  /// <summary>
  /// Represents an entry in Git configuration (global or repository scoped). The Global property indicates where the value was retrieved or where it is supposed to go.
  /// </summary>
  /// <typeparam name="T">Type of configuration value</typeparam>
  /// <example>ConfigItem<bool> Bare;</example>
  public struct ConfigItem<T> 
  {
    internal bool Global { get; private set; }
    public T Value
    {
      get;
      private set;
    }

    public static implicit operator ConfigItem<T>(T value)
    {
      return new ConfigItem<T>(value);
    }

    public static implicit operator T (ConfigItem<T> obj)
    {
      return obj.Value;
    }


    private ConfigItem(T value)  :this()
    {
      Value = value;
    }

    public override int GetHashCode()
    {
      return Value.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      return Value.Equals(obj);
    }

    public override string ToString()
    {
      return Value.ToString();
    }
  
  }
}
