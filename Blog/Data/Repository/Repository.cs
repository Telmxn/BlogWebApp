using Blog.Models;
using Blog.Models.Comments;
using Blog.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Repository
{
    public class Repository : IRepository
    {
        private AppDbContext _ctx;

        public Repository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public void AddPost(Post post)
        {
            _ctx.Posts.Add(post);
        }

        public void AddSubComment(SubComment comment)
        {
            _ctx.SubComments.Add(comment);
        }

        public List<Post> GetAllPosts()
        {
            return _ctx.Posts.ToList();
        }

        public IndexViewModel GetAllPosts(int pageNumber,string Category)
        {
            int pageSize = 5;
            int skipAmount = pageSize * (pageNumber - 1);

            var query = _ctx.Posts.AsQueryable();

            if (!String.IsNullOrEmpty(Category))
                query = query.Where(post => post.Category.ToLower().Equals(Category.ToLower()));

            int postsCount = query.Count();

            return new IndexViewModel
            {
                PageNumber = pageNumber,
                PageCount = (int)Math.Ceiling((double)postsCount / pageSize),
                NextPage = postsCount > skipAmount + pageSize,
                Category = Category,
                Posts = query.Skip(skipAmount).Take(pageSize).ToList()
            };
        }

        public Post GetPost(int id)
        {
            return _ctx.Posts.Include(p => p.MainComments)
                .ThenInclude(mc => mc.SubComments)
                .FirstOrDefault(prop => prop.Id == id);
        }

        public void RemovePost(int id)
        {
            _ctx.Posts.Remove(GetPost(id));
        }

        public async Task<bool> SaveChangesAsync()
        {
            if(await _ctx.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }

        public void UpdatePost(Post post)
        {
            _ctx.Posts.Update(post);
        }
    }
}
