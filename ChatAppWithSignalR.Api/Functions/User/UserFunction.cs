using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatAppWithSignalR.Api.Functions.User
{
    /*
        * Класс "UserFunction" предоставляет методы для аутентификации пользователей,
          получения информации о пользователе и генерации JWT-токенов. Эти методы могут
          использоваться в приложении ChatApp для управления пользователями
          и обеспечения безопасности аутентификации.
    */
    public class UserFunction : IUserFunction
    {
        private readonly ChatAppContext _chatAppContext;

        // В конструкторе класса "UserFunction" принимается параметр "chatAppContext"
        // типа "ChatAppContext", который представляет контекст базы данных приложения.
        public UserFunction(ChatAppContext chatAppContext)
        {
            _chatAppContext = chatAppContext;
        }

        /*
            * Метод "Authenticate" используется для аутентификации пользователя.
              Он принимает идентификатор пользователя (loginId) и пароль (password).
              Внутри метода выполняется запрос к базе данных для поиска пользователя
              по идентификатору. Если пользователь существует, его пароль проверяется
              с помощью метода "VerifyPassword". Если пароль совпадает, генерируется
              JWT-токен с помощью метода "GenerateJwtToken", и возвращается объект
              "User" с информацией о пользователе и созданным токеном.
              Если аутентификация не удалась или произошла ошибка,
              метод возвращает значение null.
        */
        public User? Authenticate(string loginId, string password)
        {
            try
            {
                var entity = _chatAppContext.TblUsers.SingleOrDefault(x => x.LoginId == loginId);
                if (entity == null) return null;

                var isPasswordMatched = VertifyPassword(password, entity.StoredSalt, entity.Password);

                if (!isPasswordMatched) return null;

                var token = GenerateJwtToken(entity);

                return new User
                {
                    Id = entity.Id,
                    UserName = entity.UserName,
                    Token = token
                };

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /*
            * Метод "GetUserById" используется для получения информации о пользователе
              по его идентификатору. Он принимает идентификатор пользователя (id).
              Внутри метода выполняется запрос к базе данных для поиска пользователя
              по идентификатору. Если пользователь существует, он преобразуется
              в объект "User" со значениями свойств, такими как имя пользователя,
              идентификатор, источник аватара, статус отсутствия и т.д.
              Если пользователя не существует, возвращается пустой объект "User".
        */
        public User GetUserById(int id)
        {
            var entity = _chatAppContext.TblUsers
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (entity == null) return new User();

            var awayDuration = entity.IsOnline ? "" : Utilities.CalcAwayDuration(entity.LastLogonTime);
            return
                new User
                {
                    UserName = entity.UserName,
                    Id = entity.Id,
                    AvatarSourceName = entity.AvatarSourceName,
                    IsAway = awayDuration != "" ? true : false,
                    AwayDuration = awayDuration,
                    IsOnline = entity.IsOnline,
                    LastLogonTime = entity.LastLogonTime
                };
        }

        /*
            * Приватный метод "VerifyPassword" используется для проверки соответствия
              введенного пароля и хранящегося в базе данных пароля. Он принимает 
              введенный пароль (enteredPassword), хранящуюся соль (storedSalt) 
              и хранящийся зашифрованный пароль (storedPassword). Метод использует 
              функцию Pbkdf2 с параметрами, такими как соль, итерационный счетчик 
              и алгоритм хэширования, чтобы создать зашифрованный пароль из введенного 
              пароля и сравнивает его с хранящимся зашифрованным паролем. Если пароли 
              совпадают, метод возвращает true, в противном случае - false.
        */
        private bool VertifyPassword(string enteredPassword, byte[] storedSalt, string storedPassword)
        {
            string encryptyedPassword = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: enteredPassword,
                    salt: storedSalt,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

            return encryptyedPassword.Equals(storedPassword);
        }

        /*
            * Приватный метод "GenerateJwtToken" используется для генерации JWT-токена
              для аутентифицированного пользователя. Он принимает объект "TblUser"
              (представляющий пользователя) и возвращает сгенерированный токен. 
              Внутри метода создается новый экземпляр "JwtSecurityTokenHandler" 
              и берется секретный ключ для подписи токена. Затем создается дескриптор 
              токена, включающий идентификатор пользователя в виде утверждения, время 
              истечения срока действия токена и подпись с использованием симметричного 
              ключа. Наконец, с помощью метода "CreateToken" создается токен, 
              и результат возвращается как строка с помощью метода "WriteToken".
        */
        private string GenerateJwtToken(TblUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("1234567890123456");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
    }
}
