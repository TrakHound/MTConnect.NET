// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;

namespace MTConnect.Http.Controllers
{
    class DataItemsQueryFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var dataItems = new Dictionary<string, string>();
            foreach (var kvp in context.HttpContext.Request.Query)
            {
                if (!context.ActionArguments.ContainsKey(kvp.Key))
                {
                    dataItems.Add(kvp.Key, kvp.Value);
                }
            }

            base.OnActionExecuting(context);
        }
    }
}