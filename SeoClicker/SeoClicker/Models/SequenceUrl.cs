using System;

namespace SeoClicker.Models
{
    public class SequenceUrl
    {
        public Guid SequenceID { get; set; }
        public string URL { get; set; }
        public string Device { get; set; }
        public string Country { get; set; }
        public Guid? UserID { get; set; }
        public DateTime Date { get; set; }
        public bool IsProcessed { get; set; }
    }
}
