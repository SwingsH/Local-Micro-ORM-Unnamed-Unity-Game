using System.Collections.Generic;

namespace TIZSoft.UnknownGame
{
    public class ApiHeaderCreator
    {
        //readonly ViolinSave violinSave;
        //readonly User localUser;

        //public ApiHeaderCreator(ViolinSave violinSave, User localUser)
        public ApiHeaderCreator()
        {
            //this.localUser = localUser;
            //this.violinSave = violinSave;
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
