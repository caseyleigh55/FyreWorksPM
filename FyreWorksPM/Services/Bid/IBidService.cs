using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FyreWorksPM.Services.Bid
{
    public interface IBidService
    {
        Task<string> GetNextBidNumberAsync();
        Task<BidDto> CreateBidAsync(CreateBidDto dto);        

    }
}
