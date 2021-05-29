using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityHTTPServer
{
    public interface IServer
    {
        void OnGet(ServerSideHttpRequest request, HttpResponse response);
        void OnPost(ServerSideHttpRequest request, HttpResponse response);
        void OnDefault(ServerSideHttpRequest request, HttpResponse response);
    }
}
