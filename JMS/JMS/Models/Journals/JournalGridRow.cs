using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JMS.Models.Journals
{
    public class JxGridModel<T>
    {
        public IEnumerable<T> data { get; set; }
        public long itemsCount { get; set; }
    }
    public class JournalGridRow
    {
        public long Id { get; set; }
        public string JournalName { get; set; }
        public string Path { get; set; }
        public bool Active { get; set; }
        public string Logo { get; set; }
        public string JournalTitle { get; set; }
    }
}
