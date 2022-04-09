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

        /// <summary>
        /// Obteniendo la lista de publicaciones publicadas sin restriccion de rol
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Insertando comentario en una publicacion X
        /// </summary>
        /// <param name="postsId">id de la publicacion</param>
        /// <param name="comment">comentario a insertar</param>
        /// <returns></returns>
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

        /// <summary>
        /// Procesa todas las acciones que puede ejecutar un Escritor
        /// </summary>
        /// <param name="actionProcess">Accion recibida</param>
        /// <param name="posts">publicacion recibida</param>
        /// <param name="author">autor de la publicacion</param>
        /// <returns></returns>
        public ProcessPostsResponse ProcessPostsWriter(string actionProcess, Posts posts, string author)
        {
            ProcessPostsResponse result = new ProcessPostsResponse();
            try
            {
                result = blogRepositorio.ProcessPostsWriter(actionProcess, posts, author);
            }
            catch (Exception e)
            {
                logger.Error("Error al procesar las publicacions de un escritor, Message [{0}] - StackTrace [{1}] ", e.Message, e.StackTrace);
                throw;
            }
            return result;
        }
    }
}