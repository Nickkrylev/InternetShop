using System;

namespace InternetShop.DataBase.Models
{
    public class PurchaseModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Amount { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
    }
}
