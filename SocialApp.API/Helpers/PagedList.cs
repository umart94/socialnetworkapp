using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SocialApp.API.Helpers
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, 
            int pageNumber, int pageSize)
        {
            //users and page numbers taken as source, countasync aggregator counts source, and then the users are sent to
            //items variable... page # 1 - pageSize = 10...
            //items = list of items
            //return new paged list with count pagesize and numbers through this method to userscontroller
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}

/*
helps avoiding performance problems
parameters are passed by query string
http://localhost:5000/api/users?pageNumber=1&pageSize=5

the page size should be limited
we should always page results

deferred Execution
ToListAsync();


store query commands in a variable
execution is deferred
IQueryable<T> creates an expression Tree

Execution:
ToListAsync(),ToArrayAsync(),ToDictionary()
Singleton Queries


*/