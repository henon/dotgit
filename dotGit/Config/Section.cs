using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace dotGit.Config
{
    public class Section
    {
        internal Section(Configuration config, string name, NameValueCollection entries)
        {
            Configuration = config;
            Name = name;
            foreach (var key in entries.Keys)
            {
                m_entries[key as string] = new Entry(this, key as string);
            }
        }
        internal Dictionary<string, Entry> m_entries = new Dictionary<string, Entry>();

        public string Name { get; private set; }

        public IEnumerable<Entry> Entries
        {
            get
            {
                return m_entries.Values;
            }
        }

        public Configuration Configuration
        {
            get;
            private set;
        }
    }
}
