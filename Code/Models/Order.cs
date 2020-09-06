namespace Bingo.Models
{
    /// <summary>
    /// 订单实体
    /// </summary>
    public class Order: BaseModel
    {
        /// <summary>
        /// 购买人姓名
        /// </summary>
        public string BuyerName { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string PurchaseOrderNumber { get; set; }

        /// <summary>
        /// ZipCode
        /// </summary>
        public string BillingZipCode { get; set; }

        /// <summary>
        /// 订购数量
        /// </summary>
        public decimal? OrderAmount { get; set; }
    }
}
