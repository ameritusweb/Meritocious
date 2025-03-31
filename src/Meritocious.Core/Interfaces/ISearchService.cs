using Meritocious.Common.Enums;
using Meritocious.Core.Features.Search.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Interfaces
{
    public interface ISearchService
    {
        Task<SearchResultDto> SearchAsync(
            string searchTerm,
            List<ContentType> contentTypes,
            Dictionary<string, string> filters,
            int page = 1,
            int pageSize = 20);
    }
}
