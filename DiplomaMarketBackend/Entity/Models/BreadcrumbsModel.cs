﻿namespace DiplomaMarketBackend.Entity.Models
{
    public class BreadcrumbsModel
    {
        public int Id { get; set; }

        public int? TitleId { get; set; }
        public TextContent? Title { get; set; }

        public string href { get; set; }
    }
}
