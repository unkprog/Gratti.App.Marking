using System;
using System.Data.SqlClient;
using Gratti.App.Marking.Api.Model;
using Gratti.App.Marking.Extensions;

namespace Gratti.App.Marking.Views.Controls.Oms.Models
{
    partial class OrdersViewModel
    {

        public void SaveCisTrue(string ConnectionString, string cistrue)
        {
            if (string.IsNullOrEmpty(cistrue))
                return;

            DataMatrixModel model = new DataMatrixModel(cistrue);

            int id = SaveCis(ConnectionString, model);
            byte[] img = Core.DataMatrix.Encoder.EncodeToBytes(string.IsNullOrEmpty(model.CisTrue) ? model.Cis : model.CisTrue, 200);
            SaveDataMatrixBitmap(ConnectionString, id, img);
        }

        private int SaveCis(string ConnectionString, DataMatrixModel model)
        {
            int result = 0;
            string sqlCmd = string.Concat("declare @DocID int"
                   , Environment.NewLine, "select top 1 @DocID = [DocID] from [dbo].[СписокКМ] where [Cis] = @Cis"
                   , Environment.NewLine, "if isnull(@DocID,0) = 0"
                   , Environment.NewLine, "begin"
                   , Environment.NewLine, "  select @DocID = isnull(max([DocID]), 0) + 1 from [dbo].[СписокКМ] with(nolock)"
                   , Environment.NewLine, "  insert into [dbo].[СписокКМ] ([DocID], [ExtID], [Deleted], [ModifyDate], [Version], [Cis], [Gtin], [SGtin], [Uit], [CisTrue])"
                   , Environment.NewLine, "  select [DocID] = @DocID, [ExtID] = ltrim(rtrim(str(@DocID))), [Deleted] = 0, [ModifyDate] = getdate(), [Version] = 0"
                   , Environment.NewLine, "       , [Cis] = @Cis, [Gtin] = @Gtin, [SGtin] = @SGtin, [Uit] = @Uit, [CisTrue] = @CisTrue"
                   , Environment.NewLine, "end"
                   , Environment.NewLine, "else"
                   , Environment.NewLine, "  update [dbo].[СписокКМ] set [ModifyDate] = getdate(), [Cis] = @Cis, [Gtin] = @Gtin, [SGtin] = @SGtin, [Uit] = @Uit, [CisTrue] = @CisTrue"
                   , Environment.NewLine, "  where [DocID] = @DocID"
                   , Environment.NewLine, "select @DocID");


            Core.Sql.Mssql.ExecuteQuery(ConnectionString, sqlCmd
                , new SqlParameter[]
                {
                        new SqlParameter("@Cis", model.Cis),
                        new SqlParameter("@Gtin", SaveGtin(ConnectionString, model)),
                        new SqlParameter("@SGtin", model.Sgtin),
                        new SqlParameter("@Uit", model.Uit),
                        new SqlParameter("@CisTrue", model.CisTrue),
                }
                , null
                , (values) => {
                    result = values[0].ToInt();
                });

            return result;
        }

        private int SaveGtin(string ConnectionString, DataMatrixModel model)
        {
            int result = 0;
            string sqlCmd = string.Concat("declare @DocID int"
                   , Environment.NewLine, "select top 1 @DocID = [DocID] from [dbo].[GTIN] where [Gtin] = @Gtin"
                   , Environment.NewLine, "if isnull(@DocID,0) = 0"
                   , Environment.NewLine, "begin"
                   , Environment.NewLine, "  select @DocID = isnull(max([DocID]), 0) + 1 from [dbo].[GTIN] with(nolock)"
                   , Environment.NewLine, "  insert into [dbo].[GTIN] ([DocID], [ExtID], [Deleted], [ModifyDate], [Version], [Gtin], [Тип])"
                   , Environment.NewLine, "  select [DocID] = @DocID, [ExtID] = ltrim(rtrim(str(@DocID))), [Deleted] = 0, [ModifyDate] = getdate(), [Version] = 0"
                   , Environment.NewLine, "       , [Gtin] = @Gtin, [Тип] = 1"
                   , Environment.NewLine, "end"
                   , Environment.NewLine, "select @DocID");


            Core.Sql.Mssql.ExecuteQuery(ConnectionString, sqlCmd
                , new SqlParameter[]
                {
                        new SqlParameter("@Gtin", model.Gtin),
                }
                , null
                , (values) => {
                    result = values[0].ToInt();
                });
            return result;
        }

        private void SaveDataMatrixBitmap(string ConnectionString, int id, byte[] img)
        {
            string sqlCmd = string.Concat("declare @DocID int"
                   , Environment.NewLine, "select top 1 @DocID = [DocID] from [dbo].[КМImage] where [КМ] = @id"
                   , Environment.NewLine, "if isnull(@DocID,0) = 0"
                   , Environment.NewLine, "begin"
                   , Environment.NewLine, "  select @DocID = isnull(max([DocID]), 0) + 1 from [dbo].[КМImage] with(nolock)"
                   , Environment.NewLine, "  insert into [dbo].[КМImage] ([DocID], [ExtID], [Deleted], [ModifyDate], [Version], [КМ], [Img])"
                   , Environment.NewLine, "  select [DocID] = @DocID, [ExtID] = ltrim(rtrim(str(@DocID))), [Deleted] = 0, [ModifyDate] = getdate(), [Version] = 0"
                   , Environment.NewLine, "       , [КМ] = @id, [Img] = @Img"
                   , Environment.NewLine, "end"
                   , Environment.NewLine, "else"
                   , Environment.NewLine, "  update [dbo].[КМImage] set [ModifyDate] = getdate(), [Img] = @Img"
                   , Environment.NewLine, "  where [DocID] = @DocID"
                   , Environment.NewLine, "select @DocID");
            Core.Sql.Mssql.ExecuteQuery(ConnectionString, sqlCmd
                   , new SqlParameter[]
                   {
                        new SqlParameter("@id", id),
                        new SqlParameter("@Img", img)
                   }
                   , null
                   , (values) => {

                   });

        }
    }
}
