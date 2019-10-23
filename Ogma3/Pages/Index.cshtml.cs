﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ogma3.Data;
using Ogma3.Data.Models;
using Utils;

namespace Ogma3.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public string SampleText { get; set; }

        public List<Story> Stories { get; set; }

        public IndexModel(ApplicationDbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            Stories = await _context.Stories
                .Include(s => s.StoryTags)
                .ThenInclude(st => st.Tag)
                .ThenInclude(t => t.Namespace)
                .ToListAsync();
            
            SampleText = Lorem.Ipsum(5, new IpsumOptions
            {
                Decorate = true,
                Length = IpsumLength.Short
            });
        }
    }
}
