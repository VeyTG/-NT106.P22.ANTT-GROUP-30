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
    }
}
