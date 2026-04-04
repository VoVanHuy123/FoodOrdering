using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodOrdering.Context;
using FoodOrdering.DTOs;
using FoodOrdering.Models;
using FoodOrdering.services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Controllers
{
    public class OrdersController : Controller
    {
        private readonly FoodOrderingContext _context;
        private readonly IOrdersService _orderService;

        public OrdersController(
            FoodOrderingContext context,
            IOrdersService orderService)
        {
            _context = context;
            _orderService = orderService;
        }

        // GET: Orders
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(OrderQuery query)
        {
            var result = await _orderService.GetAllAsync(query);

            ViewBag.Query = query;

            return View(result);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetByIdAsync(id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {
            ViewData["TableId"] =
        new SelectList(_context.Tables, "Id", "TableNumber");

            ViewBag.MenuItems =
                await _context.MenuItems.Where(m => m.IsAvailable).ToListAsync();

            return View(new OrderEditDTO
            {
                OrderTime = DateTime.Now
            });
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderEditDTO dto)
        {
            var order = new Orders
            {
                TableId = dto.TableId,
                OrderTime = DateTime.Now,
                Status = dto.Status,
                Note = dto.Note,
                OrderItems = dto.Items.Select(i => new OrderItems
                {
                    MenuItemId = i.MenuItemId,
                    
                    Quantity = i.Quantity,
                    Price = _context.MenuItems.First(m => m.Id == i.MenuItemId).Price
                }).ToList()
            };

            order.TotalAmount = order.OrderItems.Sum(i => i.Price * i.Quantity);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _orderService.GetEditAsync(id);

            if (order == null)
                return NotFound();

            ViewData["TableId"] =
                new SelectList(_context.Tables, "Id", "Id", order.TableId);

            return View(order);
        }

        //POST: Orders/Edit/
        [HttpPost]
        public async Task<IActionResult> Edit(OrderEditDTO dto)
        {
            await _orderService.UpdateAsync(dto);

            return RedirectToAction(nameof(Details), new { id = dto.Id });
        }

        //}

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _orderService.GetByIdAsync(id.Value);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var orders = await _context.Orders.FindAsync(id);
            //if (orders != null)
            //{
            //    _context.Orders.Remove(orders);
            //}
            bool is_delete = await _orderService.DeleteAsync(id);
            if(is_delete == true)
            {
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool OrdersExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
