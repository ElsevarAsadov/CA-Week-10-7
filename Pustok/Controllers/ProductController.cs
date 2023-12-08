using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pustok.Business.Services.Interfaces;
using Pustok.Models;
using Pustok.Repositories.Interfaces;
using Pustok.ViewModels;

namespace Pustok.Controllers;

public class ProductController : Controller
{
    private readonly IBookService _bookService;
    private readonly IBookRepository _bookRepository;

    public ProductController(IBookRepository bookRepository, IBookService bookService)
    {
        _bookRepository = bookRepository;
        _bookService = bookService;
    }
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Detail(int id)
    {
        Book book = await _bookService.GetByIdAsync(id);
        ProductDetailViewModel productDetailViewModel = new ProductDetailViewModel()
        {
            Book = book,
            RelatedBooks = await _bookService.GetAllRelatedBooksAsync(book)
        };

        return View(productDetailViewModel);
    }

    public async Task<IActionResult> GetBookModal(int id)
    {
        var book = await _bookService.GetByIdAsync(id);

        return PartialView("_BookModalPartial",book);
    }

    public IActionResult AddToBasket(int bookId)
    {

        if (!_bookRepository.Table.Any(x => x.Id == bookId)) return NotFound(); // 404

        List<BasketProductViewModel> basketItemList = new List<BasketProductViewModel>();
        BasketProductViewModel basketProduct = null;
        string basketItemListStr = HttpContext.Request.Cookies["BasketItems"];

        if (basketItemListStr != null)
        {
            basketItemList = JsonConvert.DeserializeObject<List<BasketProductViewModel>>(basketItemListStr);

            basketProduct = basketItemList.FirstOrDefault(x => x.BookId == bookId);

            if (basketProduct != null)
            {
                basketProduct.Count++;
            }
            else
            {
                basketProduct = new BasketProductViewModel()
                {
                    BookId = bookId,
                    Count = 1
                };

                basketItemList.Add(basketProduct);
            }
        }
        else
        {
            basketProduct = new BasketProductViewModel()
            {
                BookId = bookId,
                Count = 1
            };

            basketItemList.Add(basketProduct);
        }

        basketItemListStr = JsonConvert.SerializeObject(basketItemList);

        HttpContext.Response.Cookies.Append("BasketItems", basketItemListStr);

        return Ok(); //200
    }

    public IActionResult GetBasketItems()
    {
        List<BasketProductViewModel> basketItemList = new List<BasketProductViewModel>();

        string basketItemListStr = HttpContext.Request.Cookies["BasketItems"];

        if (basketItemListStr != null)
        {
            basketItemList = JsonConvert.DeserializeObject<List<BasketProductViewModel>>(basketItemListStr);
        }

        return Json(basketItemList);
    }

    public async Task<IActionResult> Checkout()
    {
        List<CheckoutViewModel> checkoutItemList = new List<CheckoutViewModel>();
        List<BasketProductViewModel> basketItemList = new List<BasketProductViewModel>();
        CheckoutViewModel checkoutItem = null;

        string basketItemListStr = HttpContext.Request.Cookies["BasketItems"];
        if (basketItemListStr != null)
        {
            basketItemList = JsonConvert.DeserializeObject<List<BasketProductViewModel>>(basketItemListStr);

            foreach (var item in basketItemList)
            {
                checkoutItem = new CheckoutViewModel
                {
                    Book = await _bookRepository.GetByIdAsync(x => x.Id == item.BookId),
                    Count = item.Count
                };
                checkoutItemList.Add(checkoutItem);
            }
        }

        return View(checkoutItemList);
    }

}
