using Blog.Engine.Base;
using Blog.Engine.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Blog.Engine.Controllers
{
    [RoutePrefix("blogengine")]
    public class BlogEngineController : ApiController
    {

        /// <summary>
        /// Obteniendo la lista de publicaciones publicadas sin restriccion de rol
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public HttpResponseMessage Get()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                List<Posts> result = new BlogEngineService().get();
                PostsData data = new PostsData();
                data.Data = result;
                response = Request.CreateResponse(System.Net.HttpStatusCode.OK, data);
            }
            catch (Exception e)
            {
                Error err = ErrorFactory.Build(e);
                response = Request.CreateResponse((System.Net.HttpStatusCode)422, err);
                return response;
            }
            return response;
        }

        /// <summary>
        /// Insertando comentario en una publicacion X
        /// </summary>
        /// <param name="postsId">id de la publicacion</param>
        /// <param name="comment">comentario a insertar</param>
        /// <returns></returns>
        [HttpPost()]
        public HttpResponseMessage Post(int postsId, string comment)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                Result result = new BlogEngineService().post(postsId, comment);
                CommentData data = new CommentData();
                data.Data = result;
                response = Request.CreateResponse(System.Net.HttpStatusCode.OK, data);
            }
            catch (Exception e)
            {
                Error err = ErrorFactory.Build(e);
                response = Request.CreateResponse((System.Net.HttpStatusCode)422, err);
                return response;
            }
            return response;
        }

        /// <summary>
        /// Procesa todas las acciones que puede ejecutar un Escritor
        /// </summary>
        /// <param name="actionProcess">Accion recibida</param>
        /// <param name="posts">publicacion recibida</param>
        /// <param name="author">autor de la publicacion</param>
        /// <returns></returns>
        [HttpPost()]
        public HttpResponseMessage PostProcess(string actionProcess, Posts posts, string author)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                ProcessPostsResponse result = new BlogEngineService().postprocess(actionProcess, posts, author);
                ProcessPostsResponseData data = new ProcessPostsResponseData();
                data.Data = result;
                response = Request.CreateResponse(System.Net.HttpStatusCode.OK, data);
            }
            catch (Exception e)
            {
                Error err = ErrorFactory.Build(e);
                response = Request.CreateResponse((System.Net.HttpStatusCode)422, err);
                return response;
            }
            return response;
        }
    }
}