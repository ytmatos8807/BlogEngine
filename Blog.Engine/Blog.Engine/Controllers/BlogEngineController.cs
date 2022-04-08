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
    }
}