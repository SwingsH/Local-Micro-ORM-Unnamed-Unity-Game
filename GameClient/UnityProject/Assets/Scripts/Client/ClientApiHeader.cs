using System.Collections.Generic;

namespace TIZSoft.UnknownGame
{
    public class ApiHeaderCreator
    {
        public ApiHeaderCreator()
        {
            //this.localUser = localUser;
            //this.Save = Save;
        }

        public IEnumerable<KeyValuePair<string, string>> CreateHeaders()
        {
            var headers = new Dictionary<string, string>();
            headers["User-Id"] = "todo_user_id";
            var token = "todo_assess_token";
            headers["Token"] = token;
            return headers;
        }
    }
}
