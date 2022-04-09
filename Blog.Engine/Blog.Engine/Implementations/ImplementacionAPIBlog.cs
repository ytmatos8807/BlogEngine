using Blog.Engine.Base;
using Blog.Engine.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Engine.Implementations
{
    public class ImplementacionAPIBlog
    {
        #region Logger
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        #endregion

        #region Repository
        private IBlogEngineRepositorio blogRepositorio;
        #endregion

        #region Ctor
        public ImplementacionAPIBlog()
        {
            this.blogRepositorio = new BlogEngineRepositorio();
        }
        #endregion

        public List<Posts> GetListPublishedPosts()
        {
            List<Posts> result = new List<Posts>();
            try
            {
                result = blogRepositorio.GetListPublishedPosts();
            }
            catch (Exception e)
            {
                logger.Error("Error al obtener la lista de publicaciones publicados, Message [{0}] - StackTrace [{1}] ", e.Message, e.StackTrace);
                throw;
            }
            return result;
        }

        public Result AddCommentPosts(int postsId, string comment)
        {
            Result result = new Result();
            try
            {
                result = blogRepositorio.AddCommentPosts(postsId, comment);
            }
            catch (Exception e)
            {
                logger.Error("Error al insertar comenario en publicacions publicadas, Message [{0}] - StackTrace [{1}] ", e.Message, e.StackTrace);
                throw;
            }
            return result;
        }
    }
}