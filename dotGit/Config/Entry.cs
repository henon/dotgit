using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotGit.Config
{
    public class Entry
    {
        internal Entry(Section section, string name)
        {
            Section = section;
            Name = name;
        }

        public Section Section
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public string FullName
        {
            get
            {
                return Section.Name + "." + Name;
            }
        }

        public string Value
        {
            get { return Section.Configuration.GetValue<string>(Section.Name, Name); }
            set { Section.Configuration.SetValue(Section.Name, Name, value); }
        }
    }
}
