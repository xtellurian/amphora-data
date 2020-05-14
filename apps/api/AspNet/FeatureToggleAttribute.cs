using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;

namespace Amphora.Api.AspNet
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class FeatureToggleAttribute : FeatureGateAttribute
    {
        public FeatureToggleAttribute(params string[] features)
            : this(RequirementType.All, features)
        {
        }

        public FeatureToggleAttribute(RequirementType requirementType, params string[] features)
            : base(requirementType, features)
        {
        }

        public FeatureToggleAttribute(RequirementType requirementType, params object[] features)
            : base(requirementType, features)
        {
        }

        public FeatureToggleAttribute(params object[] features)
            : this(RequirementType.All, features)
        {
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var fm = context.HttpContext.RequestServices.GetRequiredService<IFeatureManagerSnapshot>();
            bool flag = false; // default false
            if (RequirementType == RequirementType.All)
            {
                foreach (var f in Features)
                {
                    // if any are disabled, its false
                    if (!await fm.IsEnabledAsync(f))
                    {
                        flag = false;
                        break;
                    }
                    else
                    {
                        flag = true; // flag must be enabled
                    }
                }
            }
            else
            {
                foreach (var f in Features)
                {
                    // if any are enabled, its true
                    if (await fm.IsEnabledAsync(f))
                    {
                        flag = true;
                        break;
                    }
                }
            }

            if (flag)
            {
                await next().ConfigureAwait(false);
            }
            else
            {
                context.HttpContext.Response.StatusCode = 404;
                await context.HttpContext.Response.CompleteAsync();
            }
        }
    }
}