using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Common.DTOs.Merit
{
    public class MeritScoreHistoryDto
    {
        public decimal Score { get; set; }
        public DateTime Timestamp { get; set; }
        public string Reason { get; set; }
    }
}
