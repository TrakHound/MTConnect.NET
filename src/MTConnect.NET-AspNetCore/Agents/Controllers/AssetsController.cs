// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MTConnect.Http;
using System;
using System.Threading.Tasks;

namespace MTConnect.Agents.Controllers
{
    [ApiController]
    [Route("/assets")]
    public class AssetsController : ControllerBase
    {
        private readonly IMTConnectAgent _agent;
        private readonly ILogger<AssetsController> _logger;

        public AssetsController(IMTConnectAgent agent, ILogger<AssetsController> logger)
        {
            _agent = agent;
            _logger = logger;
        }


        /// <summary>
        /// An Agent responds to an Asset Request with an MTConnectAssets Response Document that contains
        /// information for MTConnect Assets from the Agent, subject to any filtering defined in the Request.
        /// </summary>
        /// <param name="type">Defines the type of MTConnect Asset to be returned in the MTConnectAssets Response Document.</param>
        /// <param name="removed">
        /// An attribute that indicates whether the Asset has been removed from a piece of equipment.
        /// If the value of the removed parameter in the query is true, then Asset Documents for Assets that have been marked as removed from a piece of equipment will be included in the Response Document.
        /// If the value of the removed parameter in the query is false, then Asset Documents for Assets that have been marked as removed from a piece of equipment will not be included in the Response Document.
        /// </param>
        /// <param name="count">Defines the maximum number of Asset Documents to return in an MTConnectAssets Response Document.</param>
        /// <param name="version">The MTConnect Version of the response document</param>
        /// <param name="documentFormat">The format of the response document</param>
        /// <param name="indentOutput">A boolean flag to indent the response document (pretty)</param>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAssets(
            [FromQuery] string type = null,
            [FromQuery] bool removed = false,
            [FromQuery] int count = 100,
            [FromQuery] Version version = null,
            [FromQuery] DocumentFormat documentFormat = DocumentFormat.XML,
            [FromQuery] bool? indentOutput = null
            )
        {
            _logger.LogInformation($"[Api-Interface] : {Request.Host} : Requested a MTConnectAssets Document : [{Request.Method}] : {Request.Path} : {Request.QueryString}");

            // Set Indent Ouput (Pretty) parameter
            var indent = true;
            if (indentOutput != null) indent = indentOutput.Value;
            else if (_agent.Configuration != null) indent = _agent.Configuration.Pretty;

            var response = await MTConnectHttpRequests.GetAssetsRequest(_agent, type, removed, count, version, documentFormat, indent);

            _logger.LogInformation($"[Api-Interface] : {Request.Host} : [{Request.Method}] : {Request.Path} : Response ({response.StatusCode}) in {response.ResponseDuration}ms");

            return CreateResult(response);
        }


        private IActionResult CreateResult(MTConnectHttpResponse response)
        {
            return new ContentResult
            {
                Content = response.Content,
                ContentType = response.ContentType,
                StatusCode = response.StatusCode
            };
        }
    }
}
