using Microsoft.AspNetCore.Mvc;
using Pustok.Business.Services.Interfaces;
using Pustok.DAL;
using Pustok.ViewModels;

namespace Pustok.ViewComponents
{
    public class NavViewComponent : ViewComponent
    {
        private readonly IGenreService _genreService;
        private readonly PustokContext _context;
        public NavViewComponent(IGenreService genreService, PustokContext context)
        {
            _context = context;
            _genreService = genreService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            NavigationViewModel navigationViewModel = new NavigationViewModel();

            navigationViewModel.Genres = await _genreService.GetAllAsync();
            navigationViewModel.Settings = _context.Settings.ToList();
            return View(navigationViewModel);
        }
    }
}
