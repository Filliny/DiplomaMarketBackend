using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Helpers;
using DiplomaMarketBackend.Models;
using DiplomaMarketBackend.Parser.Article;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DiplomaMarketBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : Controller
    {
        ILogger<WorkController> _logger;
        BaseContext _context;
        IFileService _fileService;

        public ReviewsController(ILogger<WorkController> logger, BaseContext context, IFileService fileService)
        {
            _logger = logger;
            _context = context;
            _fileService = fileService;
        }


        /// <summary>
        /// Post new article review to api
        /// </summary>
        /// <param name="review">Form entity</param>
        /// <returns>Review entity if all is ok</returns>
        /// <response code="400">If the request value is bad</response>
        [HttpPost]
        [Route("post_review")]
        [RequestSizeLimit(100_000_000)]
        public async Task<ActionResult<Review>> PostReview([FromForm] Review review)
        {
            var article = _context.Articles.FirstOrDefault(a=>a.Id == review.article_id);

            //todo test this
            var user = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

            if (article != null)
            {
                var review_model = review.Adapt<ReviewModel>();
                review_model.ReviewApproved = false;
                review_model.DateTime = DateTime.Now;

                //if(user != null)
                //{
                //    review_model.User = user;
                //}

                if(review.images.Count > 0)
                {
                    foreach (var image in review.images)
                    {
                        if (image != null) {
                            
                            var img_id = await _fileService.SaveFileFromStream(BucketNames.review.ToString(),image.Name,image.OpenReadStream());
                            review_model.UserImages.Add(img_id);
                        
                        }
                    }
                }

                _context.Reviews.Add(review_model);
                _context.SaveChanges();

                return Ok(review);
            }

            return BadRequest("Check form parameters!");
        }

        /// <summary>
        /// Get concrete review by id (for admin edit)
        /// </summary>
        /// <param name="review_id">review id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-review")]
        public async Task<ActionResult<Review>> GetReview([FromQuery] string review_id)
        {
            if(int.TryParse(review_id,out int id))
            {
               var review = await _context.Reviews.FirstOrDefaultAsync(r=>r.Id == id);

                if(review != null)
                {
                    var result = review.Adapt<Review>();

                    return Ok(result);

                }

                return NotFound("Check id!");
            }

            return BadRequest("Check parameters!");
        }

        /// <summary>
        /// Update existing review (for admin page)
        /// </summary>
        /// <param name="review">From form</param>
        /// <returns></returns>
        [HttpPut]
        [Route("update_review")]
        public async Task<IActionResult> UpdateReview([FromForm]Review review)
        {
            var exist_review = review.Adapt<ReviewModel>();

            try
            {
                _context.Reviews.Update(exist_review);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {

                return UnprocessableEntity(e.Message);
            }

        }

        /// <summary>
        /// Delete review by id
        /// </summary>
        /// <param name="review_id">review id</param>
        /// <returns>Deleted review entity if success</returns>
        /// <response code="404">If the item is not exist</response>
        /// <response code="400">If the request value is bad</response>
        [HttpDelete]
        //[Authorize(Roles="Admin")]
        [Route("delete_review")]
        public async Task<IActionResult> DeleteReview([FromQuery] string review_id)
        {
            if(int.TryParse(review_id, out int id))
            {
                var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);

                if (review != null)
                {
                    _context.Reviews.Remove(review);    
                    _context.SaveChanges();
                    return Ok(review);
                }

                return NotFound("No such review or id is wrong!");
            }

            return BadRequest("Check parameters!");
        }

        /// <summary>
        /// Create new answer for review_id  in form
        /// </summary>
        /// <param name="answer">Form data</param>
        /// <returns>Form entity if success</returns>
        /// <response code="404">If the item review is not exist</response>
        /// <response code="400">If the request values is bad</response>
        [HttpPost]
        [Route("post_answer")]
        public async Task<IActionResult> PostAnswer([FromForm] Answer answer)
        {
            var review = _context.Reviews.FirstOrDefault(r => r.Id == answer.review_id);

            
            var user = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

            if (review != null)
            {
                var new_answer = answer.Adapt<ReviewModel>();
                new_answer.Type = ReviewType.answer;
                //new_answer.User = user;

                _context.Reviews.Add(new_answer);
                _context.SaveChanges();

                return Ok(answer);
            }

            return NotFound("Review not found or id is wrong!");
        }


        /// <summary>
        /// Deletes answer by id
        /// </summary>
        /// <param name="answer_id">answer id to delete</param>
        /// <returns>Deleted answer entity if success</returns>
        /// <response code="404">If the item review is not exist</response>
        /// <response code="400">If the request values is bad</response>
        [HttpDelete]
        //[Authorize(Roles = "Admin")]
        [Route("delete_answer")]
        public async Task<IActionResult> DeleteAnswer([FromQuery] string answer_id)
        {
            if (int.TryParse(answer_id, out int id))
            {
                var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);

                if (review != null)
                {
                    _context.Reviews.Remove(review);
                    _context.SaveChanges();
                    return Ok(review);
                }

                return NotFound("No such answer or id is wrong!");
            }

            return BadRequest("Check parameters!");
        }

        /// <summary>
        /// Get article reviews with answers included
        /// </summary>
        /// <param name="lang">language</param>
        /// <param name="article_id">id of article to get reviews</param>
        /// <param name="sort">sorting/filtering parameter, values : from_buyer,by_date,helpful,with_attach</param>
        /// <param name="type">type of list to get : review/answer</param>
        /// <param name="limit">number of items to get</param>
        /// <param name="page">page of items</param>
        /// <returns>List of article reviews</returns>
        /// <response code="404">If the article is not exist</response>
        /// <response code="400">If the request values is bad</response>
        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> GetArticleReviews([FromQuery]string lang,string article_id,string sort, string type,string limit, string page) {

            lang = lang.NormalizeLang();
            
            if(int.TryParse(article_id, out int id) && int.TryParse(limit,out int quantity) && int.TryParse(page,out int page_num) ) {

                if (!Enum.TryParse(type.ToLower(), out ReviewType reviewType))
                    reviewType = ReviewType.review;

                if (!Enum.TryParse(sort.ToLower(), out ReviewSort reviewSort))
                    reviewSort = ReviewSort.from_buyer;

                var article = _context.Articles.FirstOrDefault(a=>a.Id == id);

                if(article == null) return NotFound("Article not found!");

                var reviews = _context.Reviews.Where(r => r.ArticleId == id);

                if(reviewType == ReviewType.review)
                {
                    reviews = reviews.Where(r => r.Type == ReviewType.review);
                }
                else
                {
                    reviews = reviews.Where(r => r.Type == ReviewType.answer);
                }

                if(reviewSort == ReviewSort.from_buyer)
                {

                    var byers = _context.OrderItems.Include(r => r.Order).Where(i => i.ArticleId == id).GroupBy(i => i.Order.UserId).Select(g => g.Key).ToList();

                    reviews = reviews.Where(r => byers.Contains(r.UserId));

                }
                else if(reviewSort == ReviewSort.by_date)
                {
                    reviews =  reviews.OrderBy(r => r.DateTime);
                }
                else if(reviewSort == ReviewSort.helpful)
                {
                    reviews = reviews.OrderBy(r => r.Likes);
                }
                else if(reviewSort == ReviewSort.with_attach)
                {
                    reviews = reviews.Where(r => r.UserImages.Count != 0);
                }

                var  reviews_count = reviews.Count();
                var pages = (int)Math.Ceiling((decimal)reviews_count / (decimal)quantity);
                var skip = quantity * (page_num-1);

                var filtered = await reviews.Skip(skip).Take(quantity).ToListAsync();

                var result = new
                {
                    reviews = new List<dynamic>(),
                    items_total = filtered.Count,
                    pages,
                    page_displayed = page_num,
                    limit = quantity
                };

                foreach (var item in filtered)
                {
                    var answers = _context.Reviews.Where(a=>a.ReviewId ==  item.Id && a.Type == ReviewType.answer).ToList();

                    var rev_out = new
                    {
                        id = item.Id,
                        iser_name = item.Name,
                        review_type = item.Type.ToString(),
                        comment = item.Comment,
                        pros = item.Pros,
                        cons = item.Cons,
                        rate = item.Rate,
                        answers = new List<dynamic>(),
                        video_url = item.VideoUrl,
                        user_images = new List<string>(),
                        likes = item.Likes,
                        dislikes = item.Dislikes,
                    };

                    foreach(var image in item.UserImages)
                    {
                        rev_out.user_images.Add(Request.GetImageURL(BucketNames.review,image));
                    }
                    
                    foreach(var answer in answers)
                    {
                        rev_out.answers.Add(new
                        {
                            id= answer.Id,
                            comment = answer.Comment,
                            user_name = answer.Name
                        });
                    }

                    result.reviews.Add(rev_out);

                }

                return new JsonResult(new { data = result });
            }
       
            return BadRequest("Check parameters!");

        }
    }
}
