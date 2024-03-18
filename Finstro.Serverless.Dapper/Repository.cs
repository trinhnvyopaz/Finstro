﻿using Dapper;
using System;
using Finstro.Serverless.Dapper.Repository.Extensions;

namespace Finstro.Serverless.Dapper
{
    public abstract class Repository<T> where T : class

                        //if (p.PropertyType == typeof(bool) || p.PropertyType == typeof(System.Nullable<bool>))
                        //    value = Convert.ToInt16(value);

                        param.Add(name: ("p_" + p.Name).ToUpper(), value: value, direction: ParameterDirection.Input);

            // extract the dynamic sql query and parameters from predicate
            QueryResult result = DynamicQuery.GetDynamicQuery(_tableName, predicate);
}