using Dapper;using System;using System.Collections.Generic;using System.Data;using System.Linq;


namespace Finstro.Serverless.Dapper.Repository.Extensions
{
    public static class DapperExtensions    {        public static T Insert<T>(this IDbConnection cnn, string tableName, string primaryKey, dynamic param)        {            var query = DynamicQuery.GetInsertQuery(tableName, primaryKey, param);            List<T> ret = new List<T>();            try            {                IEnumerable<T> insert = SqlMapper.Query<T>(cnn, query, param);                if (!string.IsNullOrEmpty(primaryKey))                {                    var result = SqlMapper.Query<T>(cnn, "SELECT IDENT_CURRENT('" + tableName.ToUpper() + "') ");                    return result.First();                }                return ret.FirstOrDefault();            }            catch (Exception ex)            {                return ret.FirstOrDefault();            }        }        public static void Update(this IDbConnection cnn, string tableName, string primaryKey, dynamic param)        {            var query = DynamicQuery.GetUpdateQuery(tableName, primaryKey, param);            try            {                SqlMapper.Execute(cnn, query, param);            }            catch (Exception ex)
            {                throw ex;            }



        }    }
}
