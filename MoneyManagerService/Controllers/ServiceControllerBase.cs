using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MoneyManagerService.Core;

namespace MoneyManagerService.Controllers
{
    public abstract class ServiceControllerBase : ControllerBase
    {
        [NonAction]
        public NotFoundObjectResult NotFound(string errorMessage)
        {
            return base.NotFound(new ProblemDetailsWithErrors(errorMessage, 404, Request));
        }

        [NonAction]
        public NotFoundObjectResult NotFound(IList<string> errorMessages)
        {
            return base.NotFound(new ProblemDetailsWithErrors(errorMessages, 404, Request));
        }

        [NonAction]
        public UnauthorizedObjectResult Unauthorized(string errorMessage)
        {
            return base.Unauthorized(new ProblemDetailsWithErrors(errorMessage, 401, Request));
        }

        [NonAction]
        public UnauthorizedObjectResult Unauthorized(IList<string> errorMessages)
        {
            return base.Unauthorized(new ProblemDetailsWithErrors(errorMessages, 401, Request));
        }

        [NonAction]
        public BadRequestObjectResult BadRequest(string errorMessage)
        {
            return base.BadRequest(new ProblemDetailsWithErrors(errorMessage, 400, Request));
        }

        [NonAction]
        public BadRequestObjectResult BadRequest(IList<string> errorMessages)
        {
            return base.BadRequest(new ProblemDetailsWithErrors(errorMessages, 400, Request));
        }

        [NonAction]
        public ObjectResult InternalServerError(string errorMessage)
        {
            return base.StatusCode(500, new ProblemDetailsWithErrors(errorMessage, 500, Request));
        }

        [NonAction]
        public ObjectResult InternalServerError(IList<string> errorMessages)
        {
            return base.StatusCode(500, new ProblemDetailsWithErrors(errorMessages, 500, Request));
        }
    }
}