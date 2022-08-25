using System.Text;
using Todo.Models;

namespace Todo.Auth
{
    public class Auth
    {
        public static string[] getUserId(HttpContext httpContext)
        {
            string authHeader = httpContext.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic"))
            {

                string ecodeUsernameAndPassword = authHeader.Substring("Basic ".Length).Trim();
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                string usernameAndPassword = encoding.GetString(Convert.FromBase64String(ecodeUsernameAndPassword));
                int index = usernameAndPassword.IndexOf(":");
                var username = usernameAndPassword.Substring(0, index);
                var password = usernameAndPassword.Substring(index + 1);

                string[] userNameAndPasswordArray = new string[] { username.ToString(), password.ToString() };
                return userNameAndPasswordArray;

            }

            string[] error = new string[] { "InvalidCreadentials" };
            return error;

        }
    }
}
