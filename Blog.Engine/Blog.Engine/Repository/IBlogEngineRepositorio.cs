using Blog.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Engine.Repository
{
    public interface IBlogEngineRepositorio
    {
        List<Posts> GetListPublishedPosts();
        Result AddCommentPosts(int postsId, string comment);
    }
}