using communityWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;

namespace communityWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProjectContext _context;
        private readonly IWebHostEnvironment _environment;
       /* private string user;
        private int uid;*/
        public HomeController(ProjectContext context, IWebHostEnvironment hostEnvironment)
        {
            _environment = hostEnvironment;
            _context = context;
           /* user = HttpContext.Session.GetString("name"); ;
            uid =(int) HttpContext.Session.GetInt32("Id"); ;
*/
        }

        public IActionResult Index()
        {
            var user = HttpContext.Session.GetString("name");
            var uid = HttpContext.Session.GetInt32("Id");

            homeView v = new homeView();
            v.Awards = _context.Awards.ToList();
            v.FriendRequests = _context.FriendRequests.Where(f => f.SenderId == uid).ToList();
            v.FriendRequestsFromFriend = _context.FriendRequests.Where(f=>f.ReceiverId == uid).Include(f=>f.Sender).ToList();
            if (user != null)
            {

                var f = _context.FriendRequests.Where(f => f.Sender.Fname == user || f.Receiver.Fname == user && f.Status == "Approved").Select(f => new
                {
                    name = f.Receiver.Fname
                }).ToList();
                foreach (var i in f)
                {
                    Console.WriteLine("+++++" + i.name);
                }
            }

            /* v.Posts = (from post in _context.Posts
                        join communityMember in _context.CommunityMembers on post.CommunityId equals communityMember.CommunityId
                        where communityMember.UserId == uid
                        select post).ToList();*/
            if (uid != null)
            {

                /*  v.Posts = _context.Posts
          .Where(p => _context.CommunityMembers
              .Where(cm => cm.UserId == uid)
              .Select(cm => cm.CommunityId)
              .Contains(p.CommunityId)).OrderByDescending(p => p.CreatedDate)
          .ToList();*/

                v.Posts = _context.Posts.OrderByDescending(p=>_context.FriendRequests.Any(fr=>fr.Status == "Approved" && (fr.SenderId == uid && fr.ReceiverId == p.UserId)|| (fr.SenderId == p.UserId && fr.ReceiverId == uid))).OrderByDescending(p => p.CreatedDate).Select(p => new postView
                {
                    Id = p.Id,
                    Title = p.Title,
                    CommunityId = p.CommunityId,
                    UserId = p.UserId,
                    Description = p.Description,
                    Type = p.Type,
                    Community = p.Community,
                    User = p.User,
                    UserUpvote = _context.Votes.Count(u => u.PostId == p.Id && u.UserId == uid),
                    UserDownvote = _context.DownVotes.Count(u => u.PostId == p.Id && u.UserId == uid),
                    FileName = p.FileName,
                    CreatedDate = p.CreatedDate,
                    IsActive = p.IsActive,
                    UpvoteCount = _context.Votes.Count(u => u.PostId == p.Id),
                    DownvoteCount = _context.DownVotes.Count(d => d.PostId == p.Id)
                }).ToList();
                /*  var p = _context.Posts.Where(p => _context.CommunityMembers
              .Where(cm => _context.FriendRequests.Select(f=>f.ReceiverId).Contains(cm.UserId))
              .Select(cm => cm.CommunityId)
              .Contains(p.CommunityId)).OrderByDescending(p => p.CreatedDate)
          .ToList();*/

            }
            else
            {

                v.Posts = _context.Posts.OrderByDescending(p => p.CreatedDate).Select(p => new postView
                {
                    Id = p.Id,
                    Title = p.Title,
                    CommunityId = p.CommunityId,
                    Community = p.Community,
                    User = p.User,
                    UserUpvote = _context.Votes.Count(u => u.PostId == p.Id && u.UserId == uid),
                    UserDownvote = _context.DownVotes.Count(u => u.PostId == p.Id && u.UserId == uid),
                    UserId = p.UserId,
                    Description = p.Description,
                    Type = p.Type,
                    FileName = p.FileName,
                    CreatedDate = p.CreatedDate,
                    IsActive = p.IsActive,
                    UpvoteCount = _context.Votes.Count(u => u.PostId == p.Id),
                    DownvoteCount = _context.DownVotes.Count(d => d.PostId == p.Id)
                }).ToList();
            }

            v.Communities = _context.Communities.Include(c => c.Owner).ToList();
            if (user != null)
            {
                v.Community = _context.CommunityMembers.Include(u => u.User).Include(u => u.Community).Where(m => m.User.Fname.ToLower() == user.ToLower()).Select(s => s.Community).ToList();
                
            }

            foreach (var h in Request.Headers)
            {
                Console.WriteLine("----" + h);
            }

            return View(v);
        }
        public IActionResult Details(int id)
        {
            var user = HttpContext.Session.GetString("name");
            var uid = HttpContext.Session.GetInt32("Id");
            SinglePostView sp = new SinglePostView();
            var p = _context.Posts.Where(p => p.Id == id).Select(p => new postView
            {
                Id = p.Id,
                Title = p.Title,
                CommunityId = p.CommunityId,
                Community = p.Community,
                User = p.User,
                UserId = p.UserId,
                Description = p.Description,
                Type = p.Type,
                FileName = p.FileName,
                CreatedDate = p.CreatedDate,
                IsActive = p.IsActive,
                UserUpvote = _context.Votes.Count(u => u.PostId == p.Id && u.UserId == uid),
                UserDownvote = _context.DownVotes.Count(u => u.PostId == p.Id && u.UserId == uid),
                UpvoteCount = _context.Votes.Count(u => u.PostId == p.Id),
                DownvoteCount = _context.DownVotes.Count(d => d.PostId == p.Id)
            }).First();
            var c = _context.PostFeedbacks.Where(p => p.PostId == id).Include(p => p.User).ToList();
            var numberOfMemberInCommunity = _context.CommunityMembers.Where(s => s.Community.Name == p.Community.Name).Count();
            sp.Post = p;
            sp.Feedbacks = c;
            sp.noOfMembers = numberOfMemberInCommunity.ToString();
            if (user != null)
            {

                sp.userCommunity = _context.CommunityMembers.Where(m => m.User.Fname.ToLower() == user.ToLower()).Select(s => s.Community).ToList();
            }


            return View(sp);
        }

        public IActionResult Comment(string text, int id)
        {
            var user = HttpContext.Session.GetString("name");
            if (user != null)
            {

                var u = _context.Users.Where(u => u.Fname == user).First();
                int i = u.Id;

                PostFeedback postFeedback = new PostFeedback();
                postFeedback.UserId = i;
                postFeedback.PostId = id;
                postFeedback.Review = text;
                postFeedback.CreatedDate = DateTime.Now;
                _context.Add(postFeedback);
                _context.SaveChangesAsync();
                return RedirectToAction("details", new { id = id });
            }
            else
            {
                return RedirectToAction("Login", "register");
            }

        }
        public IActionResult Join(int communityId)
        {
            var user = HttpContext.Session.GetString("name");
            if (user == null)
            {
                return RedirectToAction("Login", "Register");
            }
            else
            {
                CommunityMember m = new CommunityMember();
                m.CommunityId = communityId;
                var u = _context.Users.Where(u => u.Fname == user).First();
                m.UserId = u.Id;
                _context.Add(m);
                _context.SaveChangesAsync();
                return Redirect(Request.Headers["Referer"].ToString());
            }

        }
        public IActionResult Leave(int communityId)
        {
            var user = HttpContext.Session.GetString("name");
            var Id = (int)HttpContext.Session.GetInt32("Id");
            if (user == null)
            {
                return RedirectToAction("Login", "Register");
            }
            else
            {
                CommunityMember m = _context.CommunityMembers.Where(c => c.CommunityId == communityId && c.UserId == Id).First();



                _context.CommunityMembers.Remove(m);
                _context.SaveChangesAsync();
                /* return RedirectToAction("communitydetail", new {id = communityId});*/
                return Redirect(Request.Headers["Referer"].ToString());
            }

        }

        public IActionResult SearchPost(string search)
        {
            if(search == null)
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var user = HttpContext.Session.GetString("name");
            if (search != null)
            {

            HttpContext.Session.SetString("search", search);
            }
            var uid = HttpContext.Session.GetInt32("Id");


            SearchPostView v = new SearchPostView();
            v.Posts = _context.Posts.Where(p => p.Title.ToLower().Contains(search.ToLower())).Include(c => c.Community).Include(c => c.User).ToList();
            v.Communities = _context.Communities.Where(c => c.Name.ToLower().Contains(search.ToLower())).Include(c => c.Owner).Take(2).ToList();
            v.Feedbacks = _context.PostFeedbacks.ToList();
            v.Members = _context.CommunityMembers.ToList();
            v.Votes = _context.Votes.ToList();
            v.DownVotes = _context.DownVotes.ToList();
            if (user != null)
            {

                v.userCommunity = _context.CommunityMembers.Where(m => m.User.Fname.ToLower() == user.ToLower()).Select(s => s.Community).ToList();
            }
            v.FriendRequests = _context.FriendRequests.Where(f => f.SenderId == uid).Include(f=>f.Receiver).Include(f=>f.Sender).ToList();
            v.FriendRequestsFromFriend = _context.FriendRequests.Where(f => f.ReceiverId == uid).Include(f => f.Sender).Include(f => f.Receiver).ToList();
            foreach(var i in v.FriendRequests)
            {
                Console.WriteLine("aaa"+i.Receiver.Image);
            }
            v.AllCommunities = _context.Communities.Include(c => c.Owner).ToList();
            if (user != null)
            {
                v.Community = _context.CommunityMembers.Include(u => u.User).Include(u => u.Community).Where(m => m.User.Fname.ToLower() == user.ToLower()).Select(s => s.Community).ToList();

            }
            v.People = _context.Users.Where(u => u.Fname.ToLower().Contains(search.ToLower())).ToList();

            return View(v);

        }
        public IActionResult searchCommunity()
        {
            var uid = HttpContext.Session.GetInt32("Id");

            var user = HttpContext.Session.GetString("name");
            var searchString = HttpContext.Session.GetString("search");
            SearchPostView v = new SearchPostView();
            v.Communities = _context.Communities.Where(c => c.Name.ToLower().Contains(searchString.ToLower())).Include(c => c.Owner).ToList();
            v.Members = _context.CommunityMembers.ToList();
            if (user != null)
            {

                v.userCommunity = _context.CommunityMembers.Where(m => m.User.Fname.ToLower() == user.ToLower()).Select(s => s.Community).ToList();
            }
            v.FriendRequests = _context.FriendRequests.Where(f => f.SenderId == uid).Include(f => f.Receiver).Include(f => f.Sender).ToList();
            v.FriendRequestsFromFriend = _context.FriendRequests.Where(f => f.ReceiverId == uid).Include(f => f.Sender).Include(f => f.Receiver).ToList();

            v.AllCommunities = _context.Communities.Include(c => c.Owner).ToList();
            if (user != null)
            {
                v.Community = _context.CommunityMembers.Include(u => u.User).Include(u => u.Community).Where(m => m.User.Fname.ToLower() == user.ToLower()).Select(s => s.Community).ToList();

            }
            return View(v);
        }

        public IActionResult CommunityDetail(int id,string sortButton)
        {
           
            communityDetailView v = new communityDetailView();
           
            var user = HttpContext.Session.GetString("name");
            if (user != null)
            {
                var a = _context.CommunityMembers.Where(m => m.User.Fname == user && m.CommunityId == id).FirstOrDefault();
                if (a != null)
                {

                    v.IsMember = true;
                }
                else
                {
                    v.IsMember = false;
                }
            }
            else
            {
                v.IsMember = false;
            }
            v.Community = _context.Communities.Where(c => c.Id == id).First();
            v.Members = _context.CommunityMembers.Where(m => m.CommunityId == id).ToList();
            int? i = HttpContext.Session.GetInt32("Id");

            if (sortButton == null)
            {
                ViewBag.SortButton = "Sort By Votes";
                v.Posts = _context.Posts.Include(p=>p.User).Where(p => p.CommunityId == id).Select(p => new postView
                {
                    Id = p.Id,
                    Title = p.Title,
                    CommunityId = p.CommunityId,
                    UserId = p.UserId,
                    Description = p.Description,
                    Type = p.Type,
                    Community = p.Community,
                    User = p.User,
                    UserUpvote = _context.Votes.Count(u => u.PostId == p.Id && u.UserId == i),
                    UserDownvote = _context.DownVotes.Count(u => u.PostId == p.Id && u.UserId == i),
                    FileName = p.FileName,
                    CreatedDate = p.CreatedDate,
                    IsActive = p.IsActive,
                    UpvoteCount = _context.Votes.Count(u => u.PostId == p.Id),
                    DownvoteCount = _context.DownVotes.Count(d => d.PostId == p.Id)
                }).ToList();

            }
            else if (sortButton == "Sort By Votes")
            {
                ViewBag.SortButton = "Unsort";
                v.Posts = _context.Posts.Include(p => p.Votes).OrderByDescending(p => p.Votes.Count - p.DownVotes.Count).Include(p => p.User).Where(p => p.CommunityId == id).Select(p => new postView
                {
                    Id = p.Id,
                    Title = p.Title,
                    CommunityId = p.CommunityId,
                    UserId = p.UserId,
                    Description = p.Description,
                    Type = p.Type,
                    Community = p.Community,
                    User = p.User,
                    UserUpvote = _context.Votes.Count(u => u.PostId == p.Id && u.UserId == i),
                    UserDownvote = _context.DownVotes.Count(u => u.PostId == p.Id && u.UserId == i),
                    FileName = p.FileName,
                    CreatedDate = p.CreatedDate,
                    IsActive = p.IsActive,
                    UpvoteCount = _context.Votes.Count(u => u.PostId == p.Id),
                    DownvoteCount = _context.DownVotes.Count(d => d.PostId == p.Id)
                }).ToList();
            }
            else
            {
                ViewBag.SortButton = "Sort By Votes";
                v.Posts = _context.Posts.Include(p => p.User).Where(p => p.CommunityId == id).Select(p => new postView
                {
                    Id = p.Id,
                    Title = p.Title,
                    CommunityId = p.CommunityId,
                    UserId = p.UserId,
                    Description = p.Description,
                    Type = p.Type,
                    Community = p.Community,
                    User = p.User,
                    UserUpvote = _context.Votes.Count(u => u.PostId == p.Id && u.UserId == i),
                    UserDownvote = _context.DownVotes.Count(u => u.PostId == p.Id && u.UserId == i),
                    FileName = p.FileName,
                    CreatedDate = p.CreatedDate,
                    IsActive = p.IsActive,
                    UpvoteCount = _context.Votes.Count(u => u.PostId == p.Id),
                    DownvoteCount = _context.DownVotes.Count(d => d.PostId == p.Id)
                }).ToList();
            }


            return View(v);
        }
        public IActionResult UserDetail(int id, string sortButton)
        {

            int? i = HttpContext.Session.GetInt32("Id");
            if(id == 0)
            {
                id = i.HasValue ? i.Value : 0;
            }
            var user = HttpContext.Session.GetString("name");
            userDetailView v = new userDetailView();
            v.User = _context.Users.Where(c => c.Id == id).First();
            ViewBag.user = v.User.Fname;
            var f = _context.FriendRequests.Where(f => f.ReceiverId == id && f.Status == "Approved").ToList();
            var ff = _context.FriendRequests.Where(f => f.SenderId == id && f.Status == "Approved").ToList();
            var c = f.Count() + ff.Count();
            v.noOfFriends = c;
            if (user != null)
            {
                var a = _context.CommunityMembers.Where(m => m.User.Fname == user && m.CommunityId == id).FirstOrDefault();
                v.IsMember = true;
                try
                {

                    v.Friend = _context.FriendRequests.Where(f => f.Sender.Fname == user && f.Receiver.Fname == v.User.Fname && f.Status == "Approved").First();
                }
                catch (Exception)
                {


                }
                try
                {
                    v.FriendF = _context.FriendRequests.Where(f => f.Receiver.Fname == user && f.Sender.Fname == v.User.Fname && f.Status == "Approved").First();

                }
                catch (Exception) { }
            }
            else
            {
                v.IsMember = false;
            }

            /*v.Posts = _context.Posts.Where(p => p.UserId == id).Include(p => p.User).Include(p => p.Community).ToList();*/
            /* v.Posts = _context.Posts.Where(p=>p.UserId == id).Select(p => new postView
             {
                 Id = p.Id,
                 Title = p.Title,
                 CommunityId = p.CommunityId,
                 UserId = p.UserId,
                 Description = p.Description,
                 Type = p.Type,
                 Community = p.Community,
                 User = p.User,
                 UserUpvote = _context.Votes.Count(u => u.PostId == p.Id && u.UserId == i),
                 UserDownvote = _context.DownVotes.Count(u => u.PostId == p.Id && u.UserId == i),
                 FileName = p.FileName,
                 CreatedDate = p.CreatedDate,
                 IsActive = p.IsActive,
                 UpvoteCount = _context.Votes.Count(u => u.PostId == p.Id),
                 DownvoteCount = _context.DownVotes.Count(d => d.PostId == p.Id)
             }).ToList();*/
            if (sortButton == null)
            {
                ViewBag.SortButton = "Sort By Votes";
                v.Posts = _context.Posts.Include(p => p.User).Where(p => p.UserId == id).Select(p => new postView
                {
                    Id = p.Id,
                    Title = p.Title,
                    CommunityId = p.CommunityId,
                    UserId = p.UserId,
                    Description = p.Description,
                    Type = p.Type,
                    Community = p.Community,
                    User = p.User,
                    UserUpvote = _context.Votes.Count(u => u.PostId == p.Id && u.UserId == i),
                    UserDownvote = _context.DownVotes.Count(u => u.PostId == p.Id && u.UserId == i),
                    FileName = p.FileName,
                    CreatedDate = p.CreatedDate,
                    IsActive = p.IsActive,
                    UpvoteCount = _context.Votes.Count(u => u.PostId == p.Id),
                    DownvoteCount = _context.DownVotes.Count(d => d.PostId == p.Id)
                }).ToList();

            }
            else if (sortButton == "Sort By Votes")
            {
                ViewBag.SortButton = "Unsort";
                v.Posts = _context.Posts.Include(p => p.Votes).OrderByDescending(p => p.Votes.Count - p.DownVotes.Count).Include(p => p.User).Where(p => p.UserId == id).Select(p => new postView
                {
                    Id = p.Id,
                    Title = p.Title,
                    CommunityId = p.CommunityId,
                    UserId = p.UserId,
                    Description = p.Description,
                    Type = p.Type,
                    Community = p.Community,
                    User = p.User,
                    UserUpvote = _context.Votes.Count(u => u.PostId == p.Id && u.UserId == i),
                    UserDownvote = _context.DownVotes.Count(u => u.PostId == p.Id && u.UserId == i),
                    FileName = p.FileName,
                    CreatedDate = p.CreatedDate,
                    IsActive = p.IsActive,
                    UpvoteCount = _context.Votes.Count(u => u.PostId == p.Id),
                    DownvoteCount = _context.DownVotes.Count(d => d.PostId == p.Id)
                }).ToList();
            }
            else
            {
                ViewBag.SortButton = "Sort By Votes";
                v.Posts = _context.Posts.Include(p => p.User).Where(p => p.UserId == id).Select(p => new postView
                {
                    Id = p.Id,
                    Title = p.Title,
                    CommunityId = p.CommunityId,
                    UserId = p.UserId,
                    Description = p.Description,
                    Type = p.Type,
                    Community = p.Community,
                    User = p.User,
                    UserUpvote = _context.Votes.Count(u => u.PostId == p.Id && u.UserId == i),
                    UserDownvote = _context.DownVotes.Count(u => u.PostId == p.Id && u.UserId == i),
                    FileName = p.FileName,
                    CreatedDate = p.CreatedDate,
                    IsActive = p.IsActive,
                    UpvoteCount = _context.Votes.Count(u => u.PostId == p.Id),
                    DownvoteCount = _context.DownVotes.Count(d => d.PostId == p.Id)
                }).ToList();
            }
            return View(v);
        }

        public IActionResult Follow(int toFollowId)
        {
            if (HttpContext.Session.GetString("name") != null)
            {

                int UserId = (int)HttpContext.Session.GetInt32("Id");

                FriendRequest friendRequest = new FriendRequest();
                friendRequest.SenderId = UserId;
                friendRequest.ReceiverId = toFollowId;
                friendRequest.Status = "Approved";
                friendRequest.SentDate = DateTime.Now;
                _context.Add(friendRequest);
                _context.SaveChangesAsync();
                return RedirectToAction("userdetail", new { id = toFollowId });
            }
            else
            {
                return RedirectToAction("login", "register");
            }


        }
        public IActionResult UnFollow(int toUnFollowId)
        {
            if (HttpContext.Session.GetString("name") != null)
            {

                int UserId = (int)HttpContext.Session.GetInt32("Id");

                var existingFriend = _context.FriendRequests.Where(f => f.ReceiverId == toUnFollowId && f.SenderId == UserId || f.SenderId == toUnFollowId && f.ReceiverId == UserId).First();



                _context.FriendRequests.Remove(existingFriend);
                _context.SaveChangesAsync();
                return RedirectToAction("userdetail", new { id = toUnFollowId });
            }
            else
            {
                return RedirectToAction("login", "register");
            }


        }

        public async Task<IActionResult> AddCommunity(string community, string des, IFormFile file)
        {
            var id = HttpContext.Session.GetInt32("Id");
            if (id != null)
            {

                Community newcommunity = new Community();

                if (file != null)
                {
                    string filename = file.FileName;
                    string filepath = Path.Combine(_environment.WebRootPath, "img", filename);

                    using (var stream = new FileStream(filepath, FileMode.Create))
                    {

                        file.CopyToAsync(stream);
                    }
                    newcommunity.Name = community;
                    newcommunity.Description = des;
                    newcommunity.Logo = filename;
                    newcommunity.IsApproved = true;
                    newcommunity.OwnerId = id;

                }
                newcommunity.CreatedDate = DateTime.Now;
                _context.Add(newcommunity);
                await _context.SaveChangesAsync();

                int i = getId(community);


                return RedirectToAction("communitydetail", new { id = i });
            }
            else
            {
                return RedirectToAction("login", "register");
            }



        }
        public int getId(string name)
        {
            var c = new Community();
            c = _context.Communities.Where(c => c.Name == name).FirstOrDefault();
            int id = c.Id;
            return id;
        }

        public IActionResult CreatePost(int cId)
        {
            SearchPostView v = new SearchPostView();
            var uid = HttpContext.Session.GetInt32("Id");

            var user = HttpContext.Session.GetString("name");
            if (user != null)
            {

                v.userCommunity = _context.CommunityMembers.Where(m => m.User.Fname.ToLower() == user.ToLower()).Select(s => s.Community).ToList();
            }
            v.FriendRequests = _context.FriendRequests.Where(f => f.SenderId == uid).Include(f => f.Receiver).Include(f => f.Sender).ToList();
            v.FriendRequestsFromFriend = _context.FriendRequests.Where(f => f.ReceiverId == uid).Include(f => f.Sender).Include(f => f.Receiver).ToList();
            foreach (var i in v.FriendRequests)
            {
                Console.WriteLine("aaa" + i.Receiver.Image);
            }
            v.AllCommunities = _context.Communities.Include(c => c.Owner).ToList();
            if (user != null)
            {
                v.Community = _context.CommunityMembers.Include(u => u.User).Include(u => u.Community).Where(m => m.User.Fname.ToLower() == user.ToLower()).Select(s => s.Community).ToList();

            }

           
            if (cId != 0)
            {

            var community = _context.Communities.Where(c => c.Id == cId).First();
            ViewBag.cName = community.Name;
            }
            var id = HttpContext.Session.GetInt32("Id");
            if (id != null)
            {
                return View(v);
            }
            else
            {
                
                return RedirectToAction("login", "register");
            }

        }
        public IActionResult SCommunity(string search)
        {
            var communities = _context.Communities.Where(c => c.Name.ToLower().Contains(search.ToLower())).ToList();

            return Json(communities);
        }
        public IActionResult SAllCommunity()
        {
            var communities = _context.Communities.ToList();

            return Json(communities);
        }

        [HttpPost]
        public async Task<IActionResult> AddPost(string title, string des, string community, IFormFile file)
        {
            var id = HttpContext.Session.GetInt32("Id");
            if (id != null)
            {
                Post post = new Post();
                if (file != null)
                {
                    string filename = file.FileName;
                    string filepath = Path.Combine(_environment.WebRootPath, "img", filename);

                    using (var stream = new FileStream(filepath, FileMode.Create))
                    {

                        await file.CopyToAsync(stream);
                    }

                  
                    post.FileName = filename;
                   

                }
                    var c = _context.Communities.Where(c => c.Name.ToLower().Contains(community.ToLower())).First();
                post.Title = title;
                post.Description = des;
                post.Type = "Public";
                post.IsActive = true;
                post.UserId = id;
                post.CommunityId = c.Id;
                post.CreatedDate = DateTime.Now;

                _context.Add(post);
                await _context.SaveChangesAsync();
                int i = getPId(title);
                return RedirectToAction("details", new { id = i });
            }
            else
            {
                return RedirectToAction("login", "register");
            }

        }
        public int getPId(string name)
        {
            var p = _context.Posts.Where(p => p.Title == name).First();
            int i = p.Id;
            return i;
        }
        public IActionResult Upvote(int pId, int uId)
        {
            if (uId == 0)
            {
                return RedirectToAction("login", "Register");
            }
            else
            {
                var a = _context.Votes.Where(v => v.PostId == pId && v.UserId == uId).FirstOrDefault();
                var d = _context.DownVotes.Where(v => v.PostId == pId && v.UserId == uId).FirstOrDefault();
                if (a == null && d == null)
                {
                    var v = new Vote();
                    v.UserId = uId;
                    v.PostId = pId;
                    _context.Add(v);
                    _context.SaveChanges();

                    return Redirect(Request.Headers["Referer"].ToString());

                }
                else if (a == null && d != null)
                {
                    var v = new Vote();
                    v.UserId = uId;
                    v.PostId = pId;
                    _context.Add(v);
                    _context.DownVotes.Remove(d);
                    _context.SaveChanges();


                    return Redirect(Request.Headers["Referer"].ToString());
                }
                else
                {
                    _context.Votes.Remove(a);
                    _context.SaveChanges();
                    return Redirect(Request.Headers["Referer"].ToString());

                }
            }
        }

        public IActionResult Upvotee(int pId, int uId)
        {
            int votes;
            int UserUpvote;
            int UserDownvote;
            if (uId == 0)
            {
                return Json(new { loginRequired = true });
            }
            else
            {
                var a = _context.Votes.Where(v => v.PostId == pId && v.UserId == uId).FirstOrDefault();

                var d = _context.DownVotes.Where(v => v.PostId == pId && v.UserId == uId).FirstOrDefault();
                if (a == null && d == null)
                {
                    var v = new Vote();
                    v.UserId = uId;
                    v.PostId = pId;
                    _context.Add(v);
                    _context.SaveChanges();
                    UserUpvote = _context.Votes.Count(u => u.PostId == pId);
                    UserDownvote = _context.DownVotes.Count(u => u.PostId == pId);
                    votes = UserUpvote - UserDownvote;
                    return Json(new { upvoteCount = votes });

                }
                else if (a == null && d != null)
                {
                    var v = new Vote();
                    v.UserId = uId;
                    v.PostId = pId;
                    _context.Add(v);
                    _context.DownVotes.Remove(d);
                    _context.SaveChanges();
                    UserUpvote = _context.Votes.Count(u => u.PostId == pId);
                    UserDownvote = _context.DownVotes.Count(u => u.PostId == pId);
                    votes = UserUpvote - UserDownvote;
                    return Json(new { upvoteCount = votes });
                }
                else
                {
                    _context.Votes.Remove(a);
                    _context.SaveChanges();
                    UserUpvote = _context.Votes.Count(u => u.PostId == pId);
                    UserDownvote = _context.DownVotes.Count(u => u.PostId == pId);
                    votes = UserUpvote - UserDownvote;
                    return Json(new { upvoteCount = votes });

                }
            }
        }
        public IActionResult Downvotee(int pId, int uId)
        {
            int votes;
            int UserUpvote;
            int UserDownvote;
            if (uId == 0)
            {
                return Json(new { loginRequired = true });
            }
            else
            {
                var a = _context.DownVotes.Where(v => v.PostId == pId && v.UserId == uId).FirstOrDefault();
                var d = _context.Votes.Where(v => v.PostId == pId && v.UserId == uId).FirstOrDefault();

                if (a == null && d == null)
                {
                    var v = new DownVote();
                    v.UserId = uId;
                    v.PostId = pId;
                    _context.Add(v);
                    _context.SaveChanges();
                    UserUpvote = _context.Votes.Count(u => u.PostId == pId);
                    UserDownvote = _context.DownVotes.Count(u => u.PostId == pId);
                    votes = UserUpvote - UserDownvote;
                    return Json(new { upvoteCount = votes });


                }
                else if (a == null && d != null)
                {
                    var v = new DownVote();
                    v.UserId = uId;
                    v.PostId = pId;
                    _context.Add(v);
                    _context.Votes.Remove(d);
                    _context.SaveChanges();
                    UserUpvote = _context.Votes.Count(u => u.PostId == pId);
                    UserDownvote = _context.DownVotes.Count(u => u.PostId == pId);
                    votes = UserUpvote - UserDownvote;
                    return Json(new { upvoteCount = votes });
                }
                else
                {
                    _context.DownVotes.Remove(a);
                    _context.SaveChanges();
                    UserUpvote = _context.Votes.Count(u => u.PostId == pId);
                    UserDownvote = _context.DownVotes.Count(u => u.PostId == pId);
                    votes = UserUpvote - UserDownvote;
                    return Json(new { upvoteCount = votes });

                }
            }
        }
        public IActionResult Downvote(int pId, int uId)
        {
            if (uId == 0)
            {
                return RedirectToAction("login", "Register");
            }
            else
            {
                var a = _context.DownVotes.Where(v => v.PostId == pId && v.UserId == uId).FirstOrDefault();
                var d = _context.Votes.Where(v => v.PostId == pId && v.UserId == uId).FirstOrDefault();

                if (a == null && d == null)
                {
                    var v = new DownVote();
                    v.UserId = uId;
                    v.PostId = pId;
                    _context.Add(v);
                    _context.SaveChanges();
                    return Redirect(Request.Headers["Referer"].ToString());

                }
                else if (a == null && d != null)
                {
                    var v = new DownVote();
                    v.UserId = uId;
                    v.PostId = pId;
                    _context.Add(v);
                    _context.Votes.Remove(d);
                    _context.SaveChanges();
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                else
                {
                    _context.DownVotes.Remove(a);
                    _context.SaveChanges();
                    return Redirect(Request.Headers["Referer"].ToString());

                }
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}