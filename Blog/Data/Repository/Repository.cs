﻿using Blog.Models;
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
            int pageCount = (int)Math.Ceiling((double)postsCount / pageSize);
            return new IndexViewModel
            {
                PageNumber = pageNumber,
                PageCount = pageCount,
                NextPage = postsCount > skipAmount + pageSize,
                Pages = PageNumbers(pageNumber, pageCount),
                Category = Category,
                Posts = query.Skip(skipAmount).Take(pageSize).ToList()
            };
        }

        private IEnumerable<int> PageNumbers(int pageNumber, int pageCount)
        {
            int midPoint = pageNumber < 3 ? 3
                :pageNumber > pageCount - 2 ? pageCount - 2
                :pageNumber;

            int lowerBound = midPoint - 2;
            int upperBound = midPoint + 2;

            if (lowerBound != 1)
            {
                yield return 1;
                if (lowerBound - 1 > 1)
                {
                    yield return -1;
                }
            }

            for (int i = midPoint - 2; i <= midPoint + 2; i++)
            {
                yield return i;
            }

            if (upperBound != pageCount)
            {
                if (pageCount - upperBound > 1)
                {
                    yield return -1;
                }
                yield return pageCount;
            }
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
