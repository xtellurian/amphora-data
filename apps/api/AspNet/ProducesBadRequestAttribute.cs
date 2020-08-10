using System.Net;
using Amphora.Api.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Amphora.Api.AspNet
{
    internal class ProducesBadRequestAttribute : ProducesResponseTypeAttribute
    {
        public ProducesBadRequestAttribute() : base(typeof(Response), (int)HttpStatusCode.BadRequest)
        { }
    }
}