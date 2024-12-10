using MetaApi.Models.VirtualFit;
using MetaApi.SqlServer.Context;
using MetaApi.Utilities;
using System.Text.Json;
using System.Text;
using MetaApi.Core.OperationResults.Base;
using MetaApi.Core.OperationResults;
using MetaApi.SqlServer.Entities.VirtualFit;
using Microsoft.EntityFrameworkCore;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {
        private readonly MetaDbContext _metaDbContext;
        private readonly IHttpClientFactory _httpClientFactory;

        public VirtualFitService(MetaDbContext metaContext,
                                 IHttpClientFactory httpClientFactory)
        {
            _metaDbContext = metaContext;
            _httpClientFactory = httpClientFactory;
        }

    }
}
