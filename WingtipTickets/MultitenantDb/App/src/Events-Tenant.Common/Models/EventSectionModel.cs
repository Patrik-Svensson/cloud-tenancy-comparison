﻿namespace Events_Tenant.Common.Models
{
    public class EventSectionModel
    {
        public int EventId { get; set; }
        public int SectionId { get; set; }
        public decimal Price { get; set; }
        public int VenueId { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
