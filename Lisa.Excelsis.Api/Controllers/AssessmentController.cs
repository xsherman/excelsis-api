﻿using System;
using System.Threading.Tasks;
using Lisa.Common.WebApi;
using Microsoft.AspNetCore.Mvc;

namespace Lisa.Excelsis.Api
{
    [Route("assessments")]
    public class AssessmentController : Controller
    {
        public AssessmentController(Database database)
        {
            _db = database;
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] string sort, [FromQuery] string order)
        {
            if (!string.IsNullOrWhiteSpace(sort))
            {
                var sorting = sort.Split(new[] { ',' });
                string[] ordering = new string[sorting.Length];
                for (int i = 0; i < ordering.Length; i++)
                {
                    ordering[i] = "asc";
                }
                if (!string.IsNullOrWhiteSpace(order))
                {
                    int orderCount = 0;
                    foreach (var orderValue in order.Split(new[] { ',' }))
                    {
                        if (orderCount == sorting.Length)
                        {
                            break;
                        }
                        ordering[orderCount] = orderValue;
                        orderCount++;
                    }
                }

                Sorting sortingObj = new Sorting(sorting, ordering);

                var result = await _db.FetchAssessments(sortingObj);

                return new OkObjectResult(result);
            }
            var assessments = await _db.FetchAssessments();
            return new OkObjectResult(assessments);
        }

        [HttpGet("{id}", Name = "getSingle")]
        public async Task<ActionResult> Get(Guid id)
        {
            var result = await _db.FetchAssessment(id);

            if (result == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] DynamicModel assessment)
        {
            if ( assessment == null )
            {
                return new BadRequestResult();
            }
            
            var validationResult = new AssessmentValidator().Validate(assessment);
            if (validationResult.HasErrors)
            {
                return new UnprocessableEntityObjectResult(validationResult.Errors);
            }
            dynamic result = await _db.PostAssessment(assessment);
            var location = Url.RouteUrl("getSingle", new { Id = result.Id }, Request.Scheme);

            return new CreatedResult(location, result);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(Guid id, [FromBody] Patch[] patches)
        {
            if (patches == null)
            {
                return new BadRequestResult();
            }

            var assessment = await _db.FetchAssessment(id);

            if (assessment == null)
            {
                return new NotFoundResult();
            }
            var validationResult = new AssessmentValidator().Validate(patches, assessment);
            if (validationResult.HasErrors)
            {
                return new UnprocessableEntityObjectResult(validationResult.Errors);
            }

            var patcher = new ModelPatcher();

            patcher.Apply(patches, assessment);

            await _db.PatchAssessment(assessment);
            return new OkObjectResult(assessment);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete()
        {
            await _db.DeleteAssessments();
            return new StatusCodeResult(204);
        }

        private Database _db;
    }
}
