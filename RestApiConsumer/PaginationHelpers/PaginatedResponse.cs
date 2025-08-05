using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiConsumer;

public class PaginatedResponse<T>
{
    public PaginationMetadata? Pagination { get; set; } // Metadata info for pagination
    public List<T>? Data { get; set; } // Actual paginated data
}

