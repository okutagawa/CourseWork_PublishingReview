using Microsoft.AspNetCore.Mvc;
using PublishingReviewApp.DTOs;
using PublishingReviewApp.Services;
namespace PublishingReviewApp.Controllers;
public class ProductsController:Controller{private readonly ProductService _products; private readonly CategoryService _categories; public ProductsController(ProductService p,CategoryService c){_products=p;_categories=c;}
public async Task<IActionResult> Index(string? q,int? category,decimal? minPrice,decimal? maxPrice,double? minRating){ViewBag.Categories=await _categories.GetAll(); return View(await _products.Search(q,category,minPrice,maxPrice,minRating));}
public async Task<IActionResult> Details(int id){var p=await _products.Get(id); if(p==null) return NotFound(); return View(p);} public async Task<IActionResult> Create(){ViewBag.Categories=await _categories.GetAll(); return View(new ProductCreateDto());}
[HttpPost] public async Task<IActionResult> Create(ProductCreateDto d){if(!ModelState.IsValid){ViewBag.Categories=await _categories.GetAll(); return View(d);} try{await _products.Create(d); TempData["Success"]="Товар создан"; return RedirectToAction(nameof(Index));} catch(Exception ex){ModelState.AddModelError("",ex.Message);ViewBag.Categories=await _categories.GetAll(); return View(d);}}
public async Task<IActionResult> Edit(int id){var p=await _products.Get(id); if(p==null)return NotFound(); ViewBag.Categories=await _categories.GetAll(); return View(new ProductCreateDto{Id=p.Id,Name=p.Name,Description=p.Description,Price=p.Price,ImageUrl=p.ImageUrl,CategoryIds=p.CategoryIds});}
[HttpPost] public async Task<IActionResult> Edit(ProductCreateDto d){if(!ModelState.IsValid){ViewBag.Categories=await _categories.GetAll();return View(d);} await _products.Update(d); TempData["Success"]="Товар обновлен"; return RedirectToAction(nameof(Index));}
public async Task<IActionResult> Delete(int id){var p=await _products.Get(id); if(p==null)return NotFound(); return View(p);} [HttpPost,ActionName("Delete")] public async Task<IActionResult> DeleteConfirmed(int id){await _products.Delete(id); TempData["Success"]="Товар удален"; return RedirectToAction(nameof(Index));}}
