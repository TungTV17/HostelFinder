using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Domain.Entities;
using HostelFinder.Infrastructure.Common;
using HostelFinder.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostelFinder.Infrastructure.Repositories
{
    public class WishlistPostRepository : BaseGenericRepository<WishlistPost>, IWishlistPostRepository
    {
        public WishlistPostRepository(HostelFinderDbContext dbContext) : base(dbContext)
        {
        }
    }
}
