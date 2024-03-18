﻿using Dapper;

namespace Finstro.Serverless.Dapper.Repository.Extensions
{

    /// <summary>

        /// <summary>

            string[] columns = param.ParameterNames.Where(s => s != "P_" + primaryKey.ToUpper()).Select(p => @"""" + p.Replace("P_", "") + @"""").ToArray();
            //          
            return sql.ToUpper();


        /*

        /// <summary>

            var parameters = columns.Where(p => p != primaryKey).Select(name => name + "=@P_" + name.Replace(@"""", "")).ToList();

        /// <summary>

            // walk the tree and build up a list of query parameter objects
            // from the left and right branches of the expression tree
            WalkTree(body, ExpressionType.Default, ref queryProperties);

            // convert the query parms into a SQL string and dynamic property object
            builder.Append("SELECT * FROM ");


        /// <summary>
                                   ref List<QueryParameter> queryProperties)

        /// <summary>
                // hack to remove the trailing ) when convering.
                propertyName = propertyName.Replace(")", string.Empty);

        /// <summary>


    /// <summary>


        /// <summary>

}