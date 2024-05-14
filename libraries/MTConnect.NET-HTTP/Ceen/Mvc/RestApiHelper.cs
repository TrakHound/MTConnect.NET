using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ceen.Mvc
{
	/// <summary>
	/// A basic class for providing a standard response
	/// </summary>
	public class ResponseEnvelope
	{
		/// <summary>
		/// The status message, should be &quot;OK&quot;
		/// </summary>
		[JsonPropertyName("status")]
		public string Status = "OK";
	}

	/// <summary>
	/// A basic class for providing pagination context
	/// </summary>
	public class PageInformation : ResponseEnvelope
	{
		/// <summary>
		/// The current page
		/// </summary>
		[JsonPropertyName("page")]
		public int? Page;
		/// <summary>
		/// The number of pages
		/// </summary>
		[JsonPropertyName("pagecount")]
		public int? PageCount;
		/// <summary>
		/// The number of items in this result
		/// </summary>
		[JsonPropertyName("itemcount")]
		public int? ItemCount;
		/// <summary>
		/// The total number of items
		/// </summary>
		[JsonPropertyName("totalitemcount")]
		public int? TotalItemCount;

		/// <summary>
		/// The actual items
		/// </summary>
		[JsonPropertyName("items")]
		public Array Items;
	}

	/// <summary>
	/// Defines the REST access methods
	/// </summary>
    public enum RestApiAccess
    {
		/// <summary>Lists all or some of the items</summary>
        List,
        /// <summary>Gets a single item by ID</summary>
        Get,
        /// <summary>Adds an item</summary>
        Add,
        /// <summary>Updates an existing item</summary>
        Update,
        /// <summary>Deletes an existing item</summary>
        Delete
    }

	/// <summary>
	/// A base class for providing standard REST access to a resource
	/// </summary>
	public abstract class RestApiHelper<TData, TKey> : Controller
	{
		[HttpGet]
		[Name("index")]
		/// <summary>
		/// Handles a GET request, using the &quot;index&quot; name,
		/// because we want two different functions to handle
		/// PUT, POST and GET, and they cannot all be called &quot;Index&quot;.
		/// </summary>
		/// <param name="context">The http context.</param>
		public virtual async Task<IResult> GetIndex(IHttpContext context, int page = 0, int results = 50, string query = null)
		{
            CheckAccess(RestApiAccess.List, context);

            if (page < 0)
				return Status(HttpStatusCode.BadRequest, "Page must be a positive integer");

			if (results <= 0)
				return Status(HttpStatusCode.BadRequest, "Results must be a positive integer");

			if (results > MaxResults)
				return Status(HttpStatusCode.BadRequest, $"Results must be less than {MaxResults}");

			return Json(await ListItemsAsync(page, results, query));
		}

		/// <summary>
		/// The maximum number of results a list request can ask for
		/// </summary>
		protected virtual int MaxResults { get { return 100; } }

		/// <summary>
		/// Lists the items in a page.
		/// </summary>
		/// <returns>The items.</returns>
		/// <param name="page">The page to return.</param>
		/// <param name="results">The number of results per page.</param>
		/// <param name="query">An optional query expression</param>
		protected abstract Task<PageInformation> ListItemsAsync(int page, int results, string query);

		/// <summary>
		/// Callback method to selectively grant or deny access to a given operation.
		/// Defaults to allowing all operations unconditionally.
		/// </summary>
		/// <param name="access">The operation being performed</param>
		/// <param name="context">The request context</param>
		protected virtual void CheckAccess(RestApiAccess access, IHttpContext context)
		{ }

		[HttpPost]
		[Name("index")]
		/// <summary>
		/// Handles a POST request, using the &quot;index&quot; name,
		/// because we want different functions to handle
		/// PUT, POST and GET, and they cannot all be called &quot;Index&quot;.
		/// </summary>
		/// <param name="context">The http context.</param>
		public virtual async Task<IResult> Post(IHttpContext context)
		{
			CheckAccess(RestApiAccess.Add, context);

			TData item;
			// TODO: Accept non-utf8 ?
            // TODO: Get the Json Async version
			using (var sr = new StreamReader(context.Request.Body, System.Text.Encoding.UTF8, false))
			{
				var str = await sr.ReadToEndAsync();
				item = JsonSerializer.Deserialize<TData>(str);
			}

			var res = await AddItemAsync(item);
			return res == null ? OK : Json(res);
		}

		/// <summary>
		/// Adds an item to the store
		/// </summary>
		/// <returns>T.</returns>
		/// <param name="item">The item to add.</param>
		protected abstract Task<TData> AddItemAsync(TData item);

		[HttpGet]
		[Route("{id}")]
		[Name("index")]
		/// <summary>
		/// Gets a todo item based on its index
		/// </summary>
		/// <returns>The HTTP results.</returns>
		/// <param name="context">The http context.</param>
		/// <param name="id">The index of the item, we limit it to only allow this to be specified via the path.</param>
		public virtual async Task<IResult> GetDetail(IHttpContext context, [Parameter(ParameterSource.Url)] TKey id)
		{
            CheckAccess(RestApiAccess.Get, context);

            if (!await ValidateIDAsync(id))
				return Status(HttpStatusCode.BadRequest, "Invalid ID");
			
			return Json(await GetItemAsync(id));
		}

		/// <summary>
		/// Validates an ID value
		/// </summary>
		/// <returns><c>True</c> if the ID is valid, false otherwise.</returns>
		/// <param name="id">The identifier to validate.</param>
		protected virtual Task<bool> ValidateIDAsync(TKey id)
		{
			return Task.FromResult(true);
		}

		/// <summary>
		/// Gets an item with a specific key
		/// </summary>
		/// <returns>The item with the given key.</returns>
		/// <param name="id">The key to find the item for.</param>
		protected abstract Task<TData> GetItemAsync(TKey id);

		[HttpPut]
		[Route("{id}")]
		[Name("index")]
		/// <summary>
		/// Updates an item based on its index
		/// </summary>
		/// <returns>The HTTP results.</returns>
		/// <param name="context">The http context.</param>
		/// <param name="id">The index of the item, we limit it to only allow this to be specified via the path.</param>
		public virtual async Task<IResult> PutDetail(IHttpContext context, TKey id)
		{
            CheckAccess(RestApiAccess.Update, context);

            if (!await ValidateIDAsync(id))
				return Status(HttpStatusCode.BadRequest, "Invalid ID");

			TData item;
			using (var sr = new StreamReader(context.Request.Body, System.Text.Encoding.UTF8, false))
			{
				var str = await sr.ReadToEndAsync();
				item = JsonSerializer.Deserialize<TData>(str);

			}

			var res = await UpdateItemAsync(id, item);
			return res == null ? OK : Json(res);
		}

		/// <summary>
		/// Updates an item
		/// </summary>
		/// <returns>An awaitable task.</returns>
		/// <param name="id">The identifier of the item to update</param>
		/// <param name="item">The item to update.</param>
		protected abstract Task<TData> UpdateItemAsync(TKey id, TData item);

		[HttpDelete]
		[Route("{id}")]
		[Name("index")]
		/// <summary>
		/// Deletes a todo item based on its index
		/// </summary>
		/// <returns>The HTTP results.</returns>
		/// <param name="context">The http context.</param>
		/// <param name="id">The index of the item, we limit it to only allow this to be specified via the path.</param>
		public async Task<IResult> DeleteDetail(IHttpContext context, TKey id)
		{
            CheckAccess(RestApiAccess.Delete, context);

            if (!await ValidateIDAsync(id))
				return Status(HttpStatusCode.BadRequest, "Invalid ID");

			await DeleteItemAsync(id);
			return OK;
		}	

		/// <summary>
		/// Deletes an item.
		/// </summary>
		/// <returns>An awaitable task.</returns>
		/// <param name="id">The identifier for the item to delete.</param>
		protected abstract Task DeleteItemAsync(TKey id);
	}
}
