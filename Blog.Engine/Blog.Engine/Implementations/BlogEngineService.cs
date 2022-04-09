using Blog.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Engine.Implementations
{
    public class BlogEngineService : IBlogEngineService
    {
        public List<Posts> get()
        {
            return new ImplementacionAPIBlog().GetListPublishedPosts();
        }

        public Result post(int postsId, string comment)
        {
            return new ImplementacionAPIBlog().AddCommentPosts(postsId, comment);
        }
    }
}