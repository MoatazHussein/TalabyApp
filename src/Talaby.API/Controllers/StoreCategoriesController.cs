using MediatR;
using Microsoft.AspNetCore.Mvc;
using Talaby.Application.StoreCategories.Commands.CreateStoreCategory;
using Talaby.Application.StoreCategories.Commands.DeleteStoreCategory;
using Talaby.Application.StoreCategories.Commands.UpdateStoreCategory;
using Talaby.Application.StoreCategories.Queries.GetAllStoreCategories;
using Talaby.Application.StoreCategories.Queries.GetStoreCategoryById;
using Talaby.Domain.Entities;

namespace StoreCategorys.API.Controllers
{
    [ApiController]
    [Route("api/storeCategories")]
    //[Authorize]
    public class StoreCategoriesController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StoreCategory>>> GetAll([FromQuery] GetAllStoreCategoriesQuery query)
        {
            var storeCategories = await mediator.Send(query);
            return Ok(storeCategories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StoreCategory?>> GetById([FromRoute] int id)
        {
            var storeCategory = await mediator.Send(new GetStoreCategoryByIdQuery(id));
            return Ok(storeCategory);
        }

        [HttpPost]
        //[Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> CreateStoreCategory(CreateStoreCategoryCommand command)
        {
            int id = await mediator.Send(command);

            return CreatedAtAction(nameof(GetById), new { id }, null);
        }


        [HttpPatch()]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStoreCategory(UpdateStoreCategoryCommand command)
        {
            await mediator.Send(command);

            //return NoContent();
            return StatusCode(200,$"Updated successfully" );


        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStoreCategory([FromRoute] int id)
        {
            await mediator.Send(new DeleteStoreCategoryCommand(id));

            //return NoContent();
            return StatusCode(200, $"Deleted successfully");

        }

    }


}
