using MongoDB.Driver;
using System.Threading.Tasks;
using ThuAo.Models;

namespace ThuAo.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService()
        {
            var client = new MongoClient("mongodb+srv://ThuAo:ThuAoLapTrinhMang25@cluster0.q11efhi.mongodb.net/");
            var database = client.GetDatabase("ThuAo"); // ✅ đúng tên database của bạn
            _users = database.GetCollection<User>("users"); // Tên collection là "users"
           
           
        }

        public async Task CreateUser(User user)
        {
            await _users.InsertOneAsync(user);
        }

        public async Task<bool> EmailExists(string email)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            var user = await _users.Find(filter).FirstOrDefaultAsync();
            return user != null;
        }

        // Phương thức để xác thực người dùng khi đăng nhập
        public async Task<User> AuthenticateUser(string username, string password)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Username, username) & Builders<User>.Filter.Eq(u => u.Password, password);
            var user = await _users.Find(filter).FirstOrDefaultAsync();
            return user;
        }
        private readonly IMongoDatabase _database;

        

        public async Task<User> FindByEmail(string email)
        {
            var collection = _database.GetCollection<User>("Users");
            return await collection.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdatePasswordByEmail(string email, string newPassword)
        {
            var collection = _database.GetCollection<User>("Users");
            var update = Builders<User>.Update.Set(u => u.Password, newPassword);
            var result = await collection.UpdateOneAsync(u => u.Email == email, update);
            return result.ModifiedCount > 0;
        }

    }
}
