using MediatR;
using Microsoft.AspNetCore.Mvc;
using Talaby.Application.Features.StoreCategories.Commands.CreateStoreCategory;
using Talaby.Application.Features.StoreCategories.Commands.DeleteStoreCategory;
using Talaby.Application.Features.StoreCategories.Commands.UpdateStoreCategory;
using Talaby.Application.Features.StoreCategories.Queries.GetAllStoreCategories;
using Talaby.Application.Features.StoreCategories.Queries.GetStoreCategoryById;
using Talaby.Domain.Entities;

namespace Talaby.API.Controllers
{
    [Route("api/storeCategories")]
    //[Authorize]
    public class StoreCategoriesController(IMediator mediator) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StoreCategory>>> GetAll([FromQuery] GetAllStoreCategoriesQuery query)
        {
            var storeCategories = await mediator.Send(query);
            return OkResponse(storeCategories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StoreCategory?>> GetById([FromRoute] int id)
        {
            var storeCategory = await mediator.Send(new GetStoreCategoryByIdQuery(id));
            return OkResponse(storeCategory);
        }

        [HttpPost]
        //[Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> CreateStoreCategory(CreateStoreCategoryCommand command)
        {
            int id = await mediator.Send(command);

            return CreatedResponse(nameof(GetById), new { id }, id);
        }


        [HttpPatch()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStoreCategory(UpdateStoreCategoryCommand command)
        {
            await mediator.Send(command);

            return OkResponse("Updated successfully");


        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStoreCategory([FromRoute] int id)
        {
            await mediator.Send(new DeleteStoreCategoryCommand(id));

            return OkResponse("Deleted successfully");

        }

    }


}
