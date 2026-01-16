using Microsoft.AspNetCore.Mvc;
using MoviesMadeEasy.DAL.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using MoviesMadeEasy.Data;
using MoviesMadeEasy.DTOs;
using MoviesMadeEasy.Models;
namespace MoviesMadeEasy.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IOpenAIService _openAIService;
        private readonly IMovieService _movieService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<BaseController> _logger;
        private readonly ITitleRepository _titleRepository;

        public HomeController(
            IOpenAIService openAIService,
            IMovieService movieService,
            UserManager<IdentityUser> userManager,
            IUserRepository userRepository,
            ITitleRepository titleRepository,
            ILogger<BaseController> logger) : base(userManager, userRepository, logger)
        {
            _openAIService = openAIService;
            _movieService = movieService;
            _userManager = userManager;
            _userRepository = userRepository;
            _titleRepository = titleRepository;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Recommendations()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }

        public IActionResult ChatboxRedirect()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetSimilarMovies(string title)
        {
            try
            {
                var recommendations = await _openAIService.GetSimilarMoviesAsync(title);

                // Store the full recommendations in session for the recommendations page
                HttpContext.Session.SetString("LastRecommendations",
                    JsonSerializer.Serialize(recommendations));
                HttpContext.Session.SetString("LastRecommendationTitle", title);

                // Return the recommendations as JSON
                return Ok(recommendations);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
            {
                _logger.LogWarning("OpenAI rate limit exceeded for {Title}", title);
                return StatusCode(429, new
                {
                    error = "rate_limit_exceeded",
                    message = "We're getting too many requests. Please try again later."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting similar movies for {Title}", title);
                return StatusCode(500, new
                {
                    error = "server_error",
                    message = "Something went wrong. Please try again later."
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetChatResponse(string query)
        {
            try
            {
                var recommendations = await _openAIService.GetChatResponse(query);

                return Ok(recommendations);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
            {
                _logger.LogWarning("OpenAI rate limit exceeded for {Query}", query);
                return StatusCode(429, new
                {
                    error = "rate_limit_exceeded",
                    message = "We're getting too many requests. Please try again later."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting similar movies for {Title}", query);
                return StatusCode(500, new
                {
                    error = "server_error",
                    message = "Something went wrong. Please try again later."
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchMovies(string query, string sortBy, int? minYear, int? maxYear)
        {
            var requestId = HttpContext.TraceIdentifier;
            var startedAt = DateTimeOffset.UtcNow;

            try
            {
                _logger.LogInformation(
                    "INFO {TimestampUtc}: SearchMovies started | requestId={RequestId} | query='{Query}' | sortBy='{SortBy}' | minYear={MinYear} | maxYear={MaxYear}",
                    startedAt,
                    requestId,
                    query,
                    sortBy,
                    minYear,
                    maxYear);

                if (string.IsNullOrWhiteSpace(query))
                {
                    _logger.LogWarning(
                        "WARN {TimestampUtc}: SearchMovies rejected request {RequestId} due to missing query parameter",
                        DateTimeOffset.UtcNow,
                        requestId);

                    return BadRequest(new
                    {
                        error = "invalid_query",
                        message = "A search term is required to look up movies.",
                        requestId
                    });
                }

                query = query.Trim();
                var movies = await _movieService.SearchMoviesAsync(query);

                if (movies == null)
                {
                    _logger.LogWarning(
                        "WARN {TimestampUtc}: SearchMovies received null response from movie service | requestId={RequestId} | query='{Query}'",
                        DateTimeOffset.UtcNow,
                        requestId,
                        query);

                    return Ok(Array.Empty<object>());
                }

                _logger.LogInformation(
                    "INFO {TimestampUtc}: SearchMovies retrieved {ResultCount} raw results | requestId={RequestId}",
                    DateTimeOffset.UtcNow,
                    movies.Count,
                    requestId);

                if (minYear.HasValue)
                {
                    movies = movies.Where(m => m.ReleaseYear >= minYear.Value).ToList();
                }
                if (maxYear.HasValue)
                {
                    movies = movies.Where(m => m.ReleaseYear <= maxYear.Value).ToList();
                }

                movies = sortBy switch
                {
                    "yearAsc" => movies.OrderBy(m => m.ReleaseYear).ToList(),
                    "yearDesc" => movies.OrderByDescending(m => m.ReleaseYear).ToList(),
                    "titleAsc" => movies.OrderBy(m => m.Title).ToList(),
                    "titleDesc" => movies.OrderByDescending(m => m.Title).ToList(),
                    "ratingHighLow" => movies.OrderByDescending(m => m.Rating).ToList(),
                    "ratingLowHigh" => movies.OrderBy(m => m.Rating).ToList(),
                    _ => movies
                };

                var movieResults = movies.Select(movie => new
                {
                    title = movie.Title,
                    releaseYear = movie.ReleaseYear,
                    posterUrl = movie.ImageSet?.VerticalPoster?.W240 ?? "https://via.placeholder.com/150",
                    genres = movie.Genres?.Select(g => g.Name).ToList() ?? new List<string>(),
                    rating = movie.Rating,
                    overview = movie.Overview,
                    services = movie.StreamingOptions?
                        .SelectMany(kvp => kvp.Value)
                        .Select(option => option.Service?.Name)
                        .Where(name => name != null)
                        .Distinct()
                        .ToList() ?? new List<string>()
                }).ToList();

                _logger.LogInformation(
                    "INFO {TimestampUtc}: SearchMovies completed successfully | requestId={RequestId} | returned={ResultCount}",
                    DateTimeOffset.UtcNow,
                    requestId,
                    movieResults.Count);

                return Ok(movieResults);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(
                    ex,
                    "ERROR {TimestampUtc}: SearchMovies external provider failure | requestId={RequestId} | query='{Query}'",
                    DateTimeOffset.UtcNow,
                    requestId,
                    query);

                return StatusCode(503, new
                {
                    error = "movie_service_unavailable",
                    message = "We couldn't reach the movie provider. Please try again shortly.",
                    requestId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "ERROR {TimestampUtc}: SearchMovies unexpected failure | requestId={RequestId}",
                    DateTimeOffset.UtcNow,
                    requestId);

                return StatusCode(500, new
                {
                    error = "server_error",
                    message = "Unexpected error while searching for movies.",
                    requestId
                });
            }
        }

        [HttpPost]
        public IActionResult CaptureMovie([FromBody] Title title)
        {
            if (title == null) return BadRequest();
            if (!User.Identity.IsAuthenticated) return Unauthorized();
            var identityId = _userManager.GetUserId(User);
            var user = _userRepository.GetUser(identityId);
            if (user.Id == null) return Unauthorized();
            _titleRepository.RecordTitleView(title, user.Id);
            return Ok();
        }
    }
}
