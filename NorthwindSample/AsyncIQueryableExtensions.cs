﻿using Microsoft.EntityFrameworkCore;
using NorthwindSample.Paged;

namespace NorthwindSample
{
    public static class AsyncIQueryableExtensions
    {
        public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize)
        {
            if (source == null)
                return new PagedList<T>(new List<T>(), pageIndex, pageSize);

            pageSize = Math.Max(pageSize, 1);

            var data = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();


            return new PagedList<T>(data, pageIndex, pageSize);
            
        }
    }
}