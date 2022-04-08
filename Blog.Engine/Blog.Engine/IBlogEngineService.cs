using Blog.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Engine
{
    interface IBlogEngineService
    {
        List<Posts> get();
    }
}