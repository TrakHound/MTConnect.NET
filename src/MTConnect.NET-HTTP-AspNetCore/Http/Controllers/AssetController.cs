// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MTConnect.Agents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MTConnect.Http.Controllers
{
    [ApiController]
    [Route("/asset")]
    public class AssetController : ControllerBase
    {
        private readonly IMTConnectAgent _agent;
        private readonly ILogger<AssetController> _logger;

        public AssetController(IMTConnectAgent agent, ILogger<AssetController> logger)
        {
            _agent = agent;
            _logger = logger;
        }


        /// <summary>
        /// An Agent responds to an Asset Request with an MTConnectAssets Response Document that contains
        /// information for MTConnect Assets from the Agent, subject to any filtering defined in the Request.
        /// </summary>
        /// <param name="assetId">Identifies the id attribute of an MTConnect Asset to be provided by an Agent.</param>
        /// <param name="version">The MTConnect Version of the response document</param>
        /// <param name="documentFormat">The format of the response document</param>
        /// <param name="indentOutput">A boolean flag to indent the response document (pretty)</param>
        [HttpGet("{assetId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAsset(
            [FromRoute] string assetId,
            [FromQuery] Version version = null,
            [FromQuery] string documentFormat = DocumentFormat.XML,
            [FromQuery] bool? indentOutput = null,
            [FromQuery] bool? outputComments = null
            )
        {
            _logger.LogInformation($"[Api-Interface] : {Request.Host} : Requested a MTConnectAssets Document : [{Request.Method}] : {Request.Path} : {Request.QueryString}");

            // Set Format Options
            var formatOptions = new List<KeyValuePair<string, string>>();

            if (indentOutput.HasValue) formatOptions.Add(new KeyValuePair<string, string>("indentOutput", indentOutput.Value.ToString()));
            else formatOptions.Add(new KeyValuePair<string, string>("indentOutput", _agent.Configuration.Pretty.ToString()));

            if (outputComments.HasValue) formatOptions.Add(new KeyValuePair<string, string>("outputComments", outputComments.Value.ToString()));

            var response = await MTConnectHttpRequests.GetAssetRequest(_agent, assetId, version, documentFormat, formatOptions);

            _logger.LogInformation($"[Api-Interface] : {Request.Host} : [{Request.Method}] : {Request.Path} : Response ({response.StatusCode}) in {response.ResponseDuration}ms");

            return CreateResult(response);
        }

        [HttpPost("{assetId}")]
        [HttpPut("{assetId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PostAsset(
            [FromRoute] string assetId,
            [FromQuery] string device,
            [FromQuery] string type
            )
        {
            if (_agent != null)
            {
                using var reader = new StreamReader(Request.Body, Encoding.UTF8);
                var body = await reader.ReadToEndAsync();

                if (!string.IsNullOrEmpty(assetId) && !string.IsNullOrEmpty(device) && !string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(body))
                { 
                    // Create the Asset
                    var asset = new Assets.Asset();
                    asset.AssetId = assetId;
                    asset.Type = type;
                    asset.Xml = body;

                    // Store the Asset in the Agent
                    if (await _agent.AddAssetAsync(device, asset))
                    {
                        _logger?.LogInformation($"Asset Added via HTTP POST : {device} : {assetId} : {type}");

                        return new ContentResult
                        {
                            Content = "<success/>",
                            ContentType = "text/xml",
                            StatusCode = 200
                        };
                    }
                }
                else
                {
                    return BadRequest();
                }
            }

            return StatusCode(500);
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
