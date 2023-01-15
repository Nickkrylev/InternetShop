 using InternetShop.DataBase.Models;
using InternetShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternetShop.DataBase
{
    public class DbRepository
    {
        private EfCoreDbContext GetContext()
        {
            return new EfCoreDbContext();
        }
        public async Task<List<Product>> GetProducts()
        {
            using(var ctx = GetContext())
            {
                return ctx.Products.ToList();
            }
        }
        public async Task<List<ProductBasket>> GetBasket()
        {
            using(var ctx = GetContext())
            {
                return ctx.Basket.ToList();
            }
        }

        public void UpdateProduct(Product product)
        {
            using (var ctx = GetContext())
            {
                var origin = ctx.Products.First(x => x.Id == product.Id);
                origin.Name = product.Name;
                origin.Price = product.Price;
                origin.Amount = product.Amount;
                ctx.SaveChanges();
            }
        }

        public int GetProductAmoun(string name)
        {
            using (var ctx = GetContext())
            {
                var product = ctx.Products.First(x => x.Name == name);
                return  product.Amount;
            }
        }

        public double GetPrice(string productName)
        {
            using(var ctx = GetContext())
            {
                var product = ctx.Products.First(x => x.Name == productName);
                return product.Price*product.Amount;
            }
        }

        public async Task AddProduct(Product product)
        {
            using( var ctx = GetContext())
            {
                ctx.Products.Add(product);
                await ctx.SaveChangesAsync();
            }
        }
        public async Task RemoveProduct(string product)
        {
            using( var ctx = GetContext())
            {
                var prod = ctx.Products.First(x => x.Name == product);
                ctx.Products.Remove(prod);
                ctx.Basket.Add(new ProductBasket
                {
                    Name = prod.Name,
                    Price = prod.Price
                });
                await ctx.SaveChangesAsync();
            }
        }
        public async Task AddUser(UserModel user)
        {
            using (var ctx = GetContext())
            {
                ctx.Accounts.Add(user);
                await ctx.SaveChangesAsync();
            }
        }
        public double GetBalance(string user)
        {
            using(var ctx = GetContext())
            {
                return ctx.Accounts.First(x => x.Name == user).Balance;
            }
        }
        public bool IsUserExist(string userName)
        {
            using(var ctx = GetContext())
            {
                return ctx.Accounts.Count(x => x.Name == userName) > 0;
            }
        }
        public async Task BuyProduct(BuyModel buyModel)
        {
            using(var ctx = GetContext())
            {
                var account = ctx.Accounts.First(x => x.Name == buyModel.UserName);

                var product = ctx.Products.First(x => x.Name == buyModel.ProductName);

                ctx.Purchases.Add(new PurchaseModel
                {
                    UserId = account.Id,
                    ProductId = product.Id,
                    Date = System.DateTime.Now,
                    Amount = buyModel.Amount                                  
                });
                product.Amount -= buyModel.Amount;
                account.Balance = account.Balance - product.Price*product.Amount;
                await ctx.SaveChangesAsync();
            }
        }
        public async Task<List<Product>> GetUserProducts(string userName)
        {
            using(var ctx = GetContext())
            {
                var accountId = ctx.Accounts.First(x => x.Name == userName).Id;

                return ctx.Purchases.Where(x => x.UserId == accountId).Join(ctx.Products, x => x.ProductId, y => y.Id, (x,y) => new Product
                {
                    Id = y.Id,
                    Name = y.Name,
                    Amount = x.Amount,
                    Price = y.Price
                }).ToList();
            }
        }
        public async Task EditBalance(User user)
        {
            using (var ctx = GetContext())
            {
                ctx.Accounts.First(x => x.Name == user.Name).Balance = user.Balance;
                await ctx.SaveChangesAsync();
            }
        }
    }
}
