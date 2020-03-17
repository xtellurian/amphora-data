using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Common.Contracts;
using Amphora.Common.Extensions;
using Amphora.Common.Models;
using Amphora.Common.Models.Users;

namespace Amphora.Common.Services.Users
{
    public class ApplicationUserDataService : IUserDataService
    {
        private readonly IEntityStore<ApplicationUserDataModel> store;

        public ApplicationUserDataService(IEntityStore<ApplicationUserDataModel> store)
        {
            this.store = store;
        }

        public async Task<EntityOperationResult<ApplicationUserDataModel>> CreateAsync(ClaimsPrincipal principal, ApplicationUserDataModel userData)
        {
            var userId = principal.GetUserId(); // might still be null
            userData.Id ??= userId!;

            if (userData.Id == null)
            {
                throw new NullReferenceException("User ID Cannot be null!");
            }

            userData = await store.CreateAsync(userData);
            return new EntityOperationResult<ApplicationUserDataModel>(userData, userData);
        }

        public IQueryable<ApplicationUserDataModel> Query(ClaimsPrincipal principal, Expression<Func<ApplicationUserDataModel, bool>> where)
        {
            return store.Query(where);
        }

        public async Task<EntityOperationResult<ApplicationUserDataModel>> ReadAsync(ClaimsPrincipal principal, string? userId = null)
        {
            userId ??= principal.GetUserId();
            var userData = userId != null ? await store.ReadAsync(userId) : null; // don't search if null, just be null
            if (userData is null)
            {
                return new EntityOperationResult<ApplicationUserDataModel>($"Unknown User Id: {userId}");
            }
            else
            {
                return new EntityOperationResult<ApplicationUserDataModel>(userData, userData);
            }
        }

        public async Task<EntityOperationResult<ApplicationUserDataModel>> UpdateAsync(ClaimsPrincipal principal, ApplicationUserDataModel userData)
        {
            if (userData is null)
            {
                throw new ArgumentNullException(nameof(userData));
            }

            var userId = principal.GetUserId();
            if (userData.Id != userId)
            {
                // you can only update your own user data
                return new EntityOperationResult<ApplicationUserDataModel>("You can only update your own user data.");
            }
            else
            {
                userData = await store.UpdateAsync(userData);
                return new EntityOperationResult<ApplicationUserDataModel>(userData, userData);
            }
        }
    }
}