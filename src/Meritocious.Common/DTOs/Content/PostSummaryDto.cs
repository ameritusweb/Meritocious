using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Common.DTOs.Content
{
    public class PostSummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string AuthorUsername { get; set; }
        public decimal MeritScore { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
