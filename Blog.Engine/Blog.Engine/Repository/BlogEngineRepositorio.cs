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

        /// <summary>
        /// Obtiene la cantidad de comentarios que tiene una publicacion X
        /// </summary>
        private const string QRY_GET_COMMENT_AMOUNT = " SELECT COUNT(*) FROM blog_engine.comment where id_posts = @paramId ";

        /// <summary>
        /// Obtiene el estado de una publicacion X
        /// </summary>
        private const string QRY_GET_STATUS_POSTS = " SELECT status  FROM blog_engine.posts where id_posts = @paramId ";

        /// <summary>
        /// Inserta comentario a una publicacion publicada
        /// </summary>
        private const string QRY_POST_COMMENT = " INSERT INTO `blog_engine`.`comment` (`content`, `id_posts`) VALUES (@paramComment,  @paramIdPosts) ";

        /// <summary>
        /// Obtiene una lista de publicaciones 
        /// </summary>
        private const string QRY_GET_POSTS = " SELECT * FROM blog_engine.posts where author = @paramAuthor ";

        /// <summary>
        /// Inserta una publicacion de un escritor
        /// </summary>
        private const string QRY_POST_POSTS = " INSERT INTO `blog_engine`.`posts` (`status`, `id_comment`, `title`, `content`, `author`, `date`, `blocked`) " +
                                              " VALUES (@paramStatus,  @paramcommentId, @paramTitle,  @paramContent, @paramDate,  @paramBlocked)";

        /// <summary>
        /// Actualiza una publicacion de un escritor
        /// </summary>
        private const string QRY_UPDATE_POSTS = " UPDATE `blog_engine`.`posts` SET `title` = @paramTitle ,`content` = @paramContent  WHERE (`id_posts` = @paramIdPosts) ";

        /// <summary>
        /// Qry para verificar el rol de un usuario
        /// </summary>
        private const string QRY_GET_AUTHOR_ROL = " SELECT roles FROM blog_engine.users where title = @paramTitle ";

        /// <summary>
        /// Envia una publicacion de un escritor
        /// </summary>
        private const string QRY_SUBMIT_POSTS = " UPDATE `blog_engine`.`posts` SET `title` = @paramTitle ,`content` = @paramContent `status` = @paramStatus,  `blocked` = 'S' " +
                                                "  WHERE (`id_posts` = @paramIdPosts) ";

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
                        var commentAmount = this.GetAmountCommet(Int32.Parse(postsId));
                        var title = (reader.GetValue(reader.GetOrdinal("title")).ToString() != null) ? reader.GetValue(reader.GetOrdinal("title")).ToString().Trim() : null;
                        var content = (reader.GetValue(reader.GetOrdinal("content")).ToString() != null) ? reader.GetValue(reader.GetOrdinal("content")).ToString().Trim() : null;
                        var author = (reader.GetValue(reader.GetOrdinal("author")).ToString() != null) ? reader.GetValue(reader.GetOrdinal("author")).ToString().Trim() : null;
                        var date = (reader.GetValue(reader.GetOrdinal("date")).ToString() != null) ? reader.GetValue(reader.GetOrdinal("date")).ToString().Trim() : null;

                        posts.PostsId = Convert.ToInt32(postsId);
                        posts.Status = status;
                        posts.AmountComment = commentAmount;
                        posts.Title = title;
                        posts.Content = content;
                        posts.DatePosts = DateTime.Parse(date);
                        posts.Author = author;

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

        /// <summary>
        /// Obtiene la cantidad de comentarios que tiene una publicacion X
        /// </summary>
        /// <param name="postsId">id de la publicacion</param>
        /// <returns></returns>
        private int GetAmountCommet(int postsId)
        {
            logger.Info("Iniciando servicio GetAmountCommet");

            Int32 result = 0;
            try
            {
                MySqlConnection conn = new MySqlConnection(_connString);
                conn.Open();

                logger.Debug("Vamos a ejecutar el siguiente qry [{0}]", QRY_GET_COMMENT_AMOUNT);
                MySqlCommand cmd = new MySqlCommand(QRY_GET_COMMENT_AMOUNT, conn);
                cmd.Parameters.AddWithValue("@paramId", postsId);

                result = int.Parse(cmd.ExecuteScalar().ToString());
                logger.Debug("Luego de obtener los registros obtenidos. Obtuvimos [{0}] comentarios.", result);
            }
            catch (Exception e)
            {
                logger.Error("Error al obtener la csantidad de comentarios de la publicacion [{0}] , Message [{1}] - StackTrace [{2}] ",postsId, e.Message, e.StackTrace);
                throw;
            }

            logger.Info("Saliendo del servicio GetAmountCommet");
            return result;

        }

        /// <summary>
        /// Obtiene el estado de una publicacion dada
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        private string GetStatusPost(int postsId)
        {
            logger.Info("Iniciando servicio GetStatusPost. Publicacion Id [{0}]", postsId);

            string result = string.Empty;
            try
            {
                MySqlConnection conn = new MySqlConnection(_connString);
                conn.Open();

                logger.Debug("Vamos a ejecutar el siguiente qry [{0}]", QRY_GET_STATUS_POSTS);
                MySqlCommand cmd = new MySqlCommand(QRY_GET_STATUS_POSTS, conn);
                cmd.Parameters.AddWithValue("@paramId", postsId);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    logger.Debug("Vamos a leer los registros obtenidos");
                    while (reader.Read())
                    {
                        result = (reader.GetValue(reader.GetOrdinal("status")).ToString() != null) ? reader.GetValue(reader.GetOrdinal("status")).ToString().Trim() : string.Empty;
                    }
                    logger.Debug("Luego de obtener los registros obtenidos. Obtuvimos [{0}] estado .", result);
                }
            }
            catch (Exception e)
            {
                logger.Error("Error al obtener el estado de la publicacion  [{0}], Message [{1}] - StackTrace [{2}] ", e.Message, e.StackTrace);
                throw;
            }

            logger.Info("Saliendo del servicio GetStatusPost. Publicacion Id [{0}]", postsId);
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
            logger.Info("Iniciando servicio AddCommentPosts. Publicacion Id [{0}]", postsId);

            Result result = new Result();
            try
            {
                //Verifico que este en estado publicado
                string status = this.GetStatusPost(postsId);
                if (status != EnumStatePosts.Published.ToString())
                {
                    result.State = ResultState.ERROR;

                      //aca lo ideal seria tener una libreria que se encargue del manejo de los mnesajes de error 
                      //por falta de tiempo coloco aca.
                    result.Message = " Publicación en Estado no válido para añadir un comentario"; 
                    return result;
                }

                MySqlConnection conn = new MySqlConnection(_connString);
                conn.Open();

                logger.Debug("Vamos a ejecutar el siguiente qry [{0}]", QRY_POST_COMMENT);
                MySqlCommand cmd = new MySqlCommand(QRY_POST_COMMENT, conn);
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@paramComment", comment);
                cmd.Parameters.AddWithValue("@paramIdPosts", postsId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                logger.Error("Error al insertar un comentario en la publicacion  [{0}], Message [{1}] - StackTrace [{2}] ", e.Message, e.StackTrace);
                throw;
            }

            logger.Info("Saliendo del servicio AddCommentPosts. Publicacion Id [{0}]", postsId);
            return result;

        }

        /// <summary>
        /// Obteniendo el listado de publicaciones del escritor
        /// </summary>
        /// <param name="author"></param>
        /// <returns></returns>
        private List<Posts> GetListPosts(string author)
        {
            logger.Info("Iniciando servicio GetListPosts");

            List<Posts> result = new List<Posts>();
            try
            {
                MySqlConnection conn = new MySqlConnection(_connString);
                conn.Open();

                logger.Debug("Vamos a ejecutar el siguiente qry [{0}]", QRY_GET_POSTS);
                MySqlCommand cmd = new MySqlCommand(QRY_GET_POSTS, conn);
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@paramAuthor", author);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    logger.Debug("Vamos a leer los registros obtenidos");
                    while (reader.Read())
                    {
                        Posts posts = new Posts();
                        var postsId = (reader.GetValue(reader.GetOrdinal("id_posts")).ToString() != null) ? reader.GetValue(reader.GetOrdinal("id_posts")).ToString().Trim() : null;
                        var status = (reader.GetValue(reader.GetOrdinal("status")).ToString() != null) ? reader.GetValue(reader.GetOrdinal("status")).ToString().Trim() : null;
                        var commentAmount = this.GetAmountCommet(Int32.Parse(postsId));
                        var title = (reader.GetValue(reader.GetOrdinal("title")).ToString() != null) ? reader.GetValue(reader.GetOrdinal("title")).ToString().Trim() : null;
                        var content = (reader.GetValue(reader.GetOrdinal("content")).ToString() != null) ? reader.GetValue(reader.GetOrdinal("content")).ToString().Trim() : null;
                        var date = (reader.GetValue(reader.GetOrdinal("date")).ToString() != null) ? reader.GetValue(reader.GetOrdinal("date")).ToString().Trim() : null;

                        posts.PostsId = Convert.ToInt32(postsId);
                        posts.Status = status;
                        posts.AmountComment = commentAmount;
                        posts.Title = title;
                        posts.Content = content;
                        posts.DatePosts = DateTime.Parse(date);
                        posts.Author = author;

                        result.Add(posts);
                    }
                    logger.Debug("Luego de obtener los registros obtenidos. Obtuvimos [{0}] publicaciones publicadas.", result.Count);
                }
            }
            catch (Exception e)
            {
                logger.Error("Error al obtener la lista de publicaciones, Message [{0}] - StackTrace [{1}] ", e.Message, e.StackTrace);
                throw;
            }

            logger.Info("Saliendo del servicio GetListPosts");
            return result;
        }

        /// <summary>
        /// Inserta una publicacion
        /// </summary>
        /// <param name="posts">publicacion recivida</param>
        /// <returns></returns>
        private int AddWriterPosts(Posts posts)
        {
            logger.Info("Iniciando servicio AddWriterPosts. Publicacion Id [{0}]", posts.PostsId);

            int affectRow = 0;
            try
            {
                MySqlConnection conn = new MySqlConnection(_connString);
                conn.Open();

                logger.Debug("Vamos a ejecutar el siguiente qry [{0}]", QRY_POST_POSTS);
                MySqlCommand cmd = new MySqlCommand(QRY_POST_POSTS, conn);
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@paramStatus", posts.Status);
                cmd.Parameters.AddWithValue("@paramTitle", posts.Title);
                cmd.Parameters.AddWithValue("@paramContent", posts.Content);
                cmd.Parameters.AddWithValue("@paramDate", posts.DatePosts);
                cmd.Parameters.AddWithValue("@paramBlocked", posts.Blocked);
                affectRow = cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                logger.Error("Error al insertar una publicacion  [{0}], Message [{1}] - StackTrace [{2}] ", e.Message, e.StackTrace);
                throw;
            }

            logger.Info("Saliendo del servicio AddWriterPosts. Publicacion Id [{0}]", posts.PostsId);
            return affectRow;
        }

        /// <summary>
        /// Edita una publicacion de un escritor
        /// </summary>
        /// <param name="posts">publicacion enviada</param>
        /// <returns></returns>
        private int EditWriterPost(Posts posts)
        {
            logger.Info("Iniciando servicio EditWriterPost. Publicacion Id [{0}]", posts.PostsId);

            int affectRow = 0;
            try
            {
                MySqlConnection conn = new MySqlConnection(_connString);
                conn.Open();

                logger.Debug("Vamos a ejecutar el siguiente qry [{0}]", QRY_UPDATE_POSTS);
                MySqlCommand cmd = new MySqlCommand(QRY_UPDATE_POSTS, conn);
                cmd.Prepare();

                cmd.Parameters.AddWithValue("@paramTitle", posts.Title);
                cmd.Parameters.AddWithValue("@paramContent", posts.Content);
                cmd.Parameters.AddWithValue("@paramIdPosts", posts.PostsId);

                affectRow = cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                logger.Error("Error al actalizar una publicacion del escritor [{0}], Message [{1}] - StackTrace [{2}] ", e.Message, e.StackTrace);
                throw;
            }

            logger.Info("Saliendo del servicio EditWriterPost. Publicacion Id [{0}]", posts.PostsId);
            return affectRow;
        }

        /// <summary>
        /// Obteniendo el rol de un autor
        /// </summary>
        /// <param name="author">autor recibido</param>
        /// <returns></returns>
        private string GetRolesAuthor(string author)
        {
            logger.Info("Iniciando servicio GetRolesAuthor. Autor Id [{0}]", author);

            string result = string.Empty;
            try
            {
                MySqlConnection conn = new MySqlConnection(_connString);
                conn.Open();

                logger.Debug("Vamos a ejecutar el siguiente qry [{0}]", QRY_GET_AUTHOR_ROL);
                MySqlCommand cmd = new MySqlCommand(QRY_GET_AUTHOR_ROL, conn);
                cmd.Parameters.AddWithValue("@paramTitle", author);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    logger.Debug("Vamos a leer los registros obtenidos");
                    while (reader.Read())
                    {
                        result = (reader.GetValue(reader.GetOrdinal("roles")).ToString() != null) ? reader.GetValue(reader.GetOrdinal("roles")).ToString().Trim() : string.Empty;
                    }
                    logger.Debug("Luego de obtener los registros obtenidos. Obtuvimos [{0}] estado .", result);
                }
            }
            catch (Exception e)
            {
                logger.Error("Error al obtener el rol de un autor [{0}], Message [{1}] - StackTrace [{2}] ", e.Message, e.StackTrace);
                throw;
            }

            logger.Info("Saliendo del servicio GetRolesAuthor. Autor Id [{0}]", author);
            return result;
        }

        private int SubmitWriterPosts(Posts posts)
        {
            logger.Info("Iniciando servicio SubmitWriterPosts. Publicacion Id [{0}]", posts.PostsId);

            int affectRow = 0;
            try
            {
                MySqlConnection conn = new MySqlConnection(_connString);
                conn.Open();

                logger.Debug("Vamos a ejecutar el siguiente qry [{0}]", QRY_SUBMIT_POSTS);
                MySqlCommand cmd = new MySqlCommand(QRY_SUBMIT_POSTS, conn);
                cmd.Prepare();

                cmd.Parameters.AddWithValue("@paramTitle", posts.Title);
                cmd.Parameters.AddWithValue("@paramContent", posts.Content);
                cmd.Parameters.AddWithValue("@paramIdPosts", posts.PostsId);
                cmd.Parameters.AddWithValue("@paramStatus", EnumStatePosts.PendingApproval.ToString());

                affectRow = cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                logger.Error("Error al enviar una publicacion del escritor [{0}], Message [{1}] - StackTrace [{2}] ", e.Message, e.StackTrace);
                throw;
            }

            logger.Info("Saliendo del servicio SubmitWriterPosts. Publicacion Id [{0}]", posts.PostsId);
            return affectRow;

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
            logger.Info("Iniciando servicio ProcessPostsResponse.");

            ProcessPostsResponse result = new ProcessPostsResponse();
            //Verifico que el autor sea un escritor
            var rol = this.GetRolesAuthor(author);
            if (rol != EnumRoles.WriterRol.ToString())
            {
                result.State = ResultState.ERROR;

                //aca lo ideal seria tener una libreria que se encargue del manejo de los mnesajes de error 
                //por falta de tiempo coloco aca.
                result.Message = " Autor con un Rol no válido.";
            }

            int affectRow = 0;
            EnumActionsPosts enumValues;
            try
            {
                if (Enum.TryParse(actionProcess, out enumValues))
                {
                    switch (enumValues)
                    {
                        case EnumActionsPosts.GetPosts:
                            result.Details = this.GetListPosts(author);
                            result.State = ResultState.OK;
                            result.Message = "Obteniendo listado de publicaciones del Autor" + author + "exitoso";
                            break;
                        case EnumActionsPosts.CreatePosts:
                            affectRow = this.AddWriterPosts(posts);
                            if (affectRow == 1)
                            {
                                result.State = ResultState.OK;
                                result.Message = "Insertando publicación del Autor" + author + "exitoso";
                            }
                            else {
                                result.State = ResultState.ERROR;
                                result.Message = "Ocurrió  un error Insertando publicación del Autor" + author + "exitoso";
                            }
                            break;
                        case EnumActionsPosts.EditPosts:
                            //Verifico que este en estado publicado
                            string status = this.GetStatusPost(posts.PostsId);
                            if (status == EnumStatePosts.Published.ToString() || status == EnumStatePosts.Submitted.ToString())
                            {
                                result.State = ResultState.ERROR;

                                //aca lo ideal seria tener una libreria que se encargue del manejo de los mnesajes de error 
                                //por falta de tiempo coloco aca.
                                result.Message = " Publicación en Estado no válido para editar por un escritor.";
                            }

                            affectRow = this.EditWriterPost(posts);
                            if (affectRow == 1)
                            {
                                result.State = ResultState.OK;
                                result.Message = "Editando publicación del Autor" + author + "exitoso";
                            }
                            else
                            {
                                result.State = ResultState.ERROR;
                                result.Message = "Ocurrió  un error Editando publicación del Autor" + author + "exitoso";
                            }
                            break;
                        case EnumActionsPosts.SubmitPosts:
                            affectRow = this.SubmitWriterPosts(posts);
                            if (affectRow == 1)
                            {
                                result.State = ResultState.OK;
                                result.Message = "Envio de publicación del Autor" + author + "exitoso";
                            }
                            else
                            {
                                result.State = ResultState.ERROR;
                                result.Message = "Ocurrió  un error Enviando publicación del Autor" + author + "exitoso";
                            }
                            break;
                        default:
                            result.State = ResultState.ERROR;
                            result.Message = "Acción recibida no válida";
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Error al obtener la lista de publicaciones publicados, Message [{0}] - StackTrace [{1}] ", e.Message, e.StackTrace);
                throw;
            }
            logger.Info("Saliendo de servicio ProcessPostsResponse.");
            return result;
        }
    }
}