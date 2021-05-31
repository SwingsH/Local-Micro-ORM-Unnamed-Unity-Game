using System.Collections.Generic;
using TIZSoft.UnknownGame.SaveData;

namespace TIZSoft.UnknownGame
{
    public class ApiHeaderCreator
    {
        readonly UnknowGameSave unknowGameSave;
        readonly User localUser;
        public ApiHeaderCreator(UnknowGameSave save, User user)
        {
            this.localUser = user;
            this.unknowGameSave = save;
        }

        public IEnumerable<KeyValuePair<string, string>> CreateHeaders()
        {
            var headers = new Dictionary<string, string>();
            headers["User-Id"] = unknowGameSave.GameSave.CurrentUserSave.Id.ToString();
            var token = localUser.AccessToken.Value != null ? localUser.AccessToken.Value : "";
            headers["Token"] = token;
            return headers;
        }
    }
}
