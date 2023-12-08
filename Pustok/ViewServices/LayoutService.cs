using Pustok.Business.Services.Interfaces;
using Pustok.Core.Models;
using Pustok.DAL;
using Pustok.Models;

namespace Pustok.ViewServices
{
    public class LayoutService
    {
        private readonly PustokContext _context;
        public LayoutService( PustokContext context)
        {
            _context = context;
        }

        public async Task<List<Setting>> GetBook()
        {
            var settings = _context.Settings.ToList();
            return settings;
        }

    }
}
