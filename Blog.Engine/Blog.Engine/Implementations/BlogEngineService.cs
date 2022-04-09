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

        public ProcessPostsResponse postprocess(string actionProcess, Posts posts, string author)
        {
            return new ImplementacionAPIBlog().ProcessPostsWriter(actionProcess, posts, author);
        }

        public ProcessPostsResponse postprocesseditor(string actionProcess, Posts posts, string author)
        {
            return new ImplementacionAPIBlog().ProcessPostsEditors(actionProcess, posts, author);
        }
    }
}