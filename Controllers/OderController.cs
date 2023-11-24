using ExamAPI.DTOs;
using ExamAPI.Entities;
using ExamAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly DataContext _context;
        public OrderController(DataContext context)
        {
            _context = context;
        }
        [HttpGet("get-all")]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                List<Orders> orders = await _context.Orders.OrderByDescending(s => s.OrderId).ToListAsync();
                List<OrderDTO> orderDTOs = new List<OrderDTO>();
                foreach (var order in orders)
                {
                    orderDTOs.Add(new OrderDTO
                    {
                        OrderId = order.OrderId,
                        ItemCode = order.ItemCode,
                        ItemName = order.ItemName,
                        ItemQty = order.ItemQty,
                        OrderDelivery = order.OrderDelivery,
                        OrderAddress = order.OrderAddress,
                        PhoneNumber = order.PhoneNumber,
                    });
                }
                return Ok(orderDTOs);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateOrder(CreateOrder model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Orders data = new Orders
                    {
                        ItemCode = model.ItemCode,
                        ItemName = model.ItemName,
                        ItemQty = model.ItemQty,
                        OrderDelivery = model.OrderDelivery,
                        OrderAddress = model.OrderAddress,
                        PhoneNumber = model.PhoneNumber,
                    };

                    _context.Orders.Add(data);
                    await _context.SaveChangesAsync();

                    return Created($"get-by-id?id={data.OrderId}", new OrderDTO
                    {
                        OrderId = data.OrderId,
                        ItemCode = data.ItemCode,
                        ItemName = data.ItemName,
                        ItemQty = data.ItemQty,
                        OrderDelivery = data.OrderDelivery,
                        OrderAddress = data.OrderAddress,
                        PhoneNumber = data.PhoneNumber,
                    });
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            var msgs = ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage);
            return BadRequest(string.Join(" | ", msgs));
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateOrder(EditOrder model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Orders orderExist = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(e => e.OrderId == model.Id);

                    if (orderExist != null)
                    {
                        Orders data = new Orders
                        {
                            OrderId = model.Id,
                            ItemCode = orderExist.ItemCode,
                            ItemName = orderExist.ItemName,
                            ItemQty = orderExist.ItemQty,
                            OrderDelivery = model.OrderDelivery,
                            OrderAddress = model.OrderAddress,
                            PhoneNumber = orderExist.PhoneNumber,
                        };

                        if (data != null)
                        {
                            _context.Orders.Update(data);
                            await _context.SaveChangesAsync();
                            return NoContent();
                        }

                        return NotFound();
                    }
                    else
                    {
                        return NotFound(); // Không tìm thấy lớp để cập nhật
                    }



                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }

            }
            return BadRequest();
        }
    }
}