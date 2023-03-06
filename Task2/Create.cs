using Dapper;
using System.Data;
using System.Data.SqlClient;
using Task2.Models;

namespace Task2
{
    public class DataAccess
    {

        public bool CreateCategory(Categories category)
        {

            using (IDbConnection db = DbConnectionFactory.CreateConnection())
            {
                var categoryExists = CategoryExists(category.CategoryName);
                if (categoryExists)
                {
                    return false;
                }
                else
                {
                    db.Execute("INSERT INTO Categories (CategoryName, Description, Picture) VALUES (@CategoryName, @Description, @Picture)", category);
                    return true;

                }

            }
        }

        public bool CreateProduct(Products product)
        {

            using (IDbConnection db = DbConnectionFactory.CreateConnection())
            {
                string insertQuery = @"INSERT INTO Products 
                                    (ProductName, SupplierId, CategoryId, QuantityPerUnit, UnitPrice, UnitsInStock, 
                                     UnitsOnOrder, ReorderLevel, Discontinued, LastUserId, LastDateUpdated)
                                    VALUES 
                                    (@ProductName, @SupplierId, @CategoryId, @QuantityPerUnit, @UnitPrice, @UnitsInStock, 
                                     @UnitsOnOrder, @ReorderLevel, @Discontinued, @LastUserId, @LastDateUpdated)";

                var productExists = ProductExists(product.ProductName);
                if (productExists)
                {
                    return false;
                }
                else
                {
                    db.Execute(insertQuery, product);
                    return true;

                }

            }
        }

        public bool CreateOrder(Orders order)
        {
            using (IDbConnection db = DbConnectionFactory.CreateConnection())
            {
                var query = @"SELECT TOP 1 OrderId, CustomerId, EmployeeId, OrderDate, RequiredDate, ShippedDate, 
                        ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, 
                        ShipCountry
                 FROM Orders
                 ORDER BY OrderDate DESC";
                int orderId = db.QuerySingleOrDefault<int>(query, order) + 1;
                var Count = 0;
                var orderCount = 0;
                foreach (var product in order.Products)
                {
                    var countQuantity = CheckIfProductAvailable(product.ProductId);
                    var prodactQuantity = GetProductQuantity(product.ProductId);
                    var totalCount = Count + countQuantity + product.Quantity;
                    if (totalCount > prodactQuantity)
                    {
                        Console.WriteLine("No available quantity!");
                    }
                    else
                    {
                        while (orderCount == 0)
                        {
                            db.Execute( @"INSERT INTO Orders 
                                    (CustomerId, EmployeeId, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, 
                                     ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry)
                                    VALUES 
                                    (@CustomerId, @EmployeeId, @OrderDate, @RequiredDate, @ShippedDate, @ShipVia, @Freight, 
                                     @ShipName, @ShipAddress, @ShipCity, @ShipRegion, @ShipPostalCode, @ShipCountry);

                                    SELECT CAST(SCOPE_IDENTITY() as int)");
                            orderCount++;
                        };
                        string orderDetailsQuery = @"INSERT INTO OrderDetails
                                                (OrderId, ProductId,UnitPrice, Quantity, Discount)
                                             VALUES
                                                (@OrderId, @ProductId, @UnitPrice, @Quantity, @Discount)";

                        db.Execute(orderDetailsQuery, new
                        {
                            OrderId = orderId,
                            ProductId = product.ProductId,
                            UnitPrice = product.UnitPrice,
                            Quantity = product.Quantity,
                            Discount = product.Discount
                        });
                        Count += product.Quantity;
                        orderCount++;
                    }


                }
                return true;
            
            }
        }

        public ICollection<Orders> OrderByDateOrders()
        {
            using (IDbConnection db = DbConnectionFactory.CreateConnection())
            {

                var orders = db.Query<Orders>("SELECT * FROM Orders ORDER BY OrderDate").ToList();
                return orders;
            }

        }
        public IEnumerable<Products> SortedByMostSold()
        {
            using (IDbConnection db = DbConnectionFactory.CreateConnection())
            {
                string sql = @"
                SELECT p.*, SUM(od.Quantity) AS TotalQuantitySold
                FROM Products p
                INNER JOIN OrderDetails od ON p.ProductId = od.ProductId
                GROUP BY p.ProductId, p.ProductName, p.SupplierId, p.CategoryId, p.QuantityPerUnit, p.UnitPrice, 
                         p.UnitsInStock, p.UnitsOnOrder, p.ReorderLevel, p.Discontinued
                ORDER BY TotalQuantitySold DESC";

                return db.Query<Products>(sql);
            }
        }
        private bool ProductExists(string productName)
        {
            using (IDbConnection db = DbConnectionFactory.CreateConnection())
            {
                string query = "SELECT COUNT(*) FROM Products WHERE ProductName = @ProductName";

                int count = db.ExecuteScalar<int>(query, new { ProductName = productName });

                return count > 0;
            }
        }

        private bool CategoryExists(string categoryName)
        {
            using (IDbConnection db = DbConnectionFactory.CreateConnection())
            {
                string query = "SELECT COUNT(*) FROM Categories WHERE CategoryName = @CategoryName";

                int count = db.ExecuteScalar<int>(query, new { CategoryName = categoryName });

                return count > 0;
            }

        }

        private int CheckIfProductAvailable(int productId)
        {
            using (IDbConnection db = DbConnectionFactory.CreateConnection())
            {
                var productQuantity = "SELECT OrderDetails.Quantity FROM OrderDetails WHERE ProductId = @ProductId";
                var count = db.Query<int>(productQuantity, new { ProductId = productId }).ToList();
                var countQuantity = count.Sum();
                return countQuantity;
            }
        }

        private int GetProductQuantity(int productId)
        {
            using (IDbConnection db = DbConnectionFactory.CreateConnection())
            {
                var product = "SELECT QuantityPerUnit FROM Products WHERE ProductId = @ProductId";
                var productQuantity = db.ExecuteScalar<int>(product, new { ProductId = productId });
                return productQuantity;
            }

        }
    }

}
