using Blog.Engine.Base;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Blog.Engine.Repository
{
    public class BlogEngineRepositorio : IBlogEngineRepositorio
    {
        #region Logger
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        #endregion

        #region ConnString
        private static string _connStringName = ConfigurationManager.AppSettings.Get("MySqlConnectionString");
        private static string _connString = ConfigurationManager.ConnectionStrings[_connStringName].ConnectionString;
        #endregion

        #region Qrys

        /// <summary>
        /// Obtiene una lista de publicaciones publicadas
        /// </summary>
        private const string QRY_GET_PUBLISHED_POSTS = " SELECT * FROM blog_engine.posts where status = 'published' ";
        #endregion


        public List<Posts> GetListPublishedPosts()
        {
            logger.Info("Iniciando servicio GetListPublishedPosts");

            List<Posts> result = new List<Posts>();
            try
            {
                MySqlConnection conn = new MySqlConnection(_connString);
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(QRY_GET_PUBLISHED_POSTS, conn);

                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        /* iterate once per row */
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            logger.Info("Saliendo del servicio GetListPublishedPosts");
            return result;
        }

    }
}