using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;






namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int input = 0, isFinished = 1;
            bool isAdmin = false;
            string userName="";
            HttpClient client = new HttpClient();

            Console.WriteLine("Welcome to Internet Shop!");
            
            while(input!=1 || input != 2)
            {
                Console.WriteLine("Press 1 to login or 2 to create account.");
                input = Convert.ToInt32(Console.ReadLine());
                if (input == 1)
                {
                    Console.WriteLine("Enter your user name:");
                    userName = Console.ReadLine();

                    HttpRequestMessage request = new HttpRequestMessage
                    {
                        Content = JsonContent.Create(userName),
                        Method = HttpMethod.Post,
                        RequestUri = new Uri("https://localhost:5001/api/User/login")
                    };
                    var res = client.Send(request);
                    var logged = JsonConvert.DeserializeObject<LoggedUser>( res.Content.ReadAsStringAsync().Result);
                    if(logged.Index == 0)
                    {
                        Console.WriteLine("We can't find your account... Try to register.");
                    }
                    else if(logged.Index == 1)
                    {
                        Console.WriteLine($"{userName} welcome to the shop! Your balance {logged.Balance}");
                        break;
                    }
                    else if(logged.Index == 2)
                    {
                        Console.WriteLine($"{userName} welcome to the shop! Your balance {logged.Balance}");
                        isAdmin = true;
                        break;
                    }
                }
                else if(input == 2)
                {
                    Console.WriteLine("Enter your user name:");
                    userName = Console.ReadLine();
                    HttpRequestMessage request = new HttpRequestMessage
                    {
                        Content = JsonContent.Create(new
                        {
                            Name = userName,
                            Balance = 100
                        }),
                        Method = HttpMethod.Post,
                        RequestUri = new Uri("https://localhost:5001/api/User/register")
                    };
                    client.Send(request);
                    Console.WriteLine("Your account was created! Try to login now!");
                }
                else
                {
                    Console.WriteLine("Anknown command!");
                }
            }
            while (isFinished != 0 )
            {
                Console.WriteLine("Choose action:\n" +
                    "0 - Finish working\n" +
                    "1 - Show all products\n" +
                    "2 - Buy product by name\n" +
                    "3 - Top up balance\n" +
                    "4 - History\n");
                if(isAdmin)
                    Console.WriteLine("5 - Delete product by name\n" +
                        "6 - Edit product by id\n");

                input = Convert.ToInt32(Console.ReadLine());
                if (input == 0)
                    break;
                else if(input == 1)
                {
                    var json = client.GetStringAsync("https://localhost:5001/api/Product/all").Result;
                    var list = JsonConvert.DeserializeObject<List<Product>>(json);
                    var report = new System.Text.StringBuilder();
                    report.AppendLine(
                       "____________________________________________");
                    report.AppendLine(
                        "    ID    Name          Price    Amount");
                    report.AppendLine(
                        "--------------------------------------------");
                    Console.WriteLine(report.ToString());
                    foreach (var item in list)
                    {
                        // Console.WriteLine("-------------------------------------");
                        Console.WriteLine(item);
                    }
                }
                else if(input == 2)
                {
                    HttpRequestMessage requestBalance = new HttpRequestMessage
                    {
                        Content = JsonContent.Create(userName),
                        Method = HttpMethod.Post,
                        RequestUri = new Uri("https://localhost:5001/api/User/login")
                    };
                    var res = client.Send(requestBalance);
                    var logged = JsonConvert.DeserializeObject<LoggedUser>(res.Content.ReadAsStringAsync().Result);
                    Console.WriteLine($"Your balance now:{logged.Balance}");
                    int indx = 0;
                    string product;
                    int amount = 0;
                    Console.WriteLine("Enter product name:");
                    product = Console.ReadLine();
                    Console.WriteLine("Enter product amount:");
                    bool state;
                    do
                    {
                        Console.WriteLine("Must be more zero and it`s only interation number");
                        state = false;
                        try
                        {
                            amount = int.Parse(Console.ReadLine());
                            if(amount < 1) { state = true; }
                        }
                        catch
                        {
                            //amount = int.Parse(Console.ReadLine());
                            Console.WriteLine("eroor");
                            state = true;
                            Console.WriteLine(state);
                        }
                    } while (state == true);
                    var json = client.GetStringAsync("https://localhost:5001/api/Product/all").Result;
                    var list = JsonConvert.DeserializeObject<List<Product>>(json);

                    foreach (var item in list)
                    {
                        if (item.Name == product)
                        {
                            break;
                        }


                        indx++;

                    }
                    if (indx < list.Count)
                    {
                        if (list[indx].Amount != 0)
                        {
                            if (list[indx].Price * amount <= logged.Balance)
                            {

                                HttpRequestMessage request = new HttpRequestMessage
                                {
                                    Content = JsonContent.Create(new
                                    {
                                        UserName = userName,
                                        ProductName = product,
                                        Amount = amount


                                    }),
                                    Method = HttpMethod.Post,
                                    RequestUri = new Uri("https://localhost:5001/api/Product/buy")
                                };
                                client.Send(request);
                                HttpRequestMessage requestBal = new HttpRequestMessage
                                {
                                    Content = JsonContent.Create(new
                                    {
                                        Name = userName,
                                        Balance = logged.Balance - list[indx].Price * amount
                                    }),
                                    Method = HttpMethod.Put,
                                    RequestUri = new Uri("https://localhost:5001/api/User/balance")
                                };
                                client.Send(requestBal);
                                Console.WriteLine($"You buy {product} and your balance {logged.Balance - list[indx].Price * amount}");
                            }
                            else
                            {
                                Console.WriteLine("You don`t have enough money");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Product out of stock");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Shop doesn`t have product this name {product}");
                    }
                }
                else if(input == 3)
                {
                    double balance;
                    HttpRequestMessage requestBalance = new HttpRequestMessage
                    {
                        Content = JsonContent.Create(userName),
                        Method = HttpMethod.Post,
                        RequestUri = new Uri("https://localhost:5001/api/User/login")
                    };
                    var res = client.Send(requestBalance);
                    var logged = JsonConvert.DeserializeObject<LoggedUser>(res.Content.ReadAsStringAsync().Result);
                    Console.WriteLine($"Your balance now:{logged.Balance}");

                    Console.WriteLine("Enter new balance:");
                    balance = Convert.ToDouble(Console.ReadLine());


                    HttpRequestMessage request = new HttpRequestMessage
                    {
                        Content = JsonContent.Create(new
                        {
                            Name = userName,
                            Balance = logged.Balance + balance
                        }),
                        Method = HttpMethod.Put,
                        RequestUri = new Uri("https://localhost:5001/api/User/balance")
                    };
                    Console.WriteLine($"Your updated balance:{logged.Balance + balance}");
                    client.Send(request);
                }
                else if(input == 4)
                {
                    var json = client.GetStringAsync($"https://localhost:5001/api/Product/user?name={userName}").Result;
                    var list = JsonConvert.DeserializeObject<List<Product>>(json);
                    var report = new System.Text.StringBuilder();
                   
                    report.AppendLine(
                        "____________________________________________");
                    report.AppendLine(
                         "    ID    Name          Price     Amount");
                    report.AppendLine(
                        "--------------------------------------------");
                    Console.WriteLine(report.ToString());
                    foreach (var item in list)
                    {
                        //Console.WriteLine("-------------------------------------");
                        Console.WriteLine($"{item}  ");
                    }
                }
                else if(isAdmin && input == 5)
                {
                    string product;
                    int id;
                    Console.WriteLine("Enter product name:");
                    product = Console.ReadLine();

                    HttpRequestMessage request = new HttpRequestMessage
                    {
                        Content = JsonContent.Create(product),
                        Method = HttpMethod.Delete,
                        RequestUri = new Uri("https://localhost:5001/api/Admin")
                    };
                    client.Send(request);
                }
                else if(isAdmin && input == 6)
                {
                    string product;
                    int id, amount;
                    double price;
                    Console.WriteLine("Enter product id:");
                    id = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter product name:");
                    product = Console.ReadLine();
                    Console.WriteLine("Enter product price");
                    price = Convert.ToDouble(Console.ReadLine());
                    Console.WriteLine("Enter product amount");
                    amount = Convert.ToInt32(Console.ReadLine());

                    HttpRequestMessage request = new HttpRequestMessage
                    {
                        Content = JsonContent.Create(new
                        {
                            Id = id,
                            Name = product,
                            Price = price,
                            Amount = amount
                        }),
                        Method = HttpMethod.Put,
                        RequestUri = new Uri("https://localhost:5001/api/Admin")
                    };
                    client.Send(request);
                }
            }
        }
    }
    class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Amount { get; set; }
        public override string ToString()
        {

            var report = new System.Text.StringBuilder();
            //report.AppendLine();



            report.AppendLine(
                    $"{Id,5}     {Name,-14} {Price,-10} {Amount,-10}");
            report.AppendLine(
                "--------------------------------------------");

            return report.ToString();
            //return "| " + Id + " | " + Name + " | " + Price + "|\n" ; 
        }
       
    }
    public class LoggedUser
    {
        public double Balance { get; set; }
        public int Index { get; set; }
    }
}
