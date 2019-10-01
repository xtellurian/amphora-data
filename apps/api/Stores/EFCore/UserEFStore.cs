// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Linq.Expressions;
// using System.Threading.Tasks;
// using Amphora.Api.Contracts;
// using Amphora.Api.DbContexts;
// using Amphora.Common.Models.Users;
// using Amphora.Common.Models.UserData;
// using Microsoft.EntityFrameworkCore;

// namespace Amphora.Api.Stores.EFCore
// {
//     public class UserEFStore : IEntityStore<ApplicationUser>
//     {
//         private readonly AmphoraContext context;
//         private readonly IUserService userService;

//         public UserEFStore(AmphoraContext context, IUserService userService)
//         {
//             this.context = context;
//             this.userService = userService;
//         }

//         public async Task<ApplicationUser> CreateAsync(ApplicationUser entity)
//         {
//             throw new ArgumentException("Do not create a user this way");
//         }

//         public async Task DeleteAsync(UserModel entity)
//         {
//             context.Remove(entity);
//             await context.SaveChangesAsync();
//         }

//         public async Task<IEnumerable<UserModel>> QueryAsync(Expression<Func<UserModel, bool>> where)
//         {
//             return await context.Users.Where(where).ToListAsync();
//         }

//         public async Task<UserModel> ReadAsync(string id, bool includeChildren = false)
//         {
//             return await context.Users.FirstOrDefaultAsync(u => u.Id == id);
//         }

//         public async Task<IList<UserModel>> TopAsync()
//         {
//             return await context.Users.Take(10).ToListAsync();
//         }

//         public async Task<IList<UserModel>> TopAsync(string orgId)
//         {
//             return await context.Users.Where(u => u.OrganisationId == orgId).Take(10).ToListAsync();
//         }

//         public async Task<UserModel> UpdateAsync(UserModel entity)
//         {
//             await context.SaveChangesAsync();
//             return entity;
//         }
//     }
// }