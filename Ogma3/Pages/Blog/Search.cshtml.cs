using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Castle.Core.Internal;
using Ogma3.Data;
using Ogma3.Data.Enums;
using Ogma3.Data.Models;

namespace Ogma3.Pages.Blog
{
    public class SearchModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public IList<Blogpost> Posts { get;set; }
        public int PostsCount { get; set; }
        
        public readonly int PerPage = 25;

        public string SearchBy { get; set; }
        public EBlogpostSortingOptions SortBy { get; set; }
        public int PageNumber { get; set; }

        public SearchModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> OnGetAsync([FromQuery] string q, [FromQuery] EBlogpostSortingOptions sort, [FromQuery] int page = 1)
        {
            PageNumber = page;
            SearchBy = q;
            SortBy = sort;
            
            var query = _context.Blogposts.AsQueryable();
            
            if (!q.IsNullOrEmpty())
            {
                // Search by tags
                var tags = q
                    .Split(' ')
                    .Where(x => x.StartsWith('#'))
                    .Select(x => x.ToLower().Trim())
                    .ToArray();
                if (tags.Length > 0)
                    query = query.Where(b => tags.All(i => b.Hashtags.Contains(i)));

                // Search in title
                var search = q
                    .Split(' ')
                    .Where(x => !x.StartsWith('#') && !x.IsNullOrEmpty())
                    .ToList();
                if (search.Count > 0)
                    query = query.Where(b => EF.Functions.Like(b.Title.ToUpper(), $"%{string.Join(' ', search)}%".ToUpper()));

            }
            
            // Sort
            query = sort switch
            {
                EBlogpostSortingOptions.TitleAscending  => query.OrderBy(s => s.Title),
                EBlogpostSortingOptions.TitleDescending => query.OrderByDescending(s => s.Title),
                EBlogpostSortingOptions.DateAscending   => query.OrderBy(s => s.PublishDate),
                EBlogpostSortingOptions.DateDescending  => query.OrderByDescending(s => s.PublishDate),
                EBlogpostSortingOptions.WordsAscending  => query.OrderBy(s => s.WordCount),
                EBlogpostSortingOptions.WordsDescending => query.OrderByDescending(s => s.WordCount),
                _ => query.OrderByDescending(s => s.WordCount)
            };
            
            // Finalize query
            Posts = await query
                .Include(b => b.Author)
                .Skip(Math.Max(0, page - 1) * PerPage)
                .Take(PerPage)
                .AsNoTracking()
                .ToListAsync();
            
            PostsCount = await _context.Blogposts.CountAsync();
            
            return Page();
        }
    }
}