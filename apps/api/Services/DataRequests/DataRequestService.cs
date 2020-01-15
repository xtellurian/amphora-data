using System.Security.Claims;
using System.Threading.Tasks;
using Amphora.Api.Contracts;
using Amphora.Api.Models;
using Amphora.Common.Models.DataRequests;
using Amphora.Common.Models.Users;

namespace Amphora.Api.Services.DataRequests
{
    public class DataRequestService : IPermissionedEntityStore<DataRequestModel>
    {
        private readonly IEntityStore<DataRequestModel> dataRequestStore;
        private readonly IUserService userService;

        public DataRequestService(IEntityStore<DataRequestModel> dataRequestStore, IUserService userService)
        {
            this.dataRequestStore = dataRequestStore;
            this.userService = userService;
        }

        public async Task<EntityOperationResult<DataRequestModel>> CreateAsync(ClaimsPrincipal principal, DataRequestModel model)
        {
            var user = await userService.ReadUserModelAsync(principal);

            model.CreatedById = user.Id;
            model.CreatedBy = user;

            model = await dataRequestStore.CreateAsync(model);

            return new EntityOperationResult<DataRequestModel>(user, model);
        }

        public async Task<EntityOperationResult<DataRequestModel>> ReadAsync(ClaimsPrincipal principal, string id)
        {
            var user = await userService.ReadUserModelAsync(principal);
            var model = await dataRequestStore.ReadAsync(id);
            return new EntityOperationResult<DataRequestModel>(user, model);
        }

        public async Task<EntityOperationResult<DataRequestModel>> UpdateAsync(ClaimsPrincipal principal, DataRequestModel model)
        {
            // anyone can update the votes

            var user = await userService.ReadUserModelAsync(principal);
            var existingModel = await dataRequestStore.ReadAsync(model.Id);
            if (existingModel == null)
            {
                return new EntityOperationResult<DataRequestModel>(user, "Entity not found");
            }

            if (CheckPermissionToUpdate(model, user))
            {
                // only the user who created can delete
                model = await dataRequestStore.UpdateAsync(model);
                return new EntityOperationResult<DataRequestModel>(user, model);
            }
            else
            {
                return new EntityOperationResult<DataRequestModel>(user, "You can only update your own requests.") { WasForbidden = true };
            }
        }

        private bool CheckPermissionToUpdate(DataRequestModel model, ApplicationUser user)
        {
            if (user.Id == model.CreatedById)
            {
                return true;
            }

            if (this.dataRequestStore.IsModified(model, _ => _.UserIdVotes))
            {
                // if only votes are updating
                if (this.dataRequestStore.IsModified(model, _ => _.Name)) { return false; }
                if (this.dataRequestStore.IsModified(model, _ => _.Description)) { return false; }
                if (this.dataRequestStore.IsModified(model, _ => _.GeoLocation)) { return false; }

                return true;
            }

            return false; // default
        }

        public async Task<EntityOperationResult<DataRequestModel>> DeleteAsync(ClaimsPrincipal principal, DataRequestModel model)
        {
            model = await dataRequestStore.ReadAsync(model.Id); // prevents deletion of modified entity
            var user = await userService.ReadUserModelAsync(principal);
            if (user.Id == model.CreatedById)
            {
                // only the user who created can delete
                await dataRequestStore.DeleteAsync(model);
                return new EntityOperationResult<DataRequestModel>(user, true);
            }
            else
            {
                return new EntityOperationResult<DataRequestModel>(user, "You can only delete your own requests.") { WasForbidden = true };
            }
        }
    }
}