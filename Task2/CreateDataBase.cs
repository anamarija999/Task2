using System.Data.SqlClient;
using Dapper;

namespace Task2
{
    public class CreateDataBase
    {
        public void CreateDataTable()
        {
            var connectionString = DbConnectionFactory.CreateConnection();

            connectionString.Open();

            // Create a new Products table
            var categoriesCommand = @" CREATE TABLE Categories (
                                 CategoryID INT PRIMARY KEY,
                                 CategoryName NVARCHAR(50) NULL,
                                 Description NVARCHAR(100) NULL,
                                 Picture NVARCHAR(255) NULL)";

            connectionString.Execute(categoriesCommand);
            // Create a new Categories table
            var productsCommand = @" CREATE TABLE Products (
                                 ProductId INT PRIMARY KEY,
                                 ProductName NVARCHAR(50) NULL,
                                 SupplierId INT NULL,
                                 CategoryId INT NOT NULL,
                                 QuantityPerUnit INT NULL,
                                 UnitPrice INT NULL,
                                 UnitsInStock INT NULL,
                                 UnitsOnOrder INT NULL,
                                 ReorderLevel INT NULL,
                                 Discontinued BIT NOT NULL,
                                 LastUserId INT NOT NULL,
                                 LastDateUpdated DATETIME NOT NULL,
                                 FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId))";

            connectionString.Execute(productsCommand);

            var ordersCommand = @" CREATE TABLE Orders (
                                       OrderId INT PRIMARY KEY,
                                       CustomerId INT NOT NULL,
                                       EmployeeId INT NOT NULL,
                                       OrderDate DATETIME NOT NULL,
                                       RequiredDate DATETIME NOT NULL,
                                       ShippedDate DATETIME NOT NULL,
                                       ShipVia DATETIME NOT NULL,
                                       Freight DECIMAL(18,2) NOT NULL,
                                       ShipName NVARCHAR(50) NULL,
                                       ShipAddress NVARCHAR(100) NULL,
                                       ShipCity NVARCHAR(50) NULL,
                                       ShipRegion NVARCHAR(50) NULL,
                                       ShipPostalCode NVARCHAR(20) NULL,
                                       ShipCountry NVARCHAR(50) NULL)";

            connectionString.Execute(ordersCommand);

            var orderDetailsCommand = @" CREATE TABLE OrderDetails (
                                             Id INT NOT NULL
                                             OrderId INT NOT NULL,
                                             ProductId INT NOT NULL,
                                             UnitPrice DECIMAL(18,2) NOT NULL,
                                             Quantity INT NOT NULL,
                                             Discount INT NOT NULL,
                                             PRIMARY KEY (Id),
                                             FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
                                             FOREIGN KEY (ProductId) REFERENCES Products(ProductId))";
            connectionString.Execute(orderDetailsCommand);
        }
    }
       
}


