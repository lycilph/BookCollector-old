using System.Collections;
using BookCollector.Data;

namespace BookCollector.Goodreads
{
    public class PriorityQueueItem
    {
        public QueueAction Action { get; set; }
        public Book Book { get; set; }
        public Book Parent { get; set; }
    }
}
