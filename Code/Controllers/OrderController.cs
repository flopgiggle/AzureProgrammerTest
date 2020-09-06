using Bingo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bingo.Controllers
{


    /// <summary>
    /// 订单服务
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly Db _context = new Db();



        /// <summary>
        /// 创建订单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("PostOrder")]
        public async Task<ActionResult<long>> PostOrder(Order order)
        {
            //step.1 If any field is missing, reject it with a http status code 400.
            //这里也可以使用模型验证，在模型中使用注解，ModelState.IsValid来进行验证。这里为了代码直观，直接在下面代码验证
            if (string.IsNullOrWhiteSpace(order.PurchaseOrderNumber) || string.IsNullOrWhiteSpace(order.BuyerName) ||
                string.IsNullOrWhiteSpace(order.BillingZipCode) || order.OrderAmount == 0)
            {
                return BadRequest("订单字段不能为空");
            }

            //step.2 If PurchaseOrderNumber exists in the database, return http status code 204
            if (_context.Order.Any(a => a.PurchaseOrderNumber.Contains(order.PurchaseOrderNumber)))
            {
                //return http status code 204
                return NotFound("订单号不能重复");
            }

            _context.Add(order);

            await _context.SaveChangesAsync();

            return Ok(order.Id);
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetOrder")]
        public async Task<ActionResult<List<Order>>> GetOrder([FromQuery]Order order)
        {
            var queryCondition = _context.Order.Where(a=>true);

            if (!string.IsNullOrEmpty(order.PurchaseOrderNumber))
            {
                queryCondition = queryCondition.Where(a => a.PurchaseOrderNumber == order.PurchaseOrderNumber);
            }

            if (!string.IsNullOrEmpty(order.BillingZipCode))
            {
                queryCondition = queryCondition.Where(a => a.BillingZipCode == order.BillingZipCode);
            }

            if (!string.IsNullOrEmpty(order.BuyerName))
            {
                queryCondition = queryCondition.Where(a => a.BuyerName.Contains(order.BuyerName));
            }

            if (order.OrderAmount!=null)
            {
                queryCondition = queryCondition.Where(a => a.OrderAmount == order.OrderAmount);
            }

            //此处，在实际操作中，应该使用auto maper之类工具进行数据库实体到，显示层dto的转换
            var result = await queryCondition.ToListAsync();

            return Ok(result);
        }
    }
}
