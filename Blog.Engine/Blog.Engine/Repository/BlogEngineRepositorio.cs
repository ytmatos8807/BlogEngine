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

        /// <summary>
        /// Obteniendo la lista de publicaciones publicadas sin restriccion de rol
        /// </summary>
        /// <returns></returns>
        public List<Posts> GetListPublishedPosts()
        {
            logger.Info("Iniciando servicio GetListPublishedPosts");

            List<Posts> result = new List<Posts>();
            try
            {
                MySqlConnection conn = new MySqlConnection(_connString);
                conn.Open();

                logger.Debug("Vamos a ejecutar el siguiente qry [{0}]", QRY_GET_PUBLISHED_POSTS);
                MySqlCommand cmd = new MySqlCommand(QRY_GET_PUBLISHED_POSTS, conn);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    logger.Debug("Vamos a leer los registros obtenidos");
                    while (reader.Read())
                    {
                        Posts posts = new Posts();
                        var postsId = (reader.GetValue(reader.GetOrdinal("id_posts")).ToString() != null) ? reader.GetValue(reader.GetOrdinal("id_posts")).ToString().Trim() : null;
                        var status = (reader.GetValue(reader.GetOrdinal("status")).ToString() != null) ? reader.GetValue(reader.GetOrdinal("status")).ToString().Trim() : null;
                        var commentId = (reader.GetValue(reader.GetOrdinal("id_comment")).ToString() != null) ? reader.GetValue(reader.GetOrdinal("id_comment")).ToString().Trim() : null;
                        var title = (reader.GetValue(reader.GetOrdinal("title")).ToString() != null) ? reader.GetValue(reader.GetOrdinal("title")).ToString().Trim() : null;
                        var content = (reader.GetValue(reader.GetOrdinal("content")).ToString() != null) ? reader.GetValue(reader.GetOrdinal("content")).ToString().Trim() : null;
                        var author = (reader.GetValue(reader.GetOrdinal("author")).ToString() != null) ? reader.GetValue(reader.GetOrdinal("author")).ToString().Trim() : null;
                        var date = (reader.GetValue(reader.GetOrdinal("date")).ToString() != null) ? reader.GetValue(reader.GetOrdinal("date")).ToString().Trim() : null;

                        result.Add(posts);
                    }
                    logger.Debug("Luego de obtener los registros obtenidos. Obtuvimos [{0}] publicaciones publicadas.", result.Count);
                }
            }
            catch (Exception e)
            {
                logger.Error("Error al obtener la lista de publicaciones publicados, Message [{0}] - StackTrace [{1}] ", e.Message, e.StackTrace);
                throw;
            }

            logger.Info("Saliendo del servicio GetListPublishedPosts");
            return result;
        }

    }
}