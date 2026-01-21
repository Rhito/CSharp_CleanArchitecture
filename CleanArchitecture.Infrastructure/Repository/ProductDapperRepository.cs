using CleanArchitecture.Application.Exceptions;
using CleanArchitecture.Application.Filters;
using CleanArchitecture.Application.IRepository;
using CleanArchitecture.Domain.Entities;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text;

namespace CleanArchitecture.Infrastructure.Repository
{
    public class ProductDapperRepository : IProductRepository
    {
        private readonly string _connectionString;
        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);
        public ProductDapperRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Could not find the connection string.");
        }

        public async Task<Product> AddAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product), "Product data must not be null.");
            }
            const string sql = @"INSERT INTO Products (Name, Description, StockQuantity, Price, CategoryId, IsDeleted)
                                VALUES (@Name, @Description,@StockQuantity, @Price,@CategoryId, @IsDeleted);
                                SELECT CAST(SCOPE_IDENTITY() as int);"; // Lấy ID vừa tạo trong SQL Server

           using var connection = CreateConnection();
            // 3. Thực thi truy vấn
            // Dapper sẽ tự động map các thuộc tính của 'product' vào các tham số @Name, @Price...
            var newId = await connection.QuerySingleAsync<int>(sql, product);

            // 4. Gán ID mới vào object và trả về
            product.Id = newId;
            return product;
        }

        public async Task<bool> BulkDeleteAsync(List<int> ids, bool isHardDelete)
        {
            if(ids == null || !ids.Any())
            {
                throw new ArgumentException("Ids must not be null");
            }
            string sql;
            if (isHardDelete)
            {
                sql = @"DELETE FROM Products WHERE Id in @Ids";
            }else
            {
                sql = @"UPDATE Products SET IsDeleted = 1 WHERE Id in @Ids";
            }

            using var connection = CreateConnection();
            var productAffected = await connection.ExecuteAsync(sql, new { Ids = ids });
            return productAffected > 0;
        }

        public async Task<bool> BulkRestoreAsync(List<int> ids)
        {
            if (ids == null)
            {
                throw new ArgumentException("Ids must not be null");
            }
            const string sql = @"UPDATE Products SET IsDeleted = 0 WHERE Id in @Ids";
            using var connection = CreateConnection();
            var productAffected = await connection.ExecuteAsync(sql, new { Ids = ids });
            return productAffected > 0;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            const string sql = @"SELECT Id, Name, Description, Price, StockQuantity, CategoryId, IsDeleted FROM Products";

            using var connection = CreateConnection();
            var products = await connection.QueryAsync<Product>(sql);
            return products.ToList();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
           if (id <= 0)
           {
                throw new ArgumentException("Invalid product ID.", nameof(id));
           }
            const string sql = @"SELECT Id, Name, Description, Price, StockQuantity, CategoryId, IsDeleted 
                                 FROM Products 
                                 WHERE Id = @Id";
            using var connection = CreateConnection();
            var product = await connection.QuerySingleOrDefaultAsync<Product>(sql, new { Id = id });
            return product ?? throw new NotFoundException($"Không tìm thấy ID {id}");
        }

        public async Task<List<(int Id, bool IsDeleted)>> GetListDeletedByIds(List<int> ids)
        {
            if(ids == null || !ids.Any())
            {
                throw new ArgumentException("ID list must not be null or empty.", nameof(ids));
            }

            const string sql = @"Select Id, IsDeleted FROM Products WHERE Id IN @Ids";

            using var connnection = CreateConnection();


            var productStatesRaw = await connnection.QueryAsync(sql, new { Ids = ids });

            return productStatesRaw.Select(x => ((int)x.Id, (bool)x.IsDeleted)).ToList();

        }

        public async Task<bool> HardDeleteAsync(int id)
        {
            const string sql = @"Delete From Products WHERE Id = @Id";
            using var connection = CreateConnection();
            var productDelete = await connection.ExecuteAsync(sql, new { id });
            return productDelete > 0;
        }

        public async Task<bool> RestoreAsync(int id)
        {
            const string sql = @"UPDATE Products SET [IsDeleted] = 0 WHERE Id = @Id";
            using var connection = CreateConnection();
            var productRestore = await connection.ExecuteAsync(sql, new { id });
            return productRestore > 0;
        }

        public async Task<IEnumerable<Product>> SearchProductAsync(ProductSearchFilter productSearchFilter)
        {
            var sqlBuilder = new StringBuilder(@"SELECT * FROM Products WHERE 1=1 ");


            var parameters = new DynamicParameters();
            // Apply filters
            if (!string.IsNullOrEmpty(productSearchFilter.Keyword))
            {
                sqlBuilder.Append(" AND (NAME LIKE @Keyword OR Description LIKE @Keyword) ");
                parameters.Add("@Keyword", $"%{productSearchFilter.Keyword}%");
            }
          
            // filter by category
            if (productSearchFilter.CategoryId.HasValue)
            {
                sqlBuilder.Append(" AND CategoryId = @CategoryId");
                parameters.Add("@CategoryId", productSearchFilter.CategoryId);
            }

            // filter by soft delete status
            if (productSearchFilter.IsDeleted.HasValue)
            {
                sqlBuilder.Append(" AND IsDeleted = @IsDeleted");
                parameters.Add("@IsDeleted", $"{productSearchFilter.IsDeleted}");
            }

            // filter by price range
            if (productSearchFilter.FromPrice.HasValue)
            {
                sqlBuilder.Append(" AND Price >= @FromPrice");
                parameters.Add(@"FromPrice", productSearchFilter.FromPrice);
            }
                
            if (productSearchFilter.ToPrice.HasValue)
            {
                sqlBuilder.Append(" AND Price <= @ToPrice");
                parameters.Add(@"ToPrice", productSearchFilter.ToPrice);
            }

            // ---Xử lý Phân trang(Pagination)-- -
            // BẮT BUỘC: SQL Server yêu cầu phải có ORDER BY mới được dùng OFFSET (Phân trang)
            sqlBuilder.Append(" ORDER BY ID DESC");
            sqlBuilder.Append(" OFFSET @Skip ROW FETCH NEXT @Take ROWS ONLY");


            // Apply pagination 
            int skip = (productSearchFilter.PageNumber - 1) * productSearchFilter.PageSize;
            parameters.Add("@Skip", skip);
            parameters.Add("@Take", productSearchFilter.PageSize);


            // 7. Thực thi
            using var connection = CreateConnection();
            var products = await connection.QueryAsync<Product>(sqlBuilder.ToString(), parameters);
            return products;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            const string sql = @"UPDATE Products SET IsDeleted = 1 WHERE Id = @Id";
            using var connection = CreateConnection();
            var productRestore = await connection.ExecuteAsync(sql, new {id});
            return productRestore > 0;
        }

        public async Task<Product> UpdateAsync(int id,Product product)
        {
            if(product == null)
            {
                throw new ValidationException();
            }
            product.Id = id;
            const string sql = @"UPDATE Products SET 
                                Name = @Name,
                                Description = @Description,
                                Price = @Price,
                                StockQuantity = @StockQuantity,
                                CategoryId = @CategoryId,
                                IsDeleted = @IsDeleted
                                WHERE Id = @id";

            using var connection = CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, product);

            if (rowsAffected == 0)
            {
                throw new Exception($"Update failed with ID {id}");
            }

            return product;
        }

        

       
    }
}
